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
<summary>Benchmark results (2026-11-22 03:20 UTC)</summary>

Workflow run: https://github.com/guitarrapc/SortAlgorithms/actions/runs/44978772219

### DistributionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.67GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.5 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method              | Size | Pattern            | Mean           | Error        | StdDev       | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| -------------------- |----- |------------------- |---------------:|-------------:|-------------:|-------:|--------:|-----:|----------:|------------:|
| **CountingSort**        | **256**  | **Random**             |     **1,291.1 ns** |      **4.58 ns** |      **1.63 ns** |   **1.41** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | Random             |       914.6 ns |      8.84 ns |      3.93 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | Random             |     1,171.1 ns |     22.56 ns |     11.80 ns |   1.28 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | Random             |       687.8 ns |     12.59 ns |      6.59 ns |   0.75 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | Random             |     1,425.5 ns |     29.67 ns |     13.17 ns |   1.56 |    0.01 |    2 |         - |          NA |
| BucketSortInteger   | 256  | Random             |     1,169.9 ns |     24.81 ns |     12.97 ns |   1.28 |    0.01 |    2 |         - |          NA |
| FlashSort           | 256  | Random             |     3,993.9 ns |    402.18 ns |    210.35 ns |   4.37 |    0.22 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | Random             |     2,184.8 ns |     24.31 ns |     10.79 ns |   2.39 |    0.01 |    3 |         - |          NA |
| RotateLSDRadixSort  | 256  | Random             |    53,103.3 ns |    422.76 ns |    187.71 ns |  58.06 |    0.30 |    7 |         - |          NA |
| RadixLSD256Sort     | 256  | Random             |     1,046.4 ns |     24.18 ns |     10.74 ns |   1.14 |    0.01 |    2 |         - |          NA |
| Radsort             | 256  | Random             |    10,169.2 ns |  5,040.36 ns |  2,636.20 ns |  11.12 |    2.72 |    6 |         - |          NA |
| RadixLSD10Sort      | 256  | Random             |     3,159.4 ns |     18.64 ns |      8.27 ns |   3.45 |    0.02 |    4 |         - |          NA |
| RadixMSD4Sort       | 256  | Random             |     5,300.4 ns |    261.45 ns |    136.74 ns |   5.80 |    0.14 |    5 |         - |          NA |
| RadixMSD10Sort      | 256  | Random             |     4,673.5 ns |     30.11 ns |     10.74 ns |   5.11 |    0.02 |    5 |         - |          NA |
| AmericanFlagSort    | 256  | Random             |     2,412.3 ns |    154.41 ns |     68.56 ns |   2.64 |    0.07 |    3 |         - |          NA |
| SkaSort             | 256  | Random             |     3,248.2 ns |    145.41 ns |     64.56 ns |   3.55 |    0.07 |    4 |         - |          NA |
| SpreadSort          | 256  | Random             |     6,236.0 ns |     53.37 ns |     23.69 ns |   6.82 |    0.04 |    5 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **256**  | **SingleElementMoved** |     **1,266.4 ns** |     **20.06 ns** |      **7.15 ns** |   **1.59** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 256  | SingleElementMoved |       795.9 ns |      8.10 ns |      3.60 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 256  | SingleElementMoved |     1,134.7 ns |      6.46 ns |      3.38 ns |   1.43 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 256  | SingleElementMoved |       711.0 ns |    176.30 ns |     92.21 ns |   0.89 |    0.11 |    1 |         - |          NA |
| BucketSort          | 256  | SingleElementMoved |     1,378.4 ns |     20.53 ns |      9.11 ns |   1.73 |    0.01 |    2 |         - |          NA |
| BucketSortInteger   | 256  | SingleElementMoved |     1,120.3 ns |     18.17 ns |      9.50 ns |   1.41 |    0.01 |    2 |         - |          NA |
| FlashSort           | 256  | SingleElementMoved |     3,272.1 ns |    342.82 ns |    179.30 ns |   4.11 |    0.21 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | SingleElementMoved |     2,230.9 ns |     55.57 ns |     29.06 ns |   2.80 |    0.04 |    4 |         - |          NA |
| RotateLSDRadixSort  | 256  | SingleElementMoved |    34,810.8 ns |    331.31 ns |    173.28 ns |  43.74 |    0.28 |    8 |         - |          NA |
| RadixLSD256Sort     | 256  | SingleElementMoved |       991.8 ns |      4.49 ns |      1.99 ns |   1.25 |    0.01 |    2 |         - |          NA |
| Radsort             | 256  | SingleElementMoved |     7,299.3 ns |     10.93 ns |      4.85 ns |   9.17 |    0.04 |    7 |         - |          NA |
| RadixLSD10Sort      | 256  | SingleElementMoved |     3,221.5 ns |    216.32 ns |    113.14 ns |   4.05 |    0.14 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | SingleElementMoved |     5,557.3 ns |    209.35 ns |    109.50 ns |   6.98 |    0.13 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | SingleElementMoved |     4,665.6 ns |    139.92 ns |     62.13 ns |   5.86 |    0.08 |    6 |         - |          NA |
| AmericanFlagSort    | 256  | SingleElementMoved |     1,702.5 ns |     16.76 ns |      7.44 ns |   2.14 |    0.01 |    3 |         - |          NA |
| SkaSort             | 256  | SingleElementMoved |     2,424.5 ns |     48.24 ns |     21.42 ns |   3.05 |    0.03 |    4 |         - |          NA |
| SpreadSort          | 256  | SingleElementMoved |       804.3 ns |     81.77 ns |     36.31 ns |   1.01 |    0.04 |    1 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **256**  | **Sorted**             |     **1,213.6 ns** |     **60.41 ns** |     **31.59 ns** |   **1.64** |    **0.04** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 256  | Sorted             |       742.1 ns |      6.90 ns |      3.61 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 256  | Sorted             |     1,111.2 ns |     17.63 ns |      6.29 ns |   1.50 |    0.01 |    4 |         - |          NA |
| PigeonSortInteger   | 256  | Sorted             |       565.1 ns |     15.67 ns |      8.20 ns |   0.76 |    0.01 |    2 |         - |          NA |
| BucketSort          | 256  | Sorted             |     1,296.8 ns |     11.23 ns |      4.99 ns |   1.75 |    0.01 |    4 |         - |          NA |
| BucketSortInteger   | 256  | Sorted             |     1,028.0 ns |      7.96 ns |      3.54 ns |   1.39 |    0.01 |    4 |         - |          NA |
| FlashSort           | 256  | Sorted             |     2,843.7 ns |    125.32 ns |     65.55 ns |   3.83 |    0.09 |    6 |         - |          NA |
| RadixLSD4Sort       | 256  | Sorted             |     2,164.7 ns |    155.79 ns |     69.17 ns |   2.92 |    0.09 |    5 |         - |          NA |
| RotateLSDRadixSort  | 256  | Sorted             |    34,264.1 ns |    196.95 ns |    103.01 ns |  46.17 |    0.25 |    9 |         - |          NA |
| RadixLSD256Sort     | 256  | Sorted             |       894.2 ns |     10.47 ns |      4.65 ns |   1.20 |    0.01 |    4 |         - |          NA |
| Radsort             | 256  | Sorted             |     7,770.4 ns |     67.08 ns |     35.09 ns |  10.47 |    0.07 |    8 |         - |          NA |
| RadixLSD10Sort      | 256  | Sorted             |     3,047.0 ns |     50.87 ns |     18.14 ns |   4.11 |    0.03 |    6 |         - |          NA |
| RadixMSD4Sort       | 256  | Sorted             |     5,362.1 ns |    150.25 ns |     66.71 ns |   7.23 |    0.09 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | Sorted             |     4,686.6 ns |    268.89 ns |    140.63 ns |   6.32 |    0.18 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | Sorted             |     1,289.6 ns |     53.16 ns |     23.60 ns |   1.74 |    0.03 |    4 |         - |          NA |
| SkaSort             | 256  | Sorted             |     2,012.8 ns |     12.86 ns |      5.71 ns |   2.71 |    0.01 |    5 |         - |          NA |
| SpreadSort          | 256  | Sorted             |       253.1 ns |      4.56 ns |      2.03 ns |   0.34 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **256**  | **Reversed**           |     **1,174.0 ns** |     **10.95 ns** |      **5.73 ns** |   **1.55** |    **0.01** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 256  | Reversed           |       756.3 ns |      7.74 ns |      4.05 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 256  | Reversed           |     1,165.2 ns |     12.92 ns |      5.73 ns |   1.54 |    0.01 |    4 |         - |          NA |
| PigeonSortInteger   | 256  | Reversed           |       578.7 ns |     18.55 ns |      8.24 ns |   0.77 |    0.01 |    2 |         - |          NA |
| BucketSort          | 256  | Reversed           |     1,392.0 ns |     16.00 ns |      5.71 ns |   1.84 |    0.01 |    4 |         - |          NA |
| BucketSortInteger   | 256  | Reversed           |     1,189.0 ns |     14.52 ns |      6.45 ns |   1.57 |    0.01 |    4 |         - |          NA |
| FlashSort           | 256  | Reversed           |     2,708.3 ns |     28.99 ns |     12.87 ns |   3.58 |    0.02 |    6 |         - |          NA |
| RadixLSD4Sort       | 256  | Reversed           |     2,252.9 ns |    141.30 ns |     62.74 ns |   2.98 |    0.08 |    5 |         - |          NA |
| RotateLSDRadixSort  | 256  | Reversed           |    34,994.1 ns |    151.33 ns |     79.15 ns |  46.27 |    0.25 |    9 |         - |          NA |
| RadixLSD256Sort     | 256  | Reversed           |       899.2 ns |      8.01 ns |      4.19 ns |   1.19 |    0.01 |    3 |         - |          NA |
| Radsort             | 256  | Reversed           |     7,320.5 ns |     28.09 ns |     12.47 ns |   9.68 |    0.05 |    8 |         - |          NA |
| RadixLSD10Sort      | 256  | Reversed           |     3,193.3 ns |    228.48 ns |    119.50 ns |   4.22 |    0.15 |    6 |         - |          NA |
| RadixMSD4Sort       | 256  | Reversed           |     5,583.7 ns |    253.76 ns |    132.72 ns |   7.38 |    0.17 |    7 |         - |          NA |
| RadixMSD10Sort      | 256  | Reversed           |     4,765.9 ns |    237.34 ns |    124.13 ns |   6.30 |    0.16 |    7 |         - |          NA |
| AmericanFlagSort    | 256  | Reversed           |     1,337.7 ns |      9.02 ns |      4.00 ns |   1.77 |    0.01 |    4 |         - |          NA |
| SkaSort             | 256  | Reversed           |     2,216.5 ns |     27.08 ns |     12.02 ns |   2.93 |    0.02 |    5 |         - |          NA |
| SpreadSort          | 256  | Reversed           |       373.7 ns |      4.26 ns |      2.23 ns |   0.49 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **256**  | **PipeOrgan**          |     **1,197.8 ns** |      **2.60 ns** |      **0.93 ns** |   **1.52** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | PipeOrgan          |       787.6 ns |      6.45 ns |      3.37 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 256  | PipeOrgan          |     1,117.4 ns |     13.63 ns |      4.86 ns |   1.42 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | PipeOrgan          |       628.2 ns |      7.30 ns |      3.82 ns |   0.80 |    0.01 |    1 |         - |          NA |
| BucketSort          | 256  | PipeOrgan          |     1,360.7 ns |     13.91 ns |      6.18 ns |   1.73 |    0.01 |    3 |         - |          NA |
| BucketSortInteger   | 256  | PipeOrgan          |     1,164.0 ns |     18.18 ns |      8.07 ns |   1.48 |    0.01 |    3 |         - |          NA |
| FlashSort           | 256  | PipeOrgan          |     3,456.1 ns |    265.69 ns |    138.96 ns |   4.39 |    0.17 |    5 |         - |          NA |
| RadixLSD4Sort       | 256  | PipeOrgan          |     2,162.5 ns |     42.00 ns |     18.65 ns |   2.75 |    0.02 |    4 |         - |          NA |
| RotateLSDRadixSort  | 256  | PipeOrgan          |    34,588.4 ns |    165.31 ns |     73.40 ns |  43.92 |    0.20 |    8 |         - |          NA |
| RadixLSD256Sort     | 256  | PipeOrgan          |       982.7 ns |      4.50 ns |      2.35 ns |   1.25 |    0.01 |    3 |         - |          NA |
| Radsort             | 256  | PipeOrgan          |     7,276.4 ns |     16.93 ns |      8.85 ns |   9.24 |    0.04 |    7 |         - |          NA |
| RadixLSD10Sort      | 256  | PipeOrgan          |     3,070.1 ns |     34.70 ns |     12.37 ns |   3.90 |    0.02 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | PipeOrgan          |     5,732.8 ns |    218.85 ns |    114.46 ns |   7.28 |    0.14 |    6 |         - |          NA |
| RadixMSD10Sort      | 256  | PipeOrgan          |     4,874.6 ns |    244.11 ns |    127.67 ns |   6.19 |    0.15 |    6 |         - |          NA |
| AmericanFlagSort    | 256  | PipeOrgan          |     1,887.9 ns |     17.25 ns |      9.02 ns |   2.40 |    0.01 |    4 |         - |          NA |
| SkaSort             | 256  | PipeOrgan          |     2,678.3 ns |     15.77 ns |      7.00 ns |   3.40 |    0.02 |    5 |         - |          NA |
| SpreadSort          | 256  | PipeOrgan          |     1,354.5 ns |     19.17 ns |      6.84 ns |   1.72 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **256**  | **ManyDuplicates**     |     **1,223.0 ns** |     **12.80 ns** |      **5.68 ns** |   **1.57** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 256  | ManyDuplicates     |       778.3 ns |     29.41 ns |     10.49 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 256  | ManyDuplicates     |     1,374.5 ns |    251.84 ns |    131.72 ns |   1.77 |    0.16 |    3 |         - |          NA |
| PigeonSortInteger   | 256  | ManyDuplicates     |       749.5 ns |    116.98 ns |     51.94 ns |   0.96 |    0.06 |    1 |         - |          NA |
| BucketSort          | 256  | ManyDuplicates     |     2,189.6 ns |    149.16 ns |     78.02 ns |   2.81 |    0.10 |    5 |         - |          NA |
| BucketSortInteger   | 256  | ManyDuplicates     |     1,323.1 ns |     99.63 ns |     44.24 ns |   1.70 |    0.06 |    3 |         - |          NA |
| FlashSort           | 256  | ManyDuplicates     |     3,784.1 ns |     97.11 ns |     43.12 ns |   4.86 |    0.08 |    6 |         - |          NA |
| RadixLSD4Sort       | 256  | ManyDuplicates     |     1,781.7 ns |     15.15 ns |      5.40 ns |   2.29 |    0.03 |    4 |         - |          NA |
| RotateLSDRadixSort  | 256  | ManyDuplicates     |    46,416.0 ns |    311.47 ns |    138.29 ns |  59.65 |    0.76 |    9 |         - |          NA |
| RadixLSD256Sort     | 256  | ManyDuplicates     |     1,017.7 ns |     28.20 ns |     12.52 ns |   1.31 |    0.02 |    2 |         - |          NA |
| Radsort             | 256  | ManyDuplicates     |     8,899.0 ns |    333.45 ns |    174.40 ns |  11.44 |    0.26 |    8 |         - |          NA |
| RadixLSD10Sort      | 256  | ManyDuplicates     |     2,243.7 ns |      6.56 ns |      3.43 ns |   2.88 |    0.04 |    5 |         - |          NA |
| RadixMSD4Sort       | 256  | ManyDuplicates     |     2,561.6 ns |     26.15 ns |      9.33 ns |   3.29 |    0.04 |    5 |         - |          NA |
| RadixMSD10Sort      | 256  | ManyDuplicates     |     2,945.4 ns |     38.06 ns |     13.57 ns |   3.79 |    0.05 |    5 |         - |          NA |
| AmericanFlagSort    | 256  | ManyDuplicates     |     2,449.6 ns |    100.62 ns |     44.67 ns |   3.15 |    0.07 |    5 |         - |          NA |
| SkaSort             | 256  | ManyDuplicates     |     2,828.9 ns |     79.20 ns |     41.42 ns |   3.64 |    0.07 |    5 |         - |          NA |
| SpreadSort          | 256  | ManyDuplicates     |     5,584.0 ns |    270.79 ns |    141.63 ns |   7.18 |    0.19 |    7 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **1024** | **Random**             |     **4,911.5 ns** |    **195.92 ns** |    **102.47 ns** |   **1.47** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Random             |     3,336.4 ns |     15.71 ns |      8.22 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | Random             |     3,922.5 ns |     35.41 ns |     12.63 ns |   1.18 |    0.00 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | Random             |     2,623.3 ns |    237.38 ns |    124.15 ns |   0.79 |    0.04 |    1 |         - |          NA |
| BucketSort          | 1024 | Random             |     5,334.9 ns |    134.24 ns |     59.60 ns |   1.60 |    0.02 |    2 |         - |          NA |
| BucketSortInteger   | 1024 | Random             |     4,298.9 ns |    258.67 ns |    135.29 ns |   1.29 |    0.04 |    2 |         - |          NA |
| FlashSort           | 1024 | Random             |    15,351.8 ns |    250.47 ns |    131.00 ns |   4.60 |    0.04 |    5 |         - |          NA |
| RadixLSD4Sort       | 1024 | Random             |    10,533.5 ns |    259.58 ns |    135.76 ns |   3.16 |    0.04 |    4 |         - |          NA |
| RotateLSDRadixSort  | 1024 | Random             |   288,715.0 ns | 23,456.74 ns | 10,414.94 ns |  86.54 |    2.92 |    8 |         - |          NA |
| RadixLSD256Sort     | 1024 | Random             |     5,230.2 ns |     47.29 ns |     16.87 ns |   1.57 |    0.01 |    2 |         - |          NA |
| Radsort             | 1024 | Random             |    29,415.5 ns |    366.52 ns |    191.70 ns |   8.82 |    0.06 |    7 |         - |          NA |
| RadixLSD10Sort      | 1024 | Random             |    16,342.7 ns |     50.86 ns |     26.60 ns |   4.90 |    0.01 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | Random             |    22,355.1 ns |    256.79 ns |    134.31 ns |   6.70 |    0.04 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | Random             |    22,473.0 ns |    209.16 ns |    109.39 ns |   6.74 |    0.03 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | Random             |    16,924.4 ns |    103.36 ns |     45.89 ns |   5.07 |    0.02 |    5 |         - |          NA |
| SkaSort             | 1024 | Random             |    12,359.2 ns |    323.28 ns |    169.08 ns |   3.70 |    0.05 |    4 |         - |          NA |
| SpreadSort          | 1024 | Random             |     6,839.4 ns |    273.55 ns |    143.07 ns |   2.05 |    0.04 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **1024** | **SingleElementMoved** |     **4,924.4 ns** |    **159.26 ns** |     **83.30 ns** |   **1.68** |    **0.03** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 1024 | SingleElementMoved |     2,934.9 ns |     11.61 ns |      6.07 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 1024 | SingleElementMoved |     3,873.1 ns |     26.84 ns |     11.92 ns |   1.32 |    0.00 |    2 |         - |          NA |
| PigeonSortInteger   | 1024 | SingleElementMoved |     2,470.5 ns |     26.42 ns |     11.73 ns |   0.84 |    0.00 |    1 |         - |          NA |
| BucketSort          | 1024 | SingleElementMoved |     5,176.9 ns |    266.53 ns |    139.40 ns |   1.76 |    0.04 |    2 |         - |          NA |
| BucketSortInteger   | 1024 | SingleElementMoved |     4,114.5 ns |     28.16 ns |     14.73 ns |   1.40 |    0.01 |    2 |         - |          NA |
| FlashSort           | 1024 | SingleElementMoved |    12,160.3 ns |    303.80 ns |    134.89 ns |   4.14 |    0.04 |    4 |         - |          NA |
| RadixLSD4Sort       | 1024 | SingleElementMoved |    11,043.3 ns |    559.89 ns |    292.83 ns |   3.76 |    0.09 |    4 |         - |          NA |
| RotateLSDRadixSort  | 1024 | SingleElementMoved |   176,736.0 ns |  1,660.36 ns |    868.40 ns |  60.22 |    0.30 |    7 |         - |          NA |
| RadixLSD256Sort     | 1024 | SingleElementMoved |     5,217.7 ns |     63.61 ns |     33.27 ns |   1.78 |    0.01 |    2 |         - |          NA |
| Radsort             | 1024 | SingleElementMoved |    19,309.2 ns |    177.56 ns |     78.84 ns |   6.58 |    0.03 |    6 |         - |          NA |
| RadixLSD10Sort      | 1024 | SingleElementMoved |    15,978.5 ns |    164.51 ns |     86.04 ns |   5.44 |    0.03 |    5 |         - |          NA |
| RadixMSD4Sort       | 1024 | SingleElementMoved |    24,201.9 ns |    376.75 ns |    197.05 ns |   8.25 |    0.07 |    6 |         - |          NA |
| RadixMSD10Sort      | 1024 | SingleElementMoved |    22,431.8 ns |    292.94 ns |    153.21 ns |   7.64 |    0.05 |    6 |         - |          NA |
| AmericanFlagSort    | 1024 | SingleElementMoved |     9,062.5 ns |    457.82 ns |    203.28 ns |   3.09 |    0.06 |    3 |         - |          NA |
| SkaSort             | 1024 | SingleElementMoved |     8,346.8 ns |    300.19 ns |    133.29 ns |   2.84 |    0.04 |    3 |         - |          NA |
| SpreadSort          | 1024 | SingleElementMoved |     4,456.0 ns |     15.57 ns |      5.55 ns |   1.52 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **1024** | **Sorted**             |     **4,446.6 ns** |    **171.70 ns** |     **89.80 ns** |   **1.54** |    **0.03** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Sorted             |     2,893.6 ns |     27.90 ns |      9.95 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 1024 | Sorted             |     3,976.5 ns |    280.12 ns |    146.51 ns |   1.37 |    0.05 |    4 |         - |          NA |
| PigeonSortInteger   | 1024 | Sorted             |     2,172.1 ns |     28.67 ns |     10.22 ns |   0.75 |    0.00 |    2 |         - |          NA |
| BucketSort          | 1024 | Sorted             |     4,883.6 ns |     33.24 ns |     14.76 ns |   1.69 |    0.01 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | Sorted             |     3,823.7 ns |     14.60 ns |      6.48 ns |   1.32 |    0.00 |    4 |         - |          NA |
| FlashSort           | 1024 | Sorted             |    11,196.2 ns |    257.59 ns |    134.73 ns |   3.87 |    0.05 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | Sorted             |    12,143.8 ns |    229.23 ns |    119.89 ns |   4.20 |    0.04 |    6 |         - |          NA |
| RotateLSDRadixSort  | 1024 | Sorted             |   174,612.0 ns |    831.10 ns |    369.02 ns |  60.34 |    0.23 |    9 |         - |          NA |
| RadixLSD256Sort     | 1024 | Sorted             |     4,952.9 ns |    245.27 ns |    128.28 ns |   1.71 |    0.04 |    4 |         - |          NA |
| Radsort             | 1024 | Sorted             |    19,060.5 ns |    242.49 ns |    107.66 ns |   6.59 |    0.04 |    8 |         - |          NA |
| RadixLSD10Sort      | 1024 | Sorted             |    15,774.0 ns |    129.31 ns |     57.41 ns |   5.45 |    0.03 |    7 |         - |          NA |
| RadixMSD4Sort       | 1024 | Sorted             |    23,401.2 ns |    194.07 ns |    101.50 ns |   8.09 |    0.04 |    8 |         - |          NA |
| RadixMSD10Sort      | 1024 | Sorted             |    22,682.1 ns |    317.88 ns |    166.26 ns |   7.84 |    0.06 |    8 |         - |          NA |
| AmericanFlagSort    | 1024 | Sorted             |     7,440.0 ns |    316.13 ns |    165.34 ns |   2.57 |    0.05 |    5 |         - |          NA |
| SkaSort             | 1024 | Sorted             |     7,156.3 ns |    437.26 ns |    228.69 ns |   2.47 |    0.08 |    5 |         - |          NA |
| SpreadSort          | 1024 | Sorted             |       516.4 ns |      7.59 ns |      3.97 ns |   0.18 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **1024** | **Reversed**           |     **4,310.4 ns** |      **8.31 ns** |      **3.69 ns** |   **1.47** |    **0.00** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | Reversed           |     2,928.4 ns |     15.91 ns |      5.68 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | Reversed           |     4,114.4 ns |      7.24 ns |      3.22 ns |   1.40 |    0.00 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | Reversed           |     2,175.4 ns |     20.39 ns |      9.05 ns |   0.74 |    0.00 |    1 |         - |          NA |
| BucketSort          | 1024 | Reversed           |     5,283.8 ns |     13.79 ns |      6.12 ns |   1.80 |    0.00 |    4 |         - |          NA |
| BucketSortInteger   | 1024 | Reversed           |     3,861.8 ns |     28.40 ns |     12.61 ns |   1.32 |    0.00 |    3 |         - |          NA |
| FlashSort           | 1024 | Reversed           |    10,578.2 ns |    273.89 ns |    143.25 ns |   3.61 |    0.05 |    6 |         - |          NA |
| RadixLSD4Sort       | 1024 | Reversed           |    12,064.1 ns |    354.32 ns |    185.32 ns |   4.12 |    0.06 |    6 |         - |          NA |
| RotateLSDRadixSort  | 1024 | Reversed           |   177,944.6 ns |    499.76 ns |    221.90 ns |  60.76 |    0.13 |    8 |         - |          NA |
| RadixLSD256Sort     | 1024 | Reversed           |     5,203.1 ns |     90.09 ns |     47.12 ns |   1.78 |    0.02 |    4 |         - |          NA |
| Radsort             | 1024 | Reversed           |    18,599.5 ns |    228.42 ns |    119.47 ns |   6.35 |    0.04 |    7 |         - |          NA |
| RadixLSD10Sort      | 1024 | Reversed           |    15,690.4 ns |     89.02 ns |     39.53 ns |   5.36 |    0.02 |    7 |         - |          NA |
| RadixMSD4Sort       | 1024 | Reversed           |    23,845.9 ns |    161.48 ns |     84.46 ns |   8.14 |    0.03 |    7 |         - |          NA |
| RadixMSD10Sort      | 1024 | Reversed           |    22,123.3 ns |    137.72 ns |     61.15 ns |   7.55 |    0.02 |    7 |         - |          NA |
| AmericanFlagSort    | 1024 | Reversed           |     8,429.3 ns |    329.31 ns |    172.24 ns |   2.88 |    0.06 |    5 |         - |          NA |
| SkaSort             | 1024 | Reversed           |     8,053.6 ns |    604.41 ns |    316.12 ns |   2.75 |    0.10 |    5 |         - |          NA |
| SpreadSort          | 1024 | Reversed           |     3,723.9 ns |    178.82 ns |     79.40 ns |   1.27 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **1024** | **PipeOrgan**          |     **4,438.2 ns** |     **21.05 ns** |     **11.01 ns** |   **1.42** |    **0.03** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | PipeOrgan          |     3,137.2 ns |    145.55 ns |     64.62 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 1024 | PipeOrgan          |     3,916.9 ns |      9.35 ns |      4.89 ns |   1.25 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | PipeOrgan          |     2,444.4 ns |     54.67 ns |     19.50 ns |   0.78 |    0.02 |    1 |         - |          NA |
| BucketSort          | 1024 | PipeOrgan          |     4,976.8 ns |      7.67 ns |      3.40 ns |   1.59 |    0.03 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | PipeOrgan          |     4,120.6 ns |     22.83 ns |     10.14 ns |   1.31 |    0.02 |    3 |         - |          NA |
| FlashSort           | 1024 | PipeOrgan          |    13,327.9 ns |    278.04 ns |    123.45 ns |   4.25 |    0.09 |    4 |         - |          NA |
| RadixLSD4Sort       | 1024 | PipeOrgan          |    10,739.7 ns |    210.37 ns |    110.03 ns |   3.42 |    0.07 |    4 |         - |          NA |
| RotateLSDRadixSort  | 1024 | PipeOrgan          |   176,117.5 ns |  1,140.97 ns |    506.60 ns |  56.16 |    1.06 |    6 |         - |          NA |
| RadixLSD256Sort     | 1024 | PipeOrgan          |     5,145.1 ns |     72.25 ns |     32.08 ns |   1.64 |    0.03 |    3 |         - |          NA |
| Radsort             | 1024 | PipeOrgan          |    19,151.1 ns |     69.01 ns |     36.09 ns |   6.11 |    0.11 |    5 |         - |          NA |
| RadixLSD10Sort      | 1024 | PipeOrgan          |    15,919.2 ns |    108.74 ns |     56.87 ns |   5.08 |    0.10 |    4 |         - |          NA |
| RadixMSD4Sort       | 1024 | PipeOrgan          |    24,172.8 ns |    159.96 ns |     83.66 ns |   7.71 |    0.15 |    5 |         - |          NA |
| RadixMSD10Sort      | 1024 | PipeOrgan          |    22,582.7 ns |    329.11 ns |    146.13 ns |   7.20 |    0.14 |    5 |         - |          NA |
| AmericanFlagSort    | 1024 | PipeOrgan          |    11,659.2 ns |    299.28 ns |    156.53 ns |   3.72 |    0.08 |    4 |         - |          NA |
| SkaSort             | 1024 | PipeOrgan          |    10,897.8 ns |    387.20 ns |    202.51 ns |   3.47 |    0.09 |    4 |         - |          NA |
| SpreadSort          | 1024 | PipeOrgan          |     4,797.8 ns |     17.27 ns |      7.67 ns |   1.53 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **1024** | **ManyDuplicates**     |     **4,259.8 ns** |     **31.14 ns** |     **16.29 ns** |   **1.51** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 1024 | ManyDuplicates     |     2,827.9 ns |     47.15 ns |     16.81 ns |   1.00 |    0.00 |    1 |         - |          NA |
| PigeonSort          | 1024 | ManyDuplicates     |     5,005.9 ns |    240.70 ns |    125.89 ns |   1.77 |    0.04 |    3 |         - |          NA |
| PigeonSortInteger   | 1024 | ManyDuplicates     |     2,391.6 ns |     36.20 ns |     16.07 ns |   0.85 |    0.01 |    1 |         - |          NA |
| BucketSort          | 1024 | ManyDuplicates     |     7,733.5 ns |    301.82 ns |    134.01 ns |   2.73 |    0.05 |    3 |         - |          NA |
| BucketSortInteger   | 1024 | ManyDuplicates     |     4,820.8 ns |     50.11 ns |     22.25 ns |   1.70 |    0.01 |    3 |         - |          NA |
| FlashSort           | 1024 | ManyDuplicates     |    13,587.3 ns |     86.92 ns |     38.59 ns |   4.80 |    0.03 |    4 |         - |          NA |
| RadixLSD4Sort       | 1024 | ManyDuplicates     |     6,929.5 ns |    281.47 ns |    147.22 ns |   2.45 |    0.05 |    3 |         - |          NA |
| RotateLSDRadixSort  | 1024 | ManyDuplicates     |   190,980.8 ns |  1,789.49 ns |    794.55 ns |  67.54 |    0.45 |    6 |         - |          NA |
| RadixLSD256Sort     | 1024 | ManyDuplicates     |     3,476.3 ns |    230.84 ns |    120.73 ns |   1.23 |    0.04 |    2 |         - |          NA |
| Radsort             | 1024 | ManyDuplicates     |    20,359.6 ns |    125.73 ns |     65.76 ns |   7.20 |    0.05 |    5 |         - |          NA |
| RadixLSD10Sort      | 1024 | ManyDuplicates     |     8,737.8 ns |    229.07 ns |    119.81 ns |   3.09 |    0.04 |    3 |         - |          NA |
| RadixMSD4Sort       | 1024 | ManyDuplicates     |     7,883.2 ns |    259.18 ns |    135.56 ns |   2.79 |    0.05 |    3 |         - |          NA |
| RadixMSD10Sort      | 1024 | ManyDuplicates     |     9,722.7 ns |    253.02 ns |    132.33 ns |   3.44 |    0.05 |    3 |         - |          NA |
| AmericanFlagSort    | 1024 | ManyDuplicates     |     7,365.2 ns |    327.59 ns |    171.34 ns |   2.60 |    0.06 |    3 |         - |          NA |
| SkaSort             | 1024 | ManyDuplicates     |     6,408.0 ns |    434.51 ns |    227.26 ns |   2.27 |    0.08 |    3 |         - |          NA |
| SpreadSort          | 1024 | ManyDuplicates     |     5,488.5 ns |    211.39 ns |    110.56 ns |   1.94 |    0.04 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **4096** | **Random**             |    **19,349.0 ns** |    **255.05 ns** |    **113.24 ns** |   **1.45** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Random             |    13,364.0 ns |    252.90 ns |    112.29 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | Random             |    15,485.3 ns |    152.67 ns |     79.85 ns |   1.16 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 4096 | Random             |     9,939.7 ns |    338.27 ns |    176.92 ns |   0.74 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | Random             |    21,370.2 ns |    196.68 ns |     87.33 ns |   1.60 |    0.01 |    2 |         - |          NA |
| BucketSortInteger   | 4096 | Random             |    17,998.2 ns |    244.26 ns |    127.75 ns |   1.35 |    0.01 |    2 |         - |          NA |
| FlashSort           | 4096 | Random             |    61,294.9 ns |    109.64 ns |     57.34 ns |   4.59 |    0.04 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | Random             |    48,400.3 ns |    177.22 ns |     92.69 ns |   3.62 |    0.03 |    4 |         - |          NA |
| RotateLSDRadixSort  | 4096 | Random             | 1,410,396.0 ns | 26,372.16 ns | 11,709.40 ns | 105.54 |    1.17 |    7 |         - |          NA |
| RadixLSD256Sort     | 4096 | Random             |    19,559.3 ns |     74.26 ns |     38.84 ns |   1.46 |    0.01 |    2 |         - |          NA |
| Radsort             | 4096 | Random             |    68,436.2 ns |    357.01 ns |    186.72 ns |   5.12 |    0.04 |    5 |         - |          NA |
| RadixLSD10Sort      | 4096 | Random             |    64,537.4 ns |    465.66 ns |    206.76 ns |   4.83 |    0.04 |    5 |         - |          NA |
| RadixMSD4Sort       | 4096 | Random             |    97,477.3 ns |    557.50 ns |    198.81 ns |   7.29 |    0.06 |    6 |         - |          NA |
| RadixMSD10Sort      | 4096 | Random             |    89,162.3 ns |    562.69 ns |    294.30 ns |   6.67 |    0.06 |    6 |         - |          NA |
| AmericanFlagSort    | 4096 | Random             |    61,389.0 ns |    376.35 ns |    196.84 ns |   4.59 |    0.04 |    5 |         - |          NA |
| SkaSort             | 4096 | Random             |    46,628.2 ns |    729.10 ns |    323.72 ns |   3.49 |    0.04 |    4 |         - |          NA |
| SpreadSort          | 4096 | Random             |    28,236.3 ns |    349.66 ns |    182.88 ns |   2.11 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **4096** | **SingleElementMoved** |    **18,673.4 ns** |    **300.51 ns** |    **133.43 ns** |   **1.57** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | SingleElementMoved |    11,917.4 ns |    152.18 ns |     79.59 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | SingleElementMoved |    15,235.5 ns |    259.61 ns |    115.27 ns |   1.28 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | SingleElementMoved |     9,845.6 ns |    316.97 ns |    165.78 ns |   0.83 |    0.01 |    1 |         - |          NA |
| BucketSort          | 4096 | SingleElementMoved |    20,946.6 ns |    333.73 ns |    174.55 ns |   1.76 |    0.02 |    3 |         - |          NA |
| BucketSortInteger   | 4096 | SingleElementMoved |    16,629.8 ns |     78.49 ns |     34.85 ns |   1.40 |    0.01 |    3 |         - |          NA |
| FlashSort           | 4096 | SingleElementMoved |    46,950.5 ns |    109.99 ns |     57.53 ns |   3.94 |    0.03 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | SingleElementMoved |    72,722.9 ns |    733.56 ns |    383.67 ns |   6.10 |    0.05 |    6 |         - |          NA |
| RotateLSDRadixSort  | 4096 | SingleElementMoved |   857,543.3 ns |  1,761.14 ns |    781.96 ns |  71.96 |    0.46 |    8 |         - |          NA |
| RadixLSD256Sort     | 4096 | SingleElementMoved |    19,027.2 ns |     66.50 ns |     29.53 ns |   1.60 |    0.01 |    3 |         - |          NA |
| Radsort             | 4096 | SingleElementMoved |    44,231.3 ns |    210.81 ns |    110.26 ns |   3.71 |    0.03 |    5 |         - |          NA |
| RadixLSD10Sort      | 4096 | SingleElementMoved |    63,744.5 ns |    603.62 ns |    315.70 ns |   5.35 |    0.04 |    6 |         - |          NA |
| RadixMSD4Sort       | 4096 | SingleElementMoved |   104,146.4 ns |    529.98 ns |    235.31 ns |   8.74 |    0.06 |    7 |         - |          NA |
| RadixMSD10Sort      | 4096 | SingleElementMoved |    87,783.6 ns |    847.28 ns |    443.14 ns |   7.37 |    0.06 |    7 |         - |          NA |
| AmericanFlagSort    | 4096 | SingleElementMoved |    33,819.2 ns |    343.44 ns |    179.63 ns |   2.84 |    0.02 |    4 |         - |          NA |
| SkaSort             | 4096 | SingleElementMoved |    29,255.7 ns |  1,080.75 ns |    479.86 ns |   2.45 |    0.04 |    4 |         - |          NA |
| SpreadSort          | 4096 | SingleElementMoved |    17,993.2 ns |    217.93 ns |    113.98 ns |   1.51 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **4096** | **Sorted**             |    **18,101.9 ns** |    **853.88 ns** |    **446.59 ns** |   **1.54** |    **0.04** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Sorted             |    11,786.5 ns |    399.55 ns |    142.48 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 4096 | Sorted             |    15,290.3 ns |     92.46 ns |     41.05 ns |   1.30 |    0.02 |    4 |         - |          NA |
| PigeonSortInteger   | 4096 | Sorted             |     8,617.1 ns |    243.31 ns |    127.26 ns |   0.73 |    0.01 |    2 |         - |          NA |
| BucketSort          | 4096 | Sorted             |    20,239.9 ns |    442.27 ns |    231.32 ns |   1.72 |    0.03 |    4 |         - |          NA |
| BucketSortInteger   | 4096 | Sorted             |    15,416.0 ns |    158.78 ns |     70.50 ns |   1.31 |    0.02 |    4 |         - |          NA |
| FlashSort           | 4096 | Sorted             |    44,476.5 ns |    171.75 ns |     89.83 ns |   3.77 |    0.04 |    6 |         - |          NA |
| RadixLSD4Sort       | 4096 | Sorted             |    63,924.8 ns |  1,381.69 ns |    722.65 ns |   5.42 |    0.08 |    7 |         - |          NA |
| RotateLSDRadixSort  | 4096 | Sorted             |   851,604.3 ns |  1,475.18 ns |    771.55 ns |  72.26 |    0.82 |    9 |         - |          NA |
| RadixLSD256Sort     | 4096 | Sorted             |    17,859.5 ns |    232.85 ns |    121.78 ns |   1.52 |    0.02 |    4 |         - |          NA |
| Radsort             | 4096 | Sorted             |    41,811.6 ns |    406.30 ns |    212.50 ns |   3.55 |    0.04 |    6 |         - |          NA |
| RadixLSD10Sort      | 4096 | Sorted             |    63,354.1 ns |    300.76 ns |    133.54 ns |   5.38 |    0.06 |    7 |         - |          NA |
| RadixMSD4Sort       | 4096 | Sorted             |    99,703.9 ns |    506.42 ns |    224.85 ns |   8.46 |    0.10 |    8 |         - |          NA |
| RadixMSD10Sort      | 4096 | Sorted             |    88,177.9 ns |    241.65 ns |    126.39 ns |   7.48 |    0.09 |    8 |         - |          NA |
| AmericanFlagSort    | 4096 | Sorted             |    28,051.5 ns |    407.82 ns |    213.30 ns |   2.38 |    0.03 |    5 |         - |          NA |
| SkaSort             | 4096 | Sorted             |    25,835.1 ns |    377.90 ns |    197.65 ns |   2.19 |    0.03 |    5 |         - |          NA |
| SpreadSort          | 4096 | Sorted             |     1,767.0 ns |      6.51 ns |      2.32 ns |   0.15 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **4096** | **Reversed**           |    **17,343.6 ns** |    **203.23 ns** |    **106.29 ns** |   **1.47** |    **0.03** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | Reversed           |    11,834.3 ns |    547.12 ns |    242.92 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | Reversed           |    16,416.1 ns |    223.47 ns |    116.88 ns |   1.39 |    0.03 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | Reversed           |     8,721.2 ns |    306.99 ns |    160.56 ns |   0.74 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | Reversed           |    21,102.4 ns |    192.89 ns |    100.88 ns |   1.78 |    0.03 |    3 |         - |          NA |
| BucketSortInteger   | 4096 | Reversed           |    15,825.7 ns |     85.77 ns |     44.86 ns |   1.34 |    0.03 |    3 |         - |          NA |
| FlashSort           | 4096 | Reversed           |    42,872.5 ns |    229.00 ns |    119.77 ns |   3.62 |    0.07 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | Reversed           |    67,459.6 ns |    836.05 ns |    437.27 ns |   5.70 |    0.11 |    6 |         - |          NA |
| RotateLSDRadixSort  | 4096 | Reversed           |   869,310.0 ns |  2,765.93 ns |  1,446.64 ns |  73.48 |    1.40 |    9 |         - |          NA |
| RadixLSD256Sort     | 4096 | Reversed           |    17,994.4 ns |    222.92 ns |    116.59 ns |   1.52 |    0.03 |    3 |         - |          NA |
| Radsort             | 4096 | Reversed           |    41,969.7 ns |    151.18 ns |     67.13 ns |   3.55 |    0.07 |    5 |         - |          NA |
| RadixLSD10Sort      | 4096 | Reversed           |    63,420.0 ns |    160.99 ns |     71.48 ns |   5.36 |    0.10 |    6 |         - |          NA |
| RadixMSD4Sort       | 4096 | Reversed           |   102,196.5 ns |    411.74 ns |    146.83 ns |   8.64 |    0.16 |    7 |         - |          NA |
| RadixMSD10Sort      | 4096 | Reversed           |   130,035.4 ns |  2,042.28 ns |  1,068.15 ns |  10.99 |    0.23 |    8 |         - |          NA |
| AmericanFlagSort    | 4096 | Reversed           |    31,221.1 ns |    369.87 ns |    193.45 ns |   2.64 |    0.05 |    4 |         - |          NA |
| SkaSort             | 4096 | Reversed           |    29,198.7 ns |    267.86 ns |    118.93 ns |   2.47 |    0.05 |    4 |         - |          NA |
| SpreadSort          | 4096 | Reversed           |    14,878.3 ns |    150.48 ns |     78.70 ns |   1.26 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **4096** | **PipeOrgan**          |    **18,806.0 ns** |    **144.88 ns** |     **64.33 ns** |   **1.52** |    **0.02** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 4096 | PipeOrgan          |    12,344.2 ns |    316.39 ns |    165.48 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | PipeOrgan          |    15,327.5 ns |     56.82 ns |     29.72 ns |   1.24 |    0.02 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | PipeOrgan          |     9,854.8 ns |    291.12 ns |    152.26 ns |   0.80 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | PipeOrgan          |    20,773.8 ns |    301.33 ns |    157.60 ns |   1.68 |    0.02 |    4 |         - |          NA |
| BucketSortInteger   | 4096 | PipeOrgan          |    18,706.9 ns |    224.54 ns |    117.44 ns |   1.52 |    0.02 |    4 |         - |          NA |
| FlashSort           | 4096 | PipeOrgan          |    51,657.2 ns |    485.55 ns |    253.95 ns |   4.19 |    0.06 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | PipeOrgan          |    53,083.0 ns |    517.46 ns |    270.64 ns |   4.30 |    0.06 |    5 |         - |          NA |
| RotateLSDRadixSort  | 4096 | PipeOrgan          |   856,289.5 ns |  1,754.63 ns |    917.71 ns |  69.38 |    0.88 |    8 |         - |          NA |
| RadixLSD256Sort     | 4096 | PipeOrgan          |    19,536.8 ns |    171.49 ns |     76.14 ns |   1.58 |    0.02 |    4 |         - |          NA |
| Radsort             | 4096 | PipeOrgan          |    42,774.0 ns |    286.68 ns |    127.29 ns |   3.47 |    0.05 |    5 |         - |          NA |
| RadixLSD10Sort      | 4096 | PipeOrgan          |    64,049.4 ns |    575.94 ns |    301.23 ns |   5.19 |    0.07 |    6 |         - |          NA |
| RadixMSD4Sort       | 4096 | PipeOrgan          |   104,043.3 ns |    545.32 ns |    242.13 ns |   8.43 |    0.11 |    7 |         - |          NA |
| RadixMSD10Sort      | 4096 | PipeOrgan          |    89,203.8 ns |    691.84 ns |    307.18 ns |   7.23 |    0.09 |    7 |         - |          NA |
| AmericanFlagSort    | 4096 | PipeOrgan          |    48,455.5 ns |    414.58 ns |    216.83 ns |   3.93 |    0.05 |    5 |         - |          NA |
| SkaSort             | 4096 | PipeOrgan          |    42,664.0 ns |    974.78 ns |    509.83 ns |   3.46 |    0.06 |    5 |         - |          NA |
| SpreadSort          | 4096 | PipeOrgan          |    20,596.9 ns |    259.36 ns |    135.65 ns |   1.67 |    0.02 |    4 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **4096** | **ManyDuplicates**     |    **16,889.4 ns** |    **170.95 ns** |     **89.41 ns** |   **1.52** |    **0.02** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 4096 | ManyDuplicates     |    11,111.0 ns |    294.17 ns |    153.86 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 4096 | ManyDuplicates     |    19,902.3 ns |    505.62 ns |    224.50 ns |   1.79 |    0.03 |    3 |         - |          NA |
| PigeonSortInteger   | 4096 | ManyDuplicates     |     8,737.6 ns |    253.75 ns |    132.72 ns |   0.79 |    0.02 |    1 |         - |          NA |
| BucketSort          | 4096 | ManyDuplicates     |    30,300.9 ns |    162.32 ns |     72.07 ns |   2.73 |    0.04 |    4 |         - |          NA |
| BucketSortInteger   | 4096 | ManyDuplicates     |    16,541.9 ns |    114.75 ns |     60.01 ns |   1.49 |    0.02 |    3 |         - |          NA |
| FlashSort           | 4096 | ManyDuplicates     |    53,607.2 ns |    540.73 ns |    282.81 ns |   4.83 |    0.07 |    5 |         - |          NA |
| RadixLSD4Sort       | 4096 | ManyDuplicates     |    27,037.8 ns |    378.45 ns |    168.03 ns |   2.43 |    0.04 |    4 |         - |          NA |
| RotateLSDRadixSort  | 4096 | ManyDuplicates     |   770,771.7 ns |  1,864.55 ns |    975.20 ns |  69.38 |    0.92 |    6 |         - |          NA |
| RadixLSD256Sort     | 4096 | ManyDuplicates     |    12,568.0 ns |    260.30 ns |    136.14 ns |   1.13 |    0.02 |    2 |         - |          NA |
| Radsort             | 4096 | ManyDuplicates     |    42,544.2 ns |    306.96 ns |    160.55 ns |   3.83 |    0.05 |    4 |         - |          NA |
| RadixLSD10Sort      | 4096 | ManyDuplicates     |    34,436.0 ns |    224.22 ns |     99.56 ns |   3.10 |    0.04 |    4 |         - |          NA |
| RadixMSD4Sort       | 4096 | ManyDuplicates     |    28,277.7 ns |    475.59 ns |    211.17 ns |   2.55 |    0.04 |    4 |         - |          NA |
| RadixMSD10Sort      | 4096 | ManyDuplicates     |    36,490.0 ns |    205.86 ns |     91.40 ns |   3.28 |    0.04 |    4 |         - |          NA |
| AmericanFlagSort    | 4096 | ManyDuplicates     |    26,640.3 ns |    368.79 ns |    192.88 ns |   2.40 |    0.04 |    4 |         - |          NA |
| SkaSort             | 4096 | ManyDuplicates     |    18,268.4 ns |    244.87 ns |    108.72 ns |   1.64 |    0.02 |    3 |         - |          NA |
| SpreadSort          | 4096 | ManyDuplicates     |    20,972.4 ns |    227.64 ns |     81.18 ns |   1.89 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **8192** | **Random**             |    **39,244.7 ns** |    **297.60 ns** |    **155.65 ns** |   **1.41** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Random             |    27,805.2 ns |    320.32 ns |    142.22 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | Random             |    30,849.2 ns |    275.92 ns |    122.51 ns |   1.11 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | Random             |    19,917.5 ns |    230.18 ns |    102.20 ns |   0.72 |    0.00 |    1 |         - |          NA |
| BucketSort          | 8192 | Random             |    43,925.6 ns |    398.17 ns |    176.79 ns |   1.58 |    0.01 |    2 |         - |          NA |
| BucketSortInteger   | 8192 | Random             |    36,621.4 ns |    132.95 ns |     59.03 ns |   1.32 |    0.01 |    2 |         - |          NA |
| FlashSort           | 8192 | Random             |   132,775.8 ns |    625.22 ns |    327.00 ns |   4.78 |    0.03 |    3 |         - |          NA |
| RadixLSD4Sort       | 8192 | Random             |   115,433.4 ns |  1,218.74 ns |    637.42 ns |   4.15 |    0.03 |    3 |         - |          NA |
| RotateLSDRadixSort  | 8192 | Random             | 3,411,303.1 ns | 65,102.31 ns | 34,049.77 ns | 122.69 |    1.30 |    5 |         - |          NA |
| RadixLSD256Sort     | 8192 | Random             |    39,430.1 ns |    451.86 ns |    200.63 ns |   1.42 |    0.01 |    2 |         - |          NA |
| Radsort             | 8192 | Random             |    94,994.0 ns |  1,361.25 ns |    711.96 ns |   3.42 |    0.03 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | Random             |   128,986.6 ns |    594.05 ns |    263.76 ns |   4.64 |    0.02 |    3 |         - |          NA |
| RadixMSD4Sort       | 8192 | Random             |   213,857.6 ns |  3,650.35 ns |  1,620.78 ns |   7.69 |    0.07 |    4 |         - |          NA |
| RadixMSD10Sort      | 8192 | Random             |   179,516.5 ns |    506.18 ns |    224.75 ns |   6.46 |    0.03 |    4 |         - |          NA |
| AmericanFlagSort    | 8192 | Random             |   121,716.2 ns |    392.59 ns |    174.31 ns |   4.38 |    0.02 |    3 |         - |          NA |
| SkaSort             | 8192 | Random             |    91,628.5 ns |  1,107.23 ns |    491.62 ns |   3.30 |    0.02 |    3 |         - |          NA |
| SpreadSort          | 8192 | Random             |    98,709.0 ns |    999.98 ns |    444.00 ns |   3.55 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **8192** | **SingleElementMoved** |    **37,194.2 ns** |    **219.14 ns** |    **114.61 ns** |   **1.55** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | SingleElementMoved |    23,952.5 ns |    335.17 ns |    148.82 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | SingleElementMoved |    30,150.3 ns |    316.72 ns |    140.63 ns |   1.26 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | SingleElementMoved |    19,476.0 ns |    275.05 ns |    122.12 ns |   0.81 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | SingleElementMoved |    46,826.8 ns |    217.90 ns |    113.96 ns |   1.96 |    0.01 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | SingleElementMoved |    33,244.9 ns |    424.77 ns |    188.60 ns |   1.39 |    0.01 |    3 |         - |          NA |
| FlashSort           | 8192 | SingleElementMoved |    95,443.8 ns |  1,626.62 ns |    722.23 ns |   3.98 |    0.04 |    5 |         - |          NA |
| RadixLSD4Sort       | 8192 | SingleElementMoved |   157,955.0 ns |  1,542.79 ns |    806.91 ns |   6.59 |    0.05 |    7 |         - |          NA |
| RotateLSDRadixSort  | 8192 | SingleElementMoved | 2,174,256.5 ns |  7,653.48 ns |  3,398.19 ns |  90.78 |    0.54 |    9 |         - |          NA |
| RadixLSD256Sort     | 8192 | SingleElementMoved |    40,215.1 ns |    182.46 ns |     95.43 ns |   1.68 |    0.01 |    3 |         - |          NA |
| Radsort             | 8192 | SingleElementMoved |    60,875.9 ns |    644.55 ns |    337.11 ns |   2.54 |    0.02 |    4 |         - |          NA |
| RadixLSD10Sort      | 8192 | SingleElementMoved |   127,642.6 ns |  1,013.62 ns |    530.14 ns |   5.33 |    0.04 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | SingleElementMoved |   254,024.2 ns |  2,487.63 ns |  1,301.08 ns |  10.61 |    0.08 |    8 |         - |          NA |
| RadixMSD10Sort      | 8192 | SingleElementMoved |   179,254.1 ns |  1,414.59 ns |    628.09 ns |   7.48 |    0.05 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | SingleElementMoved |    67,009.1 ns |    750.49 ns |    333.22 ns |   2.80 |    0.02 |    4 |         - |          NA |
| SkaSort             | 8192 | SingleElementMoved |    56,762.6 ns |    330.25 ns |    146.63 ns |   2.37 |    0.01 |    4 |         - |          NA |
| SpreadSort          | 8192 | SingleElementMoved |    39,901.0 ns |    580.23 ns |    303.47 ns |   1.67 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **8192** | **Sorted**             |    **37,044.4 ns** |    **621.26 ns** |    **275.84 ns** |   **1.58** |    **0.01** |    **4** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Sorted             |    23,513.3 ns |    154.88 ns |     68.77 ns |   1.00 |    0.00 |    3 |         - |          NA |
| PigeonSort          | 8192 | Sorted             |    30,091.1 ns |    231.50 ns |    121.08 ns |   1.28 |    0.01 |    4 |         - |          NA |
| PigeonSortInteger   | 8192 | Sorted             |    17,219.7 ns |    179.48 ns |     79.69 ns |   0.73 |    0.00 |    2 |         - |          NA |
| BucketSort          | 8192 | Sorted             |    41,553.5 ns |    554.99 ns |    246.42 ns |   1.77 |    0.01 |    4 |         - |          NA |
| BucketSortInteger   | 8192 | Sorted             |    31,451.6 ns |    297.86 ns |    155.78 ns |   1.34 |    0.01 |    4 |         - |          NA |
| FlashSort           | 8192 | Sorted             |    89,636.2 ns |  1,282.70 ns |    569.53 ns |   3.81 |    0.02 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | Sorted             |   160,156.1 ns |  1,099.12 ns |    574.86 ns |   6.81 |    0.03 |    8 |         - |          NA |
| RotateLSDRadixSort  | 8192 | Sorted             | 2,164,636.1 ns |  1,653.17 ns |    734.02 ns |  92.06 |    0.25 |    9 |         - |          NA |
| RadixLSD256Sort     | 8192 | Sorted             |    37,713.0 ns |    502.07 ns |    222.92 ns |   1.60 |    0.01 |    4 |         - |          NA |
| Radsort             | 8192 | Sorted             |    56,795.0 ns |    308.04 ns |    136.77 ns |   2.42 |    0.01 |    5 |         - |          NA |
| RadixLSD10Sort      | 8192 | Sorted             |   125,549.8 ns |    378.61 ns |    168.10 ns |   5.34 |    0.02 |    7 |         - |          NA |
| RadixMSD4Sort       | 8192 | Sorted             |   212,150.9 ns |    772.85 ns |    275.61 ns |   9.02 |    0.03 |    8 |         - |          NA |
| RadixMSD10Sort      | 8192 | Sorted             |   178,010.4 ns |  5,324.97 ns |  2,364.32 ns |   7.57 |    0.10 |    8 |         - |          NA |
| AmericanFlagSort    | 8192 | Sorted             |    56,879.3 ns |  3,075.32 ns |  1,365.46 ns |   2.42 |    0.05 |    5 |         - |          NA |
| SkaSort             | 8192 | Sorted             |    50,545.6 ns |    412.28 ns |    183.05 ns |   2.15 |    0.01 |    5 |         - |          NA |
| SpreadSort          | 8192 | Sorted             |     3,486.3 ns |     21.41 ns |     11.20 ns |   0.15 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **8192** | **Reversed**           |    **34,530.7 ns** |    **193.73 ns** |     **86.02 ns** |   **1.46** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | Reversed           |    23,600.1 ns |    276.78 ns |    122.89 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | Reversed           |    33,154.1 ns |    319.77 ns |    167.24 ns |   1.40 |    0.01 |    3 |         - |          NA |
| PigeonSortInteger   | 8192 | Reversed           |    17,373.7 ns |    253.75 ns |    132.72 ns |   0.74 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | Reversed           |    41,920.0 ns |    344.50 ns |    180.18 ns |   1.78 |    0.01 |    3 |         - |          NA |
| BucketSortInteger   | 8192 | Reversed           |    31,653.8 ns |    501.84 ns |    222.82 ns |   1.34 |    0.01 |    3 |         - |          NA |
| FlashSort           | 8192 | Reversed           |    83,452.9 ns |    456.36 ns |    162.74 ns |   3.54 |    0.02 |    5 |         - |          NA |
| RadixLSD4Sort       | 8192 | Reversed           |   165,895.2 ns |  1,048.90 ns |    548.59 ns |   7.03 |    0.04 |    7 |         - |          NA |
| RotateLSDRadixSort  | 8192 | Reversed           | 2,205,377.8 ns |  1,891.60 ns |    839.88 ns |  93.45 |    0.45 |    9 |         - |          NA |
| RadixLSD256Sort     | 8192 | Reversed           |    37,347.3 ns |    480.04 ns |    251.07 ns |   1.58 |    0.01 |    3 |         - |          NA |
| Radsort             | 8192 | Reversed           |    56,592.7 ns |    275.64 ns |    122.38 ns |   2.40 |    0.01 |    4 |         - |          NA |
| RadixLSD10Sort      | 8192 | Reversed           |   125,295.0 ns |    429.60 ns |    190.75 ns |   5.31 |    0.03 |    6 |         - |          NA |
| RadixMSD4Sort       | 8192 | Reversed           |   219,959.4 ns |  1,740.19 ns |    910.15 ns |   9.32 |    0.06 |    8 |         - |          NA |
| RadixMSD10Sort      | 8192 | Reversed           |   175,393.2 ns |    694.12 ns |    247.53 ns |   7.43 |    0.04 |    7 |         - |          NA |
| AmericanFlagSort    | 8192 | Reversed           |    61,872.4 ns |    849.11 ns |    444.10 ns |   2.62 |    0.02 |    4 |         - |          NA |
| SkaSort             | 8192 | Reversed           |    57,521.9 ns |    534.44 ns |    237.30 ns |   2.44 |    0.02 |    4 |         - |          NA |
| SpreadSort          | 8192 | Reversed           |    56,019.0 ns |  2,357.67 ns |    840.77 ns |   2.37 |    0.03 |    4 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **8192** | **PipeOrgan**          |    **37,710.6 ns** |    **209.33 ns** |    **109.49 ns** |   **1.42** |    **0.01** |    **2** |         **-** |          **NA** |
| CountingSortInteger | 8192 | PipeOrgan          |    26,496.9 ns |    300.67 ns |    157.26 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | PipeOrgan          |    30,755.4 ns |    241.77 ns |    126.45 ns |   1.16 |    0.01 |    2 |         - |          NA |
| PigeonSortInteger   | 8192 | PipeOrgan          |    19,626.2 ns |    255.36 ns |    133.56 ns |   0.74 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | PipeOrgan          |    40,592.6 ns |    671.98 ns |    351.46 ns |   1.53 |    0.02 |    2 |         - |          NA |
| BucketSortInteger   | 8192 | PipeOrgan          |    35,648.1 ns |    349.83 ns |    182.97 ns |   1.35 |    0.01 |    2 |         - |          NA |
| FlashSort           | 8192 | PipeOrgan          |   114,631.2 ns |  1,115.94 ns |    495.48 ns |   4.33 |    0.03 |    4 |         - |          NA |
| RadixLSD4Sort       | 8192 | PipeOrgan          |   129,505.5 ns |  1,308.97 ns |    684.62 ns |   4.89 |    0.04 |    4 |         - |          NA |
| RotateLSDRadixSort  | 8192 | PipeOrgan          | 2,174,031.8 ns |  3,918.82 ns |  1,739.98 ns |  82.05 |    0.46 |    7 |         - |          NA |
| RadixLSD256Sort     | 8192 | PipeOrgan          |    40,436.7 ns |    340.27 ns |    151.08 ns |   1.53 |    0.01 |    2 |         - |          NA |
| Radsort             | 8192 | PipeOrgan          |    59,333.8 ns |    368.57 ns |    163.65 ns |   2.24 |    0.01 |    3 |         - |          NA |
| RadixLSD10Sort      | 8192 | PipeOrgan          |   127,476.7 ns |  1,010.08 ns |    528.29 ns |   4.81 |    0.03 |    4 |         - |          NA |
| RadixMSD4Sort       | 8192 | PipeOrgan          |   219,337.3 ns |  1,801.64 ns |    799.94 ns |   8.28 |    0.05 |    6 |         - |          NA |
| RadixMSD10Sort      | 8192 | PipeOrgan          |   180,790.2 ns |  1,513.41 ns |    791.54 ns |   6.82 |    0.05 |    5 |         - |          NA |
| AmericanFlagSort    | 8192 | PipeOrgan          |    97,655.4 ns |  1,128.69 ns |    501.14 ns |   3.69 |    0.03 |    4 |         - |          NA |
| SkaSort             | 8192 | PipeOrgan          |    86,194.5 ns |    689.74 ns |    306.25 ns |   3.25 |    0.02 |    4 |         - |          NA |
| SpreadSort          | 8192 | PipeOrgan          |    68,162.2 ns |    392.86 ns |    205.47 ns |   2.57 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |        |         |      |           |             |
| **CountingSort**        | **8192** | **ManyDuplicates**     |    **34,209.4 ns** |    **337.93 ns** |    **176.74 ns** |   **1.55** |    **0.01** |    **3** |         **-** |          **NA** |
| CountingSortInteger | 8192 | ManyDuplicates     |    22,106.7 ns |    266.47 ns |    139.37 ns |   1.00 |    0.00 |    2 |         - |          NA |
| PigeonSort          | 8192 | ManyDuplicates     |    54,092.5 ns |    132.92 ns |     59.02 ns |   2.45 |    0.01 |    5 |         - |          NA |
| PigeonSortInteger   | 8192 | ManyDuplicates     |    17,212.5 ns |    221.92 ns |     98.53 ns |   0.78 |    0.01 |    1 |         - |          NA |
| BucketSort          | 8192 | ManyDuplicates     |    60,782.4 ns |    789.20 ns |    412.77 ns |   2.75 |    0.02 |    5 |         - |          NA |
| BucketSortInteger   | 8192 | ManyDuplicates     |    31,776.2 ns |    310.29 ns |    162.29 ns |   1.44 |    0.01 |    3 |         - |          NA |
| FlashSort           | 8192 | ManyDuplicates     |   106,674.4 ns |    554.75 ns |    246.31 ns |   4.83 |    0.03 |    6 |         - |          NA |
| RadixLSD4Sort       | 8192 | ManyDuplicates     |    54,233.3 ns |    190.50 ns |     84.58 ns |   2.45 |    0.01 |    5 |         - |          NA |
| RotateLSDRadixSort  | 8192 | ManyDuplicates     | 1,627,140.4 ns |  3,232.41 ns |  1,690.61 ns |  73.61 |    0.44 |    7 |         - |          NA |
| RadixLSD256Sort     | 8192 | ManyDuplicates     |    25,006.6 ns |    119.32 ns |     42.55 ns |   1.13 |    0.01 |    2 |         - |          NA |
| Radsort             | 8192 | ManyDuplicates     |    53,380.9 ns |    379.65 ns |    198.57 ns |   2.41 |    0.02 |    5 |         - |          NA |
| RadixLSD10Sort      | 8192 | ManyDuplicates     |    68,685.1 ns |    156.43 ns |     69.46 ns |   3.11 |    0.02 |    5 |         - |          NA |
| RadixMSD4Sort       | 8192 | ManyDuplicates     |    56,251.2 ns |    164.51 ns |     86.04 ns |   2.54 |    0.02 |    5 |         - |          NA |
| RadixMSD10Sort      | 8192 | ManyDuplicates     |    73,096.2 ns |    564.11 ns |    250.47 ns |   3.31 |    0.02 |    5 |         - |          NA |
| AmericanFlagSort    | 8192 | ManyDuplicates     |    52,535.2 ns |    338.82 ns |    177.21 ns |   2.38 |    0.02 |    5 |         - |          NA |
| SkaSort             | 8192 | ManyDuplicates     |    33,767.8 ns |    272.45 ns |    142.50 ns |   1.53 |    0.01 |    3 |         - |          NA |
| SpreadSort          | 8192 | ManyDuplicates     |    41,986.2 ns |    287.83 ns |    150.54 ns |   1.90 |    0.01 |    4 |         - |          NA |

### ExchangeBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.67GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.5 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method             | Size | Pattern            | Mean         | Error       | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------- |----- |------------------- |-------------:|------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **BubbleSort**         | **256**  | **Random**             |  **41,783.5 ns** |   **532.88 ns** |   **190.03 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Random             |  27,241.3 ns |   177.05 ns |    92.60 ns |  0.65 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 256  | Random             |  41,918.0 ns |   309.89 ns |   137.59 ns |  1.00 |    0.01 |    3 |         - |          NA |
| CombSort           | 256  | Random             |   8,066.4 ns |   229.05 ns |   119.80 ns |  0.19 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | Random             |  22,888.7 ns |   138.10 ns |    61.32 ns |  0.55 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **SingleElementMoved** |   **4,508.3 ns** |    **45.02 ns** |    **16.05 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | SingleElementMoved |     307.3 ns |    21.06 ns |    11.02 ns |  0.07 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 256  | SingleElementMoved |   8,542.6 ns |   258.57 ns |   135.23 ns |  1.89 |    0.03 |    4 |         - |          NA |
| CombSort           | 256  | SingleElementMoved |   2,690.4 ns |    81.61 ns |    36.24 ns |  0.60 |    0.01 |    2 |         - |          NA |
| CircleSort         | 256  | SingleElementMoved |  10,469.7 ns |   220.70 ns |   115.43 ns |  2.32 |    0.03 |    5 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **Sorted**             |     **217.6 ns** |     **1.20 ns** |     **0.43 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Sorted             |     179.1 ns |     2.89 ns |     1.28 ns |  0.82 |    0.01 |    1 |         - |          NA |
| OddEvenSort        | 256  | Sorted             |     178.1 ns |     2.90 ns |     1.29 ns |  0.82 |    0.01 |    1 |         - |          NA |
| CombSort           | 256  | Sorted             |   2,269.5 ns |    12.62 ns |     5.60 ns | 10.43 |    0.03 |    4 |         - |          NA |
| CircleSort         | 256  | Sorted             |   1,682.4 ns |    32.45 ns |    11.57 ns |  7.73 |    0.05 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **Reversed**           |  **22,982.5 ns** |   **566.28 ns** |   **296.17 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 256  | Reversed           |  21,698.0 ns |   150.55 ns |    66.85 ns |  0.94 |    0.01 |    3 |         - |          NA |
| OddEvenSort        | 256  | Reversed           |  20,253.1 ns |   794.20 ns |   415.38 ns |  0.88 |    0.02 |    3 |         - |          NA |
| CombSort           | 256  | Reversed           |   2,390.7 ns |    60.82 ns |    21.69 ns |  0.10 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | Reversed           |   3,302.5 ns |    25.51 ns |    11.33 ns |  0.14 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **PipeOrgan**          |  **26,957.0 ns** |   **410.57 ns** |   **182.30 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | PipeOrgan          |  13,977.2 ns |   115.60 ns |    60.46 ns |  0.52 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 256  | PipeOrgan          |  21,877.7 ns |   161.79 ns |    84.62 ns |  0.81 |    0.01 |    3 |         - |          NA |
| CombSort           | 256  | PipeOrgan          |   2,467.1 ns |   147.14 ns |    65.33 ns |  0.09 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | PipeOrgan          |  15,059.9 ns |   170.71 ns |    89.29 ns |  0.56 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **256**  | **ManyDuplicates**     |  **41,221.9 ns** |   **257.86 ns** |   **134.87 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 256  | ManyDuplicates     |  26,644.3 ns |   140.18 ns |    62.24 ns |  0.65 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 256  | ManyDuplicates     |  43,797.0 ns |   420.62 ns |   219.99 ns |  1.06 |    0.01 |    4 |         - |          NA |
| CombSort           | 256  | ManyDuplicates     |   6,118.9 ns |   195.20 ns |   102.09 ns |  0.15 |    0.00 |    1 |         - |          NA |
| CircleSort         | 256  | ManyDuplicates     |  19,840.7 ns |   175.69 ns |    78.01 ns |  0.48 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **Random**             | **533,034.4 ns** | **2,226.70 ns** | **1,164.61 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Random             | 296,373.3 ns |   283.07 ns |   100.95 ns |  0.56 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | Random             | 479,675.5 ns | 1,095.02 ns |   486.20 ns |  0.90 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | Random             |  42,446.9 ns |   266.42 ns |   118.29 ns |  0.08 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Random             | 126,960.8 ns |   592.32 ns |   262.99 ns |  0.24 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **SingleElementMoved** |  **66,603.7 ns** |   **300.63 ns** |   **133.48 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | SingleElementMoved |   1,101.4 ns |     5.26 ns |     1.87 ns |  0.02 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 1024 | SingleElementMoved | 122,365.5 ns |   811.88 ns |   360.48 ns |  1.84 |    0.01 |    4 |         - |          NA |
| CombSort           | 1024 | SingleElementMoved |  12,713.1 ns |   221.93 ns |    98.54 ns |  0.19 |    0.00 |    2 |         - |          NA |
| CircleSort         | 1024 | SingleElementMoved |  57,705.1 ns |   399.04 ns |   208.71 ns |  0.87 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **Sorted**             |     **841.8 ns** |     **1.88 ns** |     **0.98 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Sorted             |     569.0 ns |     3.19 ns |     1.14 ns |  0.68 |    0.00 |    1 |         - |          NA |
| OddEvenSort        | 1024 | Sorted             |     750.5 ns |    15.11 ns |     7.91 ns |  0.89 |    0.01 |    2 |         - |          NA |
| CombSort           | 1024 | Sorted             |  11,612.4 ns |   227.12 ns |   118.79 ns | 13.79 |    0.13 |    4 |         - |          NA |
| CircleSort         | 1024 | Sorted             |   7,316.8 ns |   286.12 ns |   149.65 ns |  8.69 |    0.17 |    3 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **Reversed**           | **339,369.2 ns** |   **288.54 ns** |   **102.90 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | Reversed           | 340,061.5 ns | 2,294.16 ns | 1,199.89 ns |  1.00 |    0.00 |    2 |         - |          NA |
| OddEvenSort        | 1024 | Reversed           | 318,004.3 ns | 3,310.98 ns | 1,731.71 ns |  0.94 |    0.00 |    2 |         - |          NA |
| CombSort           | 1024 | Reversed           |  12,765.6 ns |   261.18 ns |   136.60 ns |  0.04 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | Reversed           |  14,778.0 ns |   147.41 ns |    52.57 ns |  0.04 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **PipeOrgan**          | **394,670.1 ns** | **2,178.51 ns** |   **967.27 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | PipeOrgan          | 220,728.0 ns | 1,622.03 ns |   848.35 ns |  0.56 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | PipeOrgan          | 349,265.5 ns | 2,728.09 ns | 1,426.84 ns |  0.88 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | PipeOrgan          |  12,802.3 ns |   300.81 ns |   157.33 ns |  0.03 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | PipeOrgan          |  81,545.1 ns | 1,340.27 ns |   700.99 ns |  0.21 |    0.00 |    2 |         - |          NA |
|      |                    |              |             |             |       |         |      |           |             |
| **BubbleSort**         | **1024** | **ManyDuplicates**     | **530,282.8 ns** | **2,857.81 ns** | **1,494.69 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| CocktailShakerSort | 1024 | ManyDuplicates     | 291,101.3 ns |   719.61 ns |   319.51 ns |  0.55 |    0.00 |    3 |         - |          NA |
| OddEvenSort        | 1024 | ManyDuplicates     | 496,346.7 ns | 2,064.46 ns | 1,079.75 ns |  0.94 |    0.00 |    4 |         - |          NA |
| CombSort           | 1024 | ManyDuplicates     |  28,243.9 ns |   170.64 ns |    89.25 ns |  0.05 |    0.00 |    1 |         - |          NA |
| CircleSort         | 1024 | ManyDuplicates     |  99,648.5 ns |   880.76 ns |   460.66 ns |  0.19 |    0.00 |    2 |         - |          NA |

### HeapBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.67GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.5 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method           | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **HeapSort**         | **256**  | **Random**             |     **6,763.6 ns** |    **199.71 ns** |    **104.45 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | Random             |     6,892.6 ns |    115.81 ns |     51.42 ns |  1.02 |    0.02 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | Random             |    10,721.0 ns |    315.49 ns |    165.01 ns |  1.59 |    0.03 |    2 |         - |          NA |
| BottomupHeapSort | 256  | Random             |     7,031.9 ns |    177.34 ns |     92.75 ns |  1.04 |    0.02 |    1 |         - |          NA |
| WeakHeapSort     | 256  | Random             |    12,579.4 ns |    434.53 ns |    227.27 ns |  1.86 |    0.04 |    2 |         - |          NA |
| SmoothSort       | 256  | Random             |    13,444.2 ns |    184.11 ns |     81.75 ns |  1.99 |    0.03 |    2 |         - |          NA |
| PoplarHeapSort   | 256  | Random             |    13,611.2 ns |    258.71 ns |    114.87 ns |  2.01 |    0.03 |    2 |         - |          NA |
| TournamentSort   | 256  | Random             |    13,377.2 ns |    203.73 ns |     90.46 ns |  1.98 |    0.03 |    2 |         - |          NA |
| BinomialHeapSort | 256  | Random             |    27,305.9 ns |    633.98 ns |    281.49 ns |  4.04 |    0.07 |    3 |         - |          NA |
| PairingHeapSort  | 256  | Random             |    16,239.1 ns |    265.11 ns |    138.66 ns |  2.40 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **SingleElementMoved** |     **5,800.4 ns** |    **182.55 ns** |     **95.48 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 256  | SingleElementMoved |     5,467.0 ns |    136.69 ns |     71.49 ns |  0.94 |    0.02 |    4 |         - |          NA |
| TernaryHeapSort  | 256  | SingleElementMoved |     5,288.0 ns |  2,084.65 ns |  1,090.31 ns |  0.91 |    0.18 |    4 |         - |          NA |
| BottomupHeapSort | 256  | SingleElementMoved |     5,637.5 ns |    227.64 ns |    101.08 ns |  0.97 |    0.02 |    4 |         - |          NA |
| WeakHeapSort     | 256  | SingleElementMoved |     5,506.4 ns |    171.88 ns |     89.89 ns |  0.95 |    0.02 |    4 |         - |          NA |
| SmoothSort       | 256  | SingleElementMoved |     1,290.2 ns |    129.64 ns |     57.56 ns |  0.22 |    0.01 |    1 |         - |          NA |
| PoplarHeapSort   | 256  | SingleElementMoved |     2,981.4 ns |     59.09 ns |     26.24 ns |  0.51 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 256  | SingleElementMoved |     4,499.9 ns |    416.14 ns |    217.65 ns |  0.78 |    0.04 |    3 |         - |          NA |
| BinomialHeapSort | 256  | SingleElementMoved |     6,108.3 ns |    203.36 ns |     90.29 ns |  1.05 |    0.02 |    4 |         - |          NA |
| PairingHeapSort  | 256  | SingleElementMoved |     4,338.9 ns |    125.88 ns |     65.84 ns |  0.75 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Sorted**             |     **5,597.7 ns** |    **180.55 ns** |     **94.43 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 256  | Sorted             |     5,308.7 ns |    243.69 ns |    127.46 ns |  0.95 |    0.03 |    4 |         - |          NA |
| TernaryHeapSort  | 256  | Sorted             |     3,296.1 ns |    214.68 ns |    112.28 ns |  0.59 |    0.02 |    3 |         - |          NA |
| BottomupHeapSort | 256  | Sorted             |     5,490.6 ns |    270.20 ns |    119.97 ns |  0.98 |    0.03 |    4 |         - |          NA |
| WeakHeapSort     | 256  | Sorted             |     4,931.2 ns |     14.29 ns |      5.09 ns |  0.88 |    0.01 |    4 |         - |          NA |
| SmoothSort       | 256  | Sorted             |       933.7 ns |     14.65 ns |      6.51 ns |  0.17 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 256  | Sorted             |     2,606.4 ns |     11.24 ns |      4.01 ns |  0.47 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 256  | Sorted             |     4,269.4 ns |    294.32 ns |    153.94 ns |  0.76 |    0.03 |    4 |         - |          NA |
| BinomialHeapSort | 256  | Sorted             |     5,313.5 ns |    235.37 ns |    123.10 ns |  0.95 |    0.03 |    4 |         - |          NA |
| PairingHeapSort  | 256  | Sorted             |     4,259.7 ns |    276.56 ns |    144.65 ns |  0.76 |    0.03 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **Reversed**           |     **5,219.2 ns** |    **230.72 ns** |    **120.67 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 256  | Reversed           |     5,567.5 ns |     41.00 ns |     18.20 ns |  1.07 |    0.02 |    3 |         - |          NA |
| TernaryHeapSort  | 256  | Reversed           |     2,726.0 ns |    178.70 ns |     79.35 ns |  0.52 |    0.02 |    2 |         - |          NA |
| BottomupHeapSort | 256  | Reversed           |     5,754.8 ns |    270.81 ns |    141.64 ns |  1.10 |    0.04 |    3 |         - |          NA |
| WeakHeapSort     | 256  | Reversed           |     5,740.2 ns |    394.77 ns |    206.47 ns |  1.10 |    0.04 |    3 |         - |          NA |
| SmoothSort       | 256  | Reversed           |     3,860.8 ns |    181.41 ns |     94.88 ns |  0.74 |    0.02 |    3 |         - |          NA |
| PoplarHeapSort   | 256  | Reversed           |     4,625.9 ns |    244.50 ns |    127.88 ns |  0.89 |    0.03 |    3 |         - |          NA |
| TournamentSort   | 256  | Reversed           |     4,342.0 ns |    167.43 ns |     87.57 ns |  0.83 |    0.02 |    3 |         - |          NA |
| BinomialHeapSort | 256  | Reversed           |     5,265.5 ns |    295.79 ns |    154.70 ns |  1.01 |    0.04 |    3 |         - |          NA |
| PairingHeapSort  | 256  | Reversed           |     1,983.3 ns |      4.87 ns |      1.74 ns |  0.38 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **PipeOrgan**          |     **5,411.2 ns** |    **259.10 ns** |    **135.51 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 256  | PipeOrgan          |     5,167.7 ns |    190.62 ns |     99.70 ns |  0.96 |    0.03 |    2 |         - |          NA |
| TernaryHeapSort  | 256  | PipeOrgan          |     3,015.0 ns |    185.94 ns |     82.56 ns |  0.56 |    0.02 |    1 |         - |          NA |
| BottomupHeapSort | 256  | PipeOrgan          |     5,547.8 ns |    284.68 ns |    148.89 ns |  1.03 |    0.04 |    2 |         - |          NA |
| WeakHeapSort     | 256  | PipeOrgan          |     5,367.4 ns |    288.79 ns |    151.04 ns |  0.99 |    0.04 |    2 |         - |          NA |
| SmoothSort       | 256  | PipeOrgan          |     4,066.6 ns |    264.74 ns |    138.47 ns |  0.75 |    0.03 |    2 |         - |          NA |
| PoplarHeapSort   | 256  | PipeOrgan          |     4,632.0 ns |    243.40 ns |    108.07 ns |  0.86 |    0.03 |    2 |         - |          NA |
| TournamentSort   | 256  | PipeOrgan          |     3,128.1 ns |    177.80 ns |     78.95 ns |  0.58 |    0.02 |    1 |         - |          NA |
| BinomialHeapSort | 256  | PipeOrgan          |     6,026.1 ns |    253.40 ns |    132.53 ns |  1.11 |    0.03 |    2 |         - |          NA |
| PairingHeapSort  | 256  | PipeOrgan          |     5,442.4 ns |    299.87 ns |    156.84 ns |  1.01 |    0.04 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **256**  | **ManyDuplicates**     |     **6,725.1 ns** |    **143.05 ns** |     **74.82 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 256  | ManyDuplicates     |     6,604.8 ns |    217.90 ns |    113.97 ns |  0.98 |    0.02 |    1 |         - |          NA |
| TernaryHeapSort  | 256  | ManyDuplicates     |    10,226.0 ns |    279.14 ns |    145.99 ns |  1.52 |    0.03 |    2 |         - |          NA |
| BottomupHeapSort | 256  | ManyDuplicates     |     6,883.8 ns |    122.68 ns |     54.47 ns |  1.02 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 256  | ManyDuplicates     |    10,704.5 ns |    354.96 ns |    185.65 ns |  1.59 |    0.03 |    2 |         - |          NA |
| SmoothSort       | 256  | ManyDuplicates     |    12,469.8 ns |    226.40 ns |    100.52 ns |  1.85 |    0.02 |    2 |         - |          NA |
| PoplarHeapSort   | 256  | ManyDuplicates     |    12,879.7 ns |    277.31 ns |    145.04 ns |  1.92 |    0.03 |    2 |         - |          NA |
| TournamentSort   | 256  | ManyDuplicates     |    13,106.8 ns |    373.99 ns |    166.06 ns |  1.95 |    0.03 |    2 |         - |          NA |
| BinomialHeapSort | 256  | ManyDuplicates     |    24,548.8 ns |    382.10 ns |    169.65 ns |  3.65 |    0.05 |    3 |         - |          NA |
| PairingHeapSort  | 256  | ManyDuplicates     |    15,387.0 ns |    230.50 ns |    120.56 ns |  2.29 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Random**             |    **30,623.0 ns** |    **285.41 ns** |    **126.72 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Random             |    30,909.2 ns |    197.35 ns |    103.22 ns |  1.01 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | Random             |    52,708.5 ns |    307.07 ns |    160.61 ns |  1.72 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | Random             |    32,314.8 ns |    140.17 ns |     62.24 ns |  1.06 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | Random             |    64,149.7 ns |    672.96 ns |    351.97 ns |  2.09 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 1024 | Random             |    71,228.6 ns |    251.23 ns |    131.40 ns |  2.33 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 1024 | Random             |    66,001.6 ns |    183.66 ns |     81.54 ns |  2.16 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 1024 | Random             |    65,786.7 ns |    920.11 ns |    481.23 ns |  2.15 |    0.02 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Random             |   157,168.6 ns |  1,177.67 ns |    615.94 ns |  5.13 |    0.03 |    4 |         - |          NA |
| PairingHeapSort  | 1024 | Random             |    80,652.8 ns |    509.55 ns |    226.24 ns |  2.63 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **SingleElementMoved** |    **25,513.8 ns** |    **185.84 ns** |     **97.20 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 1024 | SingleElementMoved |    25,049.5 ns |    316.99 ns |    165.79 ns |  0.98 |    0.01 |    3 |         - |          NA |
| TernaryHeapSort  | 1024 | SingleElementMoved |    19,738.1 ns |  1,075.85 ns |    562.69 ns |  0.77 |    0.02 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | SingleElementMoved |    24,974.8 ns |    202.81 ns |    106.07 ns |  0.98 |    0.01 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | SingleElementMoved |    25,896.6 ns |    329.58 ns |    172.38 ns |  1.02 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 1024 | SingleElementMoved |     4,757.2 ns |    128.23 ns |     67.07 ns |  0.19 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 1024 | SingleElementMoved |    12,922.9 ns |    236.94 ns |    123.92 ns |  0.51 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 1024 | SingleElementMoved |    21,791.6 ns |    750.67 ns |    333.30 ns |  0.85 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | SingleElementMoved |    26,014.0 ns |    293.52 ns |    153.52 ns |  1.02 |    0.01 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | SingleElementMoved |    17,314.4 ns |    265.34 ns |    117.81 ns |  0.68 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Sorted**             |    **24,825.7 ns** |    **268.08 ns** |    **140.21 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Sorted             |    23,091.4 ns |    315.29 ns |    164.90 ns |  0.93 |    0.01 |    3 |         - |          NA |
| TernaryHeapSort  | 1024 | Sorted             |    15,537.3 ns |    440.99 ns |    230.64 ns |  0.63 |    0.01 |    3 |         - |          NA |
| BottomupHeapSort | 1024 | Sorted             |    24,348.9 ns |    225.14 ns |    117.75 ns |  0.98 |    0.01 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | Sorted             |    24,724.8 ns |    200.13 ns |    104.67 ns |  1.00 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 1024 | Sorted             |     3,836.3 ns |    285.98 ns |    149.57 ns |  0.15 |    0.01 |    1 |         - |          NA |
| PoplarHeapSort   | 1024 | Sorted             |    11,656.1 ns |    164.12 ns |     85.84 ns |  0.47 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 1024 | Sorted             |    20,270.6 ns |    884.17 ns |    462.44 ns |  0.82 |    0.02 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Sorted             |    22,947.4 ns |    144.88 ns |     75.77 ns |  0.92 |    0.01 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | Sorted             |    17,028.7 ns |    249.93 ns |    130.72 ns |  0.69 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **Reversed**           |    **22,890.3 ns** |    **192.79 ns** |    **100.83 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 1024 | Reversed           |    25,012.7 ns |    178.72 ns |     93.47 ns |  1.09 |    0.01 |    3 |         - |          NA |
| TernaryHeapSort  | 1024 | Reversed           |    12,989.3 ns |    661.16 ns |    345.80 ns |  0.57 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | Reversed           |    25,051.3 ns |    238.12 ns |    124.54 ns |  1.09 |    0.01 |    3 |         - |          NA |
| WeakHeapSort     | 1024 | Reversed           |    27,625.4 ns |    110.03 ns |     48.85 ns |  1.21 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 1024 | Reversed           |    18,733.7 ns |    307.03 ns |    136.32 ns |  0.82 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 1024 | Reversed           |    21,419.3 ns |    336.26 ns |    175.87 ns |  0.94 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 1024 | Reversed           |    21,031.1 ns |  1,122.63 ns |    498.45 ns |  0.92 |    0.02 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | Reversed           |    22,977.7 ns |    292.68 ns |    153.08 ns |  1.00 |    0.01 |    3 |         - |          NA |
| PairingHeapSort  | 1024 | Reversed           |     8,022.0 ns |    314.12 ns |    164.29 ns |  0.35 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **PipeOrgan**          |    **24,375.3 ns** |    **192.60 ns** |    **100.73 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 1024 | PipeOrgan          |    23,102.5 ns |    281.80 ns |    147.39 ns |  0.95 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 1024 | PipeOrgan          |    14,547.1 ns |    493.60 ns |    258.16 ns |  0.60 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 1024 | PipeOrgan          |    24,396.0 ns |    234.32 ns |    122.56 ns |  1.00 |    0.01 |    2 |         - |          NA |
| WeakHeapSort     | 1024 | PipeOrgan          |    26,846.7 ns |    223.00 ns |     79.52 ns |  1.10 |    0.01 |    2 |         - |          NA |
| SmoothSort       | 1024 | PipeOrgan          |    20,153.0 ns |    276.28 ns |    122.67 ns |  0.83 |    0.01 |    2 |         - |          NA |
| PoplarHeapSort   | 1024 | PipeOrgan          |    21,848.2 ns |    321.06 ns |    142.55 ns |  0.90 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 1024 | PipeOrgan          |    15,267.6 ns |    717.05 ns |    375.03 ns |  0.63 |    0.01 |    1 |         - |          NA |
| BinomialHeapSort | 1024 | PipeOrgan          |    25,800.3 ns |    201.41 ns |    105.34 ns |  1.06 |    0.01 |    2 |         - |          NA |
| PairingHeapSort  | 1024 | PipeOrgan          |    35,267.5 ns |  9,375.10 ns |  4,903.36 ns |  1.45 |    0.19 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **1024** | **ManyDuplicates**     |    **29,283.8 ns** |    **338.58 ns** |    **177.08 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 1024 | ManyDuplicates     |    29,245.4 ns |    257.13 ns |    134.48 ns |  1.00 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 1024 | ManyDuplicates     |    52,975.8 ns | 10,138.30 ns |  5,302.53 ns |  1.81 |    0.17 |    2 |         - |          NA |
| BottomupHeapSort | 1024 | ManyDuplicates     |    29,981.3 ns |    221.31 ns |     98.26 ns |  1.02 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 1024 | ManyDuplicates     |    49,610.0 ns |    509.03 ns |    181.52 ns |  1.69 |    0.01 |    2 |         - |          NA |
| SmoothSort       | 1024 | ManyDuplicates     |    63,850.0 ns |    205.38 ns |     91.19 ns |  2.18 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 1024 | ManyDuplicates     |    62,048.6 ns |    385.17 ns |    201.45 ns |  2.12 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 1024 | ManyDuplicates     |    65,027.7 ns |    834.17 ns |    436.29 ns |  2.22 |    0.02 |    3 |         - |          NA |
| BinomialHeapSort | 1024 | ManyDuplicates     |   123,532.0 ns |  1,249.95 ns |    653.75 ns |  4.22 |    0.03 |    4 |         - |          NA |
| PairingHeapSort  | 1024 | ManyDuplicates     |    69,783.6 ns |  1,502.66 ns |    667.19 ns |  2.38 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Random**             |   **131,246.7 ns** |  **1,820.84 ns** |    **952.34 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Random             |   132,066.9 ns |  1,300.90 ns |    680.40 ns |  1.01 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | Random             |   250,454.6 ns |    668.94 ns |    297.01 ns |  1.91 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | Random             |   143,424.3 ns |    326.10 ns |    144.79 ns |  1.09 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | Random             |   309,789.8 ns |    985.59 ns |    437.61 ns |  2.36 |    0.02 |    3 |         - |          NA |
| SmoothSort       | 4096 | Random             |   341,695.3 ns |    982.90 ns |    436.41 ns |  2.60 |    0.02 |    3 |         - |          NA |
| PoplarHeapSort   | 4096 | Random             |   309,835.7 ns |    507.65 ns |    225.40 ns |  2.36 |    0.02 |    3 |         - |          NA |
| TournamentSort   | 4096 | Random             |   312,995.5 ns |  1,266.22 ns |    562.21 ns |  2.38 |    0.02 |    3 |         - |          NA |
| BinomialHeapSort | 4096 | Random             |   811,965.9 ns |  5,368.26 ns |  2,807.70 ns |  6.19 |    0.05 |    4 |         - |          NA |
| PairingHeapSort  | 4096 | Random             |   391,341.7 ns |  2,386.18 ns |  1,248.02 ns |  2.98 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **SingleElementMoved** |   **102,697.0 ns** |    **346.38 ns** |    **153.80 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 4096 | SingleElementMoved |   105,163.8 ns |    316.95 ns |    140.73 ns |  1.02 |    0.00 |    4 |         - |          NA |
| TernaryHeapSort  | 4096 | SingleElementMoved |   120,637.3 ns |  5,647.53 ns |  2,507.54 ns |  1.17 |    0.02 |    4 |         - |          NA |
| BottomupHeapSort | 4096 | SingleElementMoved |   110,963.1 ns |    264.10 ns |    138.13 ns |  1.08 |    0.00 |    4 |         - |          NA |
| WeakHeapSort     | 4096 | SingleElementMoved |   123,501.0 ns |    609.09 ns |    270.44 ns |  1.20 |    0.00 |    4 |         - |          NA |
| SmoothSort       | 4096 | SingleElementMoved |    18,732.6 ns |    209.75 ns |    109.70 ns |  0.18 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 4096 | SingleElementMoved |    56,367.4 ns |    297.55 ns |    155.63 ns |  0.55 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 4096 | SingleElementMoved |   125,493.7 ns |  8,831.17 ns |  4,618.87 ns |  1.22 |    0.04 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | SingleElementMoved |   114,983.5 ns |    810.33 ns |    359.79 ns |  1.12 |    0.00 |    4 |         - |          NA |
| PairingHeapSort  | 4096 | SingleElementMoved |    69,911.8 ns |    604.86 ns |    268.56 ns |  0.68 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Sorted**             |   **100,423.3 ns** |  **1,083.67 ns** |    **566.78 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Sorted             |    97,959.8 ns |  1,345.99 ns |    597.63 ns |  0.98 |    0.01 |    4 |         - |          NA |
| TernaryHeapSort  | 4096 | Sorted             |    77,652.7 ns |  4,455.10 ns |  2,330.11 ns |  0.77 |    0.02 |    3 |         - |          NA |
| BottomupHeapSort | 4096 | Sorted             |   108,637.7 ns |    452.98 ns |    201.13 ns |  1.08 |    0.01 |    4 |         - |          NA |
| WeakHeapSort     | 4096 | Sorted             |   120,624.4 ns |    388.77 ns |    172.62 ns |  1.20 |    0.01 |    4 |         - |          NA |
| SmoothSort       | 4096 | Sorted             |    15,036.9 ns |     54.02 ns |     28.25 ns |  0.15 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 4096 | Sorted             |    51,267.3 ns |    240.10 ns |    106.61 ns |  0.51 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 4096 | Sorted             |   102,870.0 ns |  7,238.15 ns |  3,785.69 ns |  1.02 |    0.04 |    4 |         - |          NA |
| BinomialHeapSort | 4096 | Sorted             |   102,504.2 ns |  1,209.27 ns |    632.47 ns |  1.02 |    0.01 |    4 |         - |          NA |
| PairingHeapSort  | 4096 | Sorted             |    69,959.4 ns |  1,020.51 ns |    453.11 ns |  0.70 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **Reversed**           |    **97,307.4 ns** |  **1,335.85 ns** |    **593.12 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 4096 | Reversed           |   102,451.5 ns |  3,391.78 ns |  1,773.97 ns |  1.05 |    0.02 |    3 |         - |          NA |
| TernaryHeapSort  | 4096 | Reversed           |    77,624.5 ns |  9,589.03 ns |  5,015.25 ns |  0.80 |    0.05 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | Reversed           |   126,582.1 ns | 32,432.09 ns | 16,962.61 ns |  1.30 |    0.16 |    3 |         - |          NA |
| WeakHeapSort     | 4096 | Reversed           |   130,430.6 ns |    656.31 ns |    291.40 ns |  1.34 |    0.01 |    3 |         - |          NA |
| SmoothSort       | 4096 | Reversed           |   102,953.6 ns |  4,223.93 ns |  1,875.45 ns |  1.06 |    0.02 |    3 |         - |          NA |
| PoplarHeapSort   | 4096 | Reversed           |    97,624.7 ns |  2,524.83 ns |  1,121.04 ns |  1.00 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 4096 | Reversed           |   135,723.6 ns | 23,874.08 ns | 10,600.24 ns |  1.39 |    0.10 |    3 |         - |          NA |
| BinomialHeapSort | 4096 | Reversed           |   100,914.1 ns |    573.48 ns |    254.63 ns |  1.04 |    0.01 |    3 |         - |          NA |
| PairingHeapSort  | 4096 | Reversed           |    31,903.3 ns |    290.42 ns |    151.90 ns |  0.33 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **PipeOrgan**          |   **101,625.6 ns** |    **891.56 ns** |    **395.86 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | PipeOrgan          |   100,373.3 ns |    746.48 ns |    331.44 ns |  0.99 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | PipeOrgan          |    72,168.2 ns |  1,888.95 ns |    987.96 ns |  0.71 |    0.01 |    1 |         - |          NA |
| BottomupHeapSort | 4096 | PipeOrgan          |   110,206.6 ns |    778.58 ns |    407.21 ns |  1.08 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | PipeOrgan          |   131,551.2 ns |    602.83 ns |    267.66 ns |  1.29 |    0.01 |    1 |         - |          NA |
| SmoothSort       | 4096 | PipeOrgan          |   209,122.2 ns |  2,594.03 ns |  1,356.73 ns |  2.06 |    0.01 |    2 |         - |          NA |
| PoplarHeapSort   | 4096 | PipeOrgan          |   107,846.2 ns |  3,079.81 ns |  1,367.46 ns |  1.06 |    0.01 |    1 |         - |          NA |
| TournamentSort   | 4096 | PipeOrgan          |    79,786.9 ns |  1,983.50 ns |    880.69 ns |  0.79 |    0.01 |    1 |         - |          NA |
| BinomialHeapSort | 4096 | PipeOrgan          |   113,167.2 ns |    762.41 ns |    338.52 ns |  1.11 |    0.01 |    1 |         - |          NA |
| PairingHeapSort  | 4096 | PipeOrgan          |    92,083.3 ns |    718.30 ns |    318.93 ns |  0.91 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **4096** | **ManyDuplicates**     |   **121,137.2 ns** |    **798.74 ns** |    **417.76 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 4096 | ManyDuplicates     |   121,663.3 ns |    623.85 ns |    276.99 ns |  1.00 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 4096 | ManyDuplicates     |   227,580.1 ns |  4,878.88 ns |  2,551.75 ns |  1.88 |    0.02 |    2 |         - |          NA |
| BottomupHeapSort | 4096 | ManyDuplicates     |   127,554.6 ns |    516.74 ns |    229.44 ns |  1.05 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 4096 | ManyDuplicates     |   221,460.9 ns |    741.47 ns |    387.80 ns |  1.83 |    0.01 |    2 |         - |          NA |
| SmoothSort       | 4096 | ManyDuplicates     |   298,378.2 ns |  2,086.20 ns |    926.29 ns |  2.46 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 4096 | ManyDuplicates     |   291,359.0 ns |  1,222.07 ns |    639.17 ns |  2.41 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 4096 | ManyDuplicates     |   310,810.9 ns |    625.16 ns |    277.58 ns |  2.57 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 4096 | ManyDuplicates     |   547,384.8 ns |  3,974.42 ns |  1,764.67 ns |  4.52 |    0.02 |    4 |         - |          NA |
| PairingHeapSort  | 4096 | ManyDuplicates     |   304,224.9 ns |  1,825.44 ns |    810.51 ns |  2.51 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Random**             |   **268,023.5 ns** |    **548.68 ns** |    **195.66 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Random             |   271,593.0 ns |  1,655.21 ns |    865.71 ns |  1.01 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | Random             |   539,978.2 ns |  5,802.56 ns |  3,034.85 ns |  2.01 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | Random             |   308,700.4 ns |  1,679.85 ns |    745.86 ns |  1.15 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | Random             |   673,056.5 ns |  1,590.55 ns |    706.22 ns |  2.51 |    0.00 |    3 |         - |          NA |
| SmoothSort       | 8192 | Random             |   742,403.8 ns |  2,784.85 ns |  1,236.49 ns |  2.77 |    0.00 |    3 |         - |          NA |
| PoplarHeapSort   | 8192 | Random             |   662,932.9 ns |  1,395.21 ns |    729.72 ns |  2.47 |    0.00 |    3 |         - |          NA |
| TournamentSort   | 8192 | Random             |   691,356.7 ns |  1,060.20 ns |    554.50 ns |  2.58 |    0.00 |    3 |         - |          NA |
| BinomialHeapSort | 8192 | Random             | 1,783,553.1 ns |  8,782.70 ns |  4,593.52 ns |  6.65 |    0.02 |    4 |         - |          NA |
| PairingHeapSort  | 8192 | Random             |   874,156.4 ns |  8,192.99 ns |  4,285.09 ns |  3.26 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **SingleElementMoved** |   **216,114.9 ns** |  **1,334.64 ns** |    **592.59 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| MinHeapSort      | 8192 | SingleElementMoved |   215,505.3 ns |    645.13 ns |    230.06 ns |  1.00 |    0.00 |    3 |         - |          NA |
| TernaryHeapSort  | 8192 | SingleElementMoved |   348,736.8 ns |  1,870.59 ns |    978.35 ns |  1.61 |    0.01 |    4 |         - |          NA |
| BottomupHeapSort | 8192 | SingleElementMoved |   244,071.6 ns |  1,448.58 ns |    757.63 ns |  1.13 |    0.00 |    3 |         - |          NA |
| WeakHeapSort     | 8192 | SingleElementMoved |   269,386.8 ns |    505.24 ns |    224.33 ns |  1.25 |    0.00 |    3 |         - |          NA |
| SmoothSort       | 8192 | SingleElementMoved |    37,214.9 ns |    418.61 ns |    185.87 ns |  0.17 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 8192 | SingleElementMoved |   117,621.8 ns |    696.75 ns |    309.36 ns |  0.54 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 8192 | SingleElementMoved |   381,918.3 ns |  1,506.38 ns |    787.87 ns |  1.77 |    0.01 |    4 |         - |          NA |
| BinomialHeapSort | 8192 | SingleElementMoved |   242,081.1 ns |  6,056.49 ns |  3,167.66 ns |  1.12 |    0.01 |    3 |         - |          NA |
| PairingHeapSort  | 8192 | SingleElementMoved |   139,946.7 ns |  1,922.30 ns |    853.51 ns |  0.65 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Sorted**             |   **214,566.3 ns** |  **1,498.13 ns** |    **665.18 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Sorted             |   222,005.3 ns | 71,942.53 ns | 31,942.93 ns |  1.03 |    0.14 |    4 |         - |          NA |
| TernaryHeapSort  | 8192 | Sorted             |   333,656.0 ns |  7,666.76 ns |  4,009.86 ns |  1.56 |    0.02 |    5 |         - |          NA |
| BottomupHeapSort | 8192 | Sorted             |   239,604.2 ns |    718.49 ns |    375.78 ns |  1.12 |    0.00 |    4 |         - |          NA |
| WeakHeapSort     | 8192 | Sorted             |   265,626.8 ns |  1,480.80 ns |    657.48 ns |  1.24 |    0.00 |    4 |         - |          NA |
| SmoothSort       | 8192 | Sorted             |    30,061.5 ns |    241.44 ns |    126.28 ns |  0.14 |    0.00 |    1 |         - |          NA |
| PoplarHeapSort   | 8192 | Sorted             |   108,098.1 ns |    418.31 ns |    185.73 ns |  0.50 |    0.00 |    2 |         - |          NA |
| TournamentSort   | 8192 | Sorted             |   387,805.9 ns | 23,327.94 ns | 10,357.75 ns |  1.81 |    0.05 |    5 |         - |          NA |
| BinomialHeapSort | 8192 | Sorted             |   213,587.1 ns |  1,130.61 ns |    502.00 ns |  1.00 |    0.00 |    4 |         - |          NA |
| PairingHeapSort  | 8192 | Sorted             |   141,176.6 ns |  1,131.72 ns |    502.49 ns |  0.66 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **Reversed**           |   **198,891.6 ns** |  **1,416.48 ns** |    **740.84 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| MinHeapSort      | 8192 | Reversed           |   218,618.8 ns |  1,823.39 ns |    953.67 ns |  1.10 |    0.01 |    2 |         - |          NA |
| TernaryHeapSort  | 8192 | Reversed           |   328,841.6 ns |  1,901.77 ns |    994.66 ns |  1.65 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | Reversed           |   244,550.0 ns |  2,361.71 ns |    842.21 ns |  1.23 |    0.01 |    2 |         - |          NA |
| WeakHeapSort     | 8192 | Reversed           |   284,240.5 ns |    977.98 ns |    434.23 ns |  1.43 |    0.01 |    2 |         - |          NA |
| SmoothSort       | 8192 | Reversed           |   438,967.4 ns |  5,919.09 ns |  2,628.11 ns |  2.21 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 8192 | Reversed           |   321,864.1 ns |    797.96 ns |    354.30 ns |  1.62 |    0.01 |    2 |         - |          NA |
| TournamentSort   | 8192 | Reversed           |   474,138.3 ns | 37,712.34 ns | 19,724.28 ns |  2.38 |    0.09 |    3 |         - |          NA |
| BinomialHeapSort | 8192 | Reversed           |   212,482.7 ns |    799.33 ns |    354.91 ns |  1.07 |    0.00 |    2 |         - |          NA |
| PairingHeapSort  | 8192 | Reversed           |    63,676.7 ns |    309.54 ns |    137.44 ns |  0.32 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **PipeOrgan**          |   **211,829.1 ns** |  **1,940.19 ns** |  **1,014.76 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | PipeOrgan          |   204,352.3 ns |  1,757.62 ns |    780.40 ns |  0.96 |    0.01 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | PipeOrgan          |   375,784.7 ns |  5,510.79 ns |  2,882.25 ns |  1.77 |    0.02 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | PipeOrgan          |   242,402.0 ns |  1,225.37 ns |    436.98 ns |  1.14 |    0.01 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | PipeOrgan          |   287,949.4 ns |    900.77 ns |    399.95 ns |  1.36 |    0.01 |    1 |         - |          NA |
| SmoothSort       | 8192 | PipeOrgan          |   551,630.4 ns |  1,793.79 ns |    796.46 ns |  2.60 |    0.01 |    2 |         - |          NA |
| PoplarHeapSort   | 8192 | PipeOrgan          |   474,942.4 ns | 84,048.16 ns | 37,317.91 ns |  2.24 |    0.16 |    2 |         - |          NA |
| TournamentSort   | 8192 | PipeOrgan          |   394,863.3 ns |  2,127.91 ns |    758.83 ns |  1.86 |    0.01 |    2 |         - |          NA |
| BinomialHeapSort | 8192 | PipeOrgan          |   235,221.9 ns |  1,844.32 ns |    818.89 ns |  1.11 |    0.01 |    1 |         - |          NA |
| PairingHeapSort  | 8192 | PipeOrgan          |   190,303.1 ns |    987.31 ns |    516.38 ns |  0.90 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **HeapSort**         | **8192** | **ManyDuplicates**     |   **243,579.0 ns** |    **937.97 ns** |    **490.58 ns** |  **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| MinHeapSort      | 8192 | ManyDuplicates     |   247,905.8 ns |    747.73 ns |    332.00 ns |  1.02 |    0.00 |    1 |         - |          NA |
| TernaryHeapSort  | 8192 | ManyDuplicates     |   496,252.8 ns |  2,250.02 ns |  1,176.80 ns |  2.04 |    0.01 |    2 |         - |          NA |
| BottomupHeapSort | 8192 | ManyDuplicates     |   274,202.6 ns |    911.23 ns |    404.59 ns |  1.13 |    0.00 |    1 |         - |          NA |
| WeakHeapSort     | 8192 | ManyDuplicates     |   466,249.5 ns |  3,201.40 ns |  1,674.39 ns |  1.91 |    0.01 |    2 |         - |          NA |
| SmoothSort       | 8192 | ManyDuplicates     |   625,000.1 ns |  2,055.23 ns |  1,074.93 ns |  2.57 |    0.01 |    3 |         - |          NA |
| PoplarHeapSort   | 8192 | ManyDuplicates     |   616,300.1 ns |  1,529.96 ns |    679.31 ns |  2.53 |    0.01 |    3 |         - |          NA |
| TournamentSort   | 8192 | ManyDuplicates     |   659,781.0 ns |  1,307.47 ns |    466.26 ns |  2.71 |    0.01 |    3 |         - |          NA |
| BinomialHeapSort | 8192 | ManyDuplicates     | 1,149,133.0 ns | 31,636.90 ns | 16,546.71 ns |  4.72 |    0.06 |    4 |         - |          NA |
| PairingHeapSort  | 8192 | ManyDuplicates     |   645,844.8 ns |  9,642.38 ns |  5,043.15 ns |  2.65 |    0.02 |    3 |         - |          NA |

### InsertionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.67GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.5 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean         | Error       | StdDev      | Ratio  | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |-------------:|------------:|------------:|-------:|--------:|-----:|----------:|------------:|
| **InsertionSort**          | **256**  | **Random**             |   **7,236.0 ns** |    **76.31 ns** |    **27.21 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Random             |   7,537.5 ns |   170.72 ns |    75.80 ns |   1.04 |    0.01 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | Random             |  11,447.6 ns |   335.36 ns |   148.90 ns |   1.58 |    0.02 |    2 |         - |          NA |
| GnomeSort              | 256  | Random             |  10,852.1 ns |   313.11 ns |   139.02 ns |   1.50 |    0.02 |    2 |         - |          NA |
| LibrarySort            | 256  | Random             |  22,909.8 ns |   182.61 ns |    95.51 ns |   3.17 |    0.02 |    3 |         - |          NA |
| MergeInsertionSort     | 256  | Random             |  20,456.7 ns |   154.98 ns |    68.81 ns |   2.83 |    0.01 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | Random             |   8,014.2 ns |   116.86 ns |    51.88 ns |   1.11 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Random             |   8,153.1 ns |    59.28 ns |    26.32 ns |   1.13 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Random             |   8,656.4 ns |   293.68 ns |   153.60 ns |   1.20 |    0.02 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Random             |   8,373.6 ns |   307.88 ns |   161.03 ns |   1.16 |    0.02 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Random             |   8,817.8 ns |    93.41 ns |    41.47 ns |   1.22 |    0.01 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **SingleElementMoved** |     **226.8 ns** |     **6.94 ns** |     **3.08 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | SingleElementMoved |     240.3 ns |    10.24 ns |     4.55 ns |   1.06 |    0.02 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | SingleElementMoved |   1,140.5 ns |   245.80 ns |   109.14 ns |   5.03 |    0.45 |    3 |         - |          NA |
| GnomeSort              | 256  | SingleElementMoved |     322.9 ns |    10.52 ns |     5.50 ns |   1.42 |    0.03 |    2 |         - |          NA |
| LibrarySort            | 256  | SingleElementMoved |  21,657.4 ns |   234.87 ns |   122.84 ns |  95.52 |    1.32 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | SingleElementMoved |  10,180.1 ns |   312.80 ns |   163.60 ns |  44.90 |    0.89 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | SingleElementMoved |     822.4 ns |    28.37 ns |    12.60 ns |   3.63 |    0.07 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 256  | SingleElementMoved |     799.4 ns |    16.06 ns |     7.13 ns |   3.53 |    0.05 |    3 |         - |          NA |
| ShellSortTokuda1992    | 256  | SingleElementMoved |     990.4 ns |    44.83 ns |    19.91 ns |   4.37 |    0.10 |    3 |         - |          NA |
| ShellSortCiura2001     | 256  | SingleElementMoved |     894.2 ns |    14.54 ns |     7.61 ns |   3.94 |    0.06 |    3 |         - |          NA |
| ShellSortLee2021       | 256  | SingleElementMoved |     981.1 ns |    30.56 ns |    13.57 ns |   4.33 |    0.08 |    3 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **Sorted**             |     **176.6 ns** |    **17.76 ns** |     **7.88 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Sorted             |     183.9 ns |     2.00 ns |     0.89 ns |   1.04 |    0.04 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | Sorted             |     886.2 ns |     7.06 ns |     2.52 ns |   5.03 |    0.20 |    3 |         - |          NA |
| GnomeSort              | 256  | Sorted             |     240.3 ns |    23.23 ns |    10.32 ns |   1.36 |    0.08 |    2 |         - |          NA |
| LibrarySort            | 256  | Sorted             |  21,552.3 ns | 1,115.79 ns |   495.42 ns | 122.22 |    5.55 |    5 |         - |          NA |
| MergeInsertionSort     | 256  | Sorted             |   9,880.4 ns |   304.57 ns |   159.29 ns |  56.03 |    2.40 |    4 |         - |          NA |
| ShellSortKnuth1973     | 256  | Sorted             |     640.6 ns |    25.32 ns |    11.24 ns |   3.63 |    0.16 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Sorted             |     642.4 ns |     3.70 ns |     1.64 ns |   3.64 |    0.15 |    3 |         - |          NA |
| ShellSortTokuda1992    | 256  | Sorted             |     785.1 ns |    26.59 ns |     9.48 ns |   4.45 |    0.19 |    3 |         - |          NA |
| ShellSortCiura2001     | 256  | Sorted             |     705.1 ns |    25.80 ns |    11.46 ns |   4.00 |    0.17 |    3 |         - |          NA |
| ShellSortLee2021       | 256  | Sorted             |     813.9 ns |     9.68 ns |     4.30 ns |   4.62 |    0.19 |    3 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **Reversed**           |  **11,948.1 ns** |   **326.92 ns** |   **145.16 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 256  | Reversed           |   9,497.1 ns |   245.66 ns |   128.49 ns |   0.79 |    0.01 |    3 |         - |          NA |
| BinaryInsertSort       | 256  | Reversed           |   4,744.6 ns |    38.47 ns |    17.08 ns |   0.40 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 256  | Reversed           |  21,538.4 ns |   268.44 ns |   140.40 ns |   1.80 |    0.02 |    5 |         - |          NA |
| LibrarySort            | 256  | Reversed           |  17,782.7 ns |   208.97 ns |    92.78 ns |   1.49 |    0.02 |    4 |         - |          NA |
| MergeInsertionSort     | 256  | Reversed           |   9,988.1 ns |   311.59 ns |   162.97 ns |   0.84 |    0.02 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | Reversed           |   1,128.6 ns |    52.53 ns |    23.33 ns |   0.09 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | Reversed           |   1,089.8 ns |    29.28 ns |    15.32 ns |   0.09 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | Reversed           |   1,135.1 ns |    22.04 ns |     9.79 ns |   0.10 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | Reversed           |   1,144.7 ns |    37.21 ns |    13.27 ns |   0.10 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | Reversed           |   1,136.9 ns |    10.94 ns |     3.90 ns |   0.10 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **PipeOrgan**          |   **6,335.6 ns** |   **121.87 ns** |    **54.11 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 256  | PipeOrgan          |   4,962.5 ns |   308.60 ns |   161.40 ns |   0.78 |    0.02 |    3 |         - |          NA |
| BinaryInsertSort       | 256  | PipeOrgan          |   2,966.7 ns |   231.30 ns |   102.70 ns |   0.47 |    0.02 |    2 |         - |          NA |
| GnomeSort              | 256  | PipeOrgan          |   9,932.5 ns |   247.00 ns |   129.19 ns |   1.57 |    0.02 |    5 |         - |          NA |
| LibrarySort            | 256  | PipeOrgan          |  15,062.7 ns |   428.97 ns |   224.36 ns |   2.38 |    0.04 |    6 |         - |          NA |
| MergeInsertionSort     | 256  | PipeOrgan          |  10,049.7 ns |   303.42 ns |   158.69 ns |   1.59 |    0.03 |    5 |         - |          NA |
| ShellSortKnuth1973     | 256  | PipeOrgan          |   1,028.1 ns |    41.71 ns |    21.81 ns |   0.16 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | PipeOrgan          |   1,059.4 ns |    18.39 ns |     9.62 ns |   0.17 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | PipeOrgan          |   1,124.7 ns |    27.66 ns |    12.28 ns |   0.18 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | PipeOrgan          |   1,076.2 ns |    38.86 ns |    20.32 ns |   0.17 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | PipeOrgan          |   1,142.8 ns |    21.38 ns |     9.49 ns |   0.18 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **256**  | **ManyDuplicates**     |   **7,126.7 ns** |    **63.68 ns** |    **33.30 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 256  | ManyDuplicates     |   7,708.7 ns |   804.94 ns |   421.00 ns |   1.08 |    0.06 |    1 |         - |          NA |
| BinaryInsertSort       | 256  | ManyDuplicates     |  11,225.0 ns |   389.20 ns |   203.56 ns |   1.58 |    0.03 |    2 |         - |          NA |
| GnomeSort              | 256  | ManyDuplicates     |  10,693.7 ns |   298.69 ns |   156.22 ns |   1.50 |    0.02 |    2 |         - |          NA |
| LibrarySort            | 256  | ManyDuplicates     |  24,055.4 ns |   315.32 ns |   164.92 ns |   3.38 |    0.03 |    3 |         - |          NA |
| MergeInsertionSort     | 256  | ManyDuplicates     |  20,255.9 ns |   452.08 ns |   236.45 ns |   2.84 |    0.03 |    3 |         - |          NA |
| ShellSortKnuth1973     | 256  | ManyDuplicates     |   7,582.3 ns |   262.57 ns |   137.33 ns |   1.06 |    0.02 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 256  | ManyDuplicates     |   7,707.7 ns |   328.43 ns |   171.78 ns |   1.08 |    0.02 |    1 |         - |          NA |
| ShellSortTokuda1992    | 256  | ManyDuplicates     |   7,716.1 ns |    97.93 ns |    51.22 ns |   1.08 |    0.01 |    1 |         - |          NA |
| ShellSortCiura2001     | 256  | ManyDuplicates     |   7,627.1 ns |   158.42 ns |    82.86 ns |   1.07 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 256  | ManyDuplicates     |   7,207.2 ns |    83.80 ns |    37.21 ns |   1.01 |    0.01 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Random**             |  **94,330.1 ns** |   **389.10 ns** |   **138.76 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Random             |  90,508.7 ns |   827.16 ns |   367.26 ns |   0.96 |    0.00 |    3 |         - |          NA |
| BinaryInsertSort       | 1024 | Random             |  63,021.6 ns |   224.15 ns |   117.23 ns |   0.67 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | Random             | 151,235.5 ns | 2,246.25 ns |   997.35 ns |   1.60 |    0.01 |    4 |         - |          NA |
| LibrarySort            | 1024 | Random             | 106,036.0 ns |   894.89 ns |   397.34 ns |   1.12 |    0.00 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | Random             | 106,169.4 ns | 1,022.22 ns |   453.87 ns |   1.13 |    0.00 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Random             |  43,886.2 ns |   238.31 ns |   105.81 ns |   0.47 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Random             |  46,164.0 ns |   234.75 ns |   122.78 ns |   0.49 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Random             |  47,904.5 ns |   204.04 ns |   106.72 ns |   0.51 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Random             |  47,192.7 ns |   163.73 ns |    85.64 ns |   0.50 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Random             |  47,820.9 ns |   234.31 ns |   122.55 ns |   0.51 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **SingleElementMoved** |     **715.7 ns** |    **15.78 ns** |     **8.25 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | SingleElementMoved |     834.8 ns |     9.40 ns |     4.17 ns |   1.17 |    0.01 |    1 |         - |          NA |
| BinaryInsertSort       | 1024 | SingleElementMoved |   4,630.9 ns |   962.07 ns |   427.17 ns |   6.47 |    0.56 |    3 |         - |          NA |
| GnomeSort              | 1024 | SingleElementMoved |   1,121.4 ns |    10.26 ns |     4.56 ns |   1.57 |    0.02 |    2 |         - |          NA |
| LibrarySort            | 1024 | SingleElementMoved | 192,966.9 ns |   877.49 ns |   389.61 ns | 269.63 |    2.96 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | SingleElementMoved |  48,580.7 ns |   254.27 ns |   132.99 ns |  67.88 |    0.75 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | SingleElementMoved |   3,546.9 ns |   272.54 ns |   121.01 ns |   4.96 |    0.17 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | SingleElementMoved |   3,920.7 ns |   229.82 ns |   120.20 ns |   5.48 |    0.17 |    3 |         - |          NA |
| ShellSortTokuda1992    | 1024 | SingleElementMoved |   4,140.7 ns |    44.10 ns |    19.58 ns |   5.79 |    0.07 |    3 |         - |          NA |
| ShellSortCiura2001     | 1024 | SingleElementMoved |   4,111.7 ns |    31.35 ns |    13.92 ns |   5.75 |    0.06 |    3 |         - |          NA |
| ShellSortLee2021       | 1024 | SingleElementMoved |   4,219.7 ns |   259.64 ns |   135.80 ns |   5.90 |    0.19 |    3 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Sorted**             |     **578.3 ns** |     **8.53 ns** |     **4.46 ns** |   **1.00** |    **0.00** |    **1** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Sorted             |     716.2 ns |     4.11 ns |     2.15 ns |   1.24 |    0.01 |    2 |         - |          NA |
| BinaryInsertSort       | 1024 | Sorted             |   4,166.6 ns |    74.29 ns |    32.98 ns |   7.21 |    0.07 |    3 |         - |          NA |
| GnomeSort              | 1024 | Sorted             |     850.5 ns |    10.64 ns |     4.73 ns |   1.47 |    0.01 |    2 |         - |          NA |
| LibrarySort            | 1024 | Sorted             | 192,825.7 ns |   972.29 ns |   431.70 ns | 333.46 |    2.53 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | Sorted             |  47,908.4 ns |   146.13 ns |    76.43 ns |  82.85 |    0.62 |    4 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Sorted             |   3,120.1 ns |    10.69 ns |     4.75 ns |   5.40 |    0.04 |    3 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Sorted             |   3,499.8 ns |   156.00 ns |    69.27 ns |   6.05 |    0.12 |    3 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Sorted             |   3,836.7 ns |   307.38 ns |   160.76 ns |   6.63 |    0.27 |    3 |         - |          NA |
| ShellSortCiura2001     | 1024 | Sorted             |   3,830.3 ns |   301.63 ns |   157.76 ns |   6.62 |    0.26 |    3 |         - |          NA |
| ShellSortLee2021       | 1024 | Sorted             |   3,842.8 ns |   245.27 ns |   128.28 ns |   6.65 |    0.21 |    3 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **Reversed**           | **178,154.3 ns** |   **826.87 ns** |   **367.13 ns** |   **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | Reversed           | 145,716.2 ns |   254.59 ns |   113.04 ns |   0.82 |    0.00 |    4 |         - |          NA |
| BinaryInsertSort       | 1024 | Reversed           |  35,243.7 ns |   290.23 ns |   151.80 ns |   0.20 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | Reversed           | 302,559.3 ns | 5,770.84 ns | 3,018.26 ns |   1.70 |    0.02 |    5 |         - |          NA |
| LibrarySort            | 1024 | Reversed           | 151,575.2 ns |   805.78 ns |   357.77 ns |   0.85 |    0.00 |    4 |         - |          NA |
| MergeInsertionSort     | 1024 | Reversed           |  48,518.1 ns |   189.38 ns |    84.09 ns |   0.27 |    0.00 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | Reversed           |   5,438.0 ns |   149.75 ns |    66.49 ns |   0.03 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | Reversed           |   5,546.5 ns |   107.09 ns |    47.55 ns |   0.03 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | Reversed           |   5,917.7 ns |    67.39 ns |    29.92 ns |   0.03 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | Reversed           |   5,833.1 ns |   226.45 ns |   100.55 ns |   0.03 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | Reversed           |   6,035.9 ns |   100.95 ns |    44.82 ns |   0.03 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **PipeOrgan**          |  **89,875.8 ns** |   **259.34 ns** |    **92.48 ns** |   **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | PipeOrgan          |  73,658.2 ns |   216.64 ns |   113.31 ns |   0.82 |    0.00 |    4 |         - |          NA |
| BinaryInsertSort       | 1024 | PipeOrgan          |  20,206.6 ns |   340.02 ns |   177.84 ns |   0.22 |    0.00 |    2 |         - |          NA |
| GnomeSort              | 1024 | PipeOrgan          | 147,275.7 ns |   616.33 ns |   273.65 ns |   1.64 |    0.00 |    6 |         - |          NA |
| LibrarySort            | 1024 | PipeOrgan          |  91,293.3 ns | 1,379.01 ns |   612.29 ns |   1.02 |    0.01 |    5 |         - |          NA |
| MergeInsertionSort     | 1024 | PipeOrgan          |  49,073.3 ns |   311.82 ns |   138.45 ns |   0.55 |    0.00 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | PipeOrgan          |   5,275.1 ns |   234.41 ns |   122.60 ns |   0.06 |    0.00 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | PipeOrgan          |   5,475.8 ns |   221.07 ns |   115.63 ns |   0.06 |    0.00 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | PipeOrgan          |   5,603.3 ns |    17.09 ns |     7.59 ns |   0.06 |    0.00 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | PipeOrgan          |   5,710.6 ns |   336.54 ns |   176.02 ns |   0.06 |    0.00 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | PipeOrgan          |   5,888.2 ns |   248.57 ns |   130.01 ns |   0.07 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |             |        |         |      |           |             |
| **InsertionSort**          | **1024** | **ManyDuplicates**     |  **93,533.2 ns** | **4,004.59 ns** | **2,094.48 ns** |   **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PairInsertionSort      | 1024 | ManyDuplicates     |  89,404.8 ns | 3,512.27 ns | 1,559.47 ns |   0.96 |    0.03 |    3 |         - |          NA |
| BinaryInsertSort       | 1024 | ManyDuplicates     |  59,871.8 ns |   286.00 ns |   126.99 ns |   0.64 |    0.01 |    2 |         - |          NA |
| GnomeSort              | 1024 | ManyDuplicates     | 148,079.4 ns | 1,318.97 ns |   689.85 ns |   1.58 |    0.03 |    4 |         - |          NA |
| LibrarySort            | 1024 | ManyDuplicates     | 112,583.9 ns |   512.15 ns |   227.40 ns |   1.20 |    0.03 |    3 |         - |          NA |
| MergeInsertionSort     | 1024 | ManyDuplicates     | 105,166.5 ns |   956.34 ns |   500.18 ns |   1.12 |    0.02 |    3 |         - |          NA |
| ShellSortKnuth1973     | 1024 | ManyDuplicates     |  33,854.4 ns |   456.90 ns |   202.87 ns |   0.36 |    0.01 |    1 |         - |          NA |
| ShellSortSedgewick1986 | 1024 | ManyDuplicates     |  34,890.9 ns |   432.21 ns |   226.05 ns |   0.37 |    0.01 |    1 |         - |          NA |
| ShellSortTokuda1992    | 1024 | ManyDuplicates     |  34,977.7 ns |   192.21 ns |    85.34 ns |   0.37 |    0.01 |    1 |         - |          NA |
| ShellSortCiura2001     | 1024 | ManyDuplicates     |  34,262.3 ns |   191.03 ns |    99.91 ns |   0.37 |    0.01 |    1 |         - |          NA |
| ShellSortLee2021       | 1024 | ManyDuplicates     |  33,524.2 ns |   232.71 ns |   121.71 ns |   0.36 |    0.01 |    1 |         - |          NA |

### MergeBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.67GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.5 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                   | Size | Pattern            | Mean           | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------- |----- |------------------- |---------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **MergeSort**                | **256**  | **Random**             |    **12,629.4 ns** |    **185.99 ns** |     **97.28 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Random             |    12,115.5 ns |    344.49 ns |    180.17 ns |  0.96 |    0.02 |    4 |         - |          NA |
| BottomupMergeSort        | 256  | Random             |     9,044.0 ns |    281.32 ns |    147.14 ns |  0.72 |    0.01 |    3 |         - |          NA |
| StdStableSort            | 256  | Random             |     5,441.8 ns |    239.57 ns |    106.37 ns |  0.43 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 256  | Random             |    18,511.6 ns |    299.19 ns |    132.84 ns |  1.47 |    0.01 |    5 |         - |          NA |
| RotateMergeSortRecursive | 256  | Random             |    22,774.3 ns |    598.83 ns |    313.20 ns |  1.80 |    0.03 |    6 |         - |          NA |
| SymMergeSort             | 256  | Random             |    12,513.8 ns |    368.82 ns |    192.90 ns |  0.99 |    0.02 |    4 |         - |          NA |
| BlockMergeSort           | 256  | Random             |    11,875.8 ns |  3,000.63 ns |  1,569.39 ns |  0.94 |    0.12 |    4 |         - |          NA |
| NaturalMergeSort         | 256  | Random             |     9,217.4 ns |    321.72 ns |    168.26 ns |  0.73 |    0.01 |    3 |         - |          NA |
| PatienceSort             | 256  | Random             |    13,073.8 ns |    337.63 ns |    149.91 ns |  1.04 |    0.01 |    4 |         - |          NA |
| DropMergeSort            | 256  | Random             |     9,495.6 ns |    354.56 ns |    157.43 ns |  0.75 |    0.01 |    3 |         - |          NA |
| TimSort                  | 256  | Random             |    10,231.8 ns |    351.53 ns |    183.86 ns |  0.81 |    0.01 |    3 |         - |          NA |
| PowerSort                | 256  | Random             |     6,686.6 ns |    152.87 ns |     67.88 ns |  0.53 |    0.01 |    2 |         - |          NA |
| ShiftSort                | 256  | Random             |    10,015.5 ns |    172.34 ns |     76.52 ns |  0.79 |    0.01 |    3 |         - |          NA |
| SpinSort                 | 256  | Random             |     5,865.2 ns |    390.38 ns |    204.18 ns |  0.46 |    0.02 |    2 |         - |          NA |
| Glidesort                | 256  | Random             |     5,696.7 ns |    210.48 ns |     93.45 ns |  0.45 |    0.01 |    2 |         - |          NA |
| Driftsort                | 256  | Random             |     4,915.7 ns |    265.09 ns |    138.65 ns |  0.39 |    0.01 |    2 |         - |          NA |
| FlatStableSort           | 256  | Random             |     6,711.6 ns |    222.99 ns |    116.63 ns |  0.53 |    0.01 |    2 |         - |          NA |
| QuadSort                 | 256  | Random             |     3,488.3 ns |    428.54 ns |    224.14 ns |  0.28 |    0.02 |    1 |         - |          NA |
| SimdMergeSort            | 256  | Random             |     2,938.6 ns |    228.58 ns |    119.55 ns |  0.23 |    0.01 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **SingleElementMoved** |     **3,516.2 ns** |     **34.58 ns** |     **15.35 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | SingleElementMoved |     4,189.1 ns |     49.05 ns |     21.78 ns |  1.19 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 256  | SingleElementMoved |     1,376.0 ns |     84.89 ns |     37.69 ns |  0.39 |    0.01 |    4 |         - |          NA |
| StdStableSort            | 256  | SingleElementMoved |       577.4 ns |     37.25 ns |     16.54 ns |  0.16 |    0.00 |    2 |         - |          NA |
| RotateMergeSort          | 256  | SingleElementMoved |       515.6 ns |     39.72 ns |     17.63 ns |  0.15 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 256  | SingleElementMoved |       721.3 ns |     64.61 ns |     33.79 ns |  0.21 |    0.01 |    2 |         - |          NA |
| SymMergeSort             | 256  | SingleElementMoved |       381.9 ns |     33.28 ns |     14.77 ns |  0.11 |    0.00 |    1 |         - |          NA |
| BlockMergeSort           | 256  | SingleElementMoved |     2,352.9 ns |    290.98 ns |    152.19 ns |  0.67 |    0.04 |    5 |         - |          NA |
| NaturalMergeSort         | 256  | SingleElementMoved |       343.7 ns |     15.89 ns |      8.31 ns |  0.10 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 256  | SingleElementMoved |     7,173.6 ns |    429.17 ns |    224.47 ns |  2.04 |    0.06 |    7 |         - |          NA |
| DropMergeSort            | 256  | SingleElementMoved |       519.7 ns |      9.04 ns |      4.02 ns |  0.15 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | SingleElementMoved |       307.7 ns |     24.84 ns |     11.03 ns |  0.09 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | SingleElementMoved |       343.5 ns |     17.14 ns |      8.97 ns |  0.10 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | SingleElementMoved |       385.9 ns |     18.66 ns |      9.76 ns |  0.11 |    0.00 |    1 |         - |          NA |
| SpinSort                 | 256  | SingleElementMoved |       676.1 ns |     53.08 ns |     23.57 ns |  0.19 |    0.01 |    2 |         - |          NA |
| Glidesort                | 256  | SingleElementMoved |       667.8 ns |     24.95 ns |     13.05 ns |  0.19 |    0.00 |    2 |         - |          NA |
| Driftsort                | 256  | SingleElementMoved |       560.7 ns |     25.94 ns |     11.52 ns |  0.16 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 256  | SingleElementMoved |     1,120.1 ns |     21.32 ns |      9.47 ns |  0.32 |    0.00 |    3 |         - |          NA |
| QuadSort                 | 256  | SingleElementMoved |       526.5 ns |     29.31 ns |     13.02 ns |  0.15 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 256  | SingleElementMoved |     2,514.4 ns |     35.40 ns |     12.62 ns |  0.72 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **Sorted**             |     **3,398.6 ns** |    **287.72 ns** |    **127.75 ns** |  **1.00** |    **0.00** |   **10** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Sorted             |     4,135.2 ns |    322.78 ns |    168.82 ns |  1.22 |    0.06 |   11 |         - |          NA |
| BottomupMergeSort        | 256  | Sorted             |     1,211.5 ns |     13.23 ns |      6.92 ns |  0.36 |    0.01 |    7 |         - |          NA |
| StdStableSort            | 256  | Sorted             |       465.3 ns |      5.94 ns |      2.64 ns |  0.14 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 256  | Sorted             |       261.5 ns |      4.30 ns |      2.25 ns |  0.08 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 256  | Sorted             |       381.7 ns |      3.92 ns |      1.40 ns |  0.11 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 256  | Sorted             |       255.8 ns |      5.14 ns |      2.69 ns |  0.08 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Sorted             |     1,967.7 ns |      9.98 ns |      4.43 ns |  0.58 |    0.02 |    8 |         - |          NA |
| NaturalMergeSort         | 256  | Sorted             |       172.3 ns |      7.63 ns |      3.99 ns |  0.05 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 256  | Sorted             |     6,171.2 ns |    434.04 ns |    227.01 ns |  1.82 |    0.09 |   12 |         - |          NA |
| DropMergeSort            | 256  | Sorted             |       478.4 ns |      6.03 ns |      2.68 ns |  0.14 |    0.00 |    5 |         - |          NA |
| TimSort                  | 256  | Sorted             |       143.7 ns |     19.89 ns |      8.83 ns |  0.04 |    0.00 |    2 |         - |          NA |
| PowerSort                | 256  | Sorted             |       159.0 ns |      6.24 ns |      2.77 ns |  0.05 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 256  | Sorted             |       185.5 ns |      5.12 ns |      2.27 ns |  0.05 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 256  | Sorted             |       117.3 ns |      8.32 ns |      4.35 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Sorted             |       182.2 ns |     15.40 ns |      8.05 ns |  0.05 |    0.00 |    2 |         - |          NA |
| Driftsort                | 256  | Sorted             |       171.7 ns |      5.93 ns |      2.63 ns |  0.05 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 256  | Sorted             |     1,002.2 ns |      9.84 ns |      4.37 ns |  0.30 |    0.01 |    6 |         - |          NA |
| QuadSort                 | 256  | Sorted             |       270.0 ns |      8.51 ns |      3.78 ns |  0.08 |    0.00 |    3 |         - |          NA |
| SimdMergeSort            | 256  | Sorted             |     2,502.8 ns |     14.11 ns |      7.38 ns |  0.74 |    0.03 |    9 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **Reversed**           |     **6,931.3 ns** |     **19.57 ns** |     **10.24 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | Reversed           |     6,287.1 ns |    255.99 ns |    133.89 ns |  0.91 |    0.02 |    6 |         - |          NA |
| BottomupMergeSort        | 256  | Reversed           |     3,794.7 ns |    267.87 ns |    140.10 ns |  0.55 |    0.02 |    5 |         - |          NA |
| StdStableSort            | 256  | Reversed           |     3,194.4 ns |     18.95 ns |      6.76 ns |  0.46 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 256  | Reversed           |     4,309.9 ns |     33.28 ns |     11.87 ns |  0.62 |    0.00 |    5 |         - |          NA |
| RotateMergeSortRecursive | 256  | Reversed           |     6,094.9 ns |    171.46 ns |     89.68 ns |  0.88 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 256  | Reversed           |     1,744.2 ns |     35.82 ns |     15.91 ns |  0.25 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 256  | Reversed           |     2,439.9 ns |     38.53 ns |     13.74 ns |  0.35 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 256  | Reversed           |       233.4 ns |     25.47 ns |     11.31 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 256  | Reversed           |     1,286.8 ns |     31.04 ns |     16.24 ns |  0.19 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 256  | Reversed           |     1,229.3 ns |     42.61 ns |     22.29 ns |  0.18 |    0.00 |    2 |         - |          NA |
| TimSort                  | 256  | Reversed           |       199.6 ns |      3.70 ns |      1.64 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 256  | Reversed           |       184.9 ns |      2.42 ns |      0.86 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | Reversed           |     3,196.0 ns |     25.95 ns |     11.52 ns |  0.46 |    0.00 |    5 |         - |          NA |
| SpinSort                 | 256  | Reversed           |       226.8 ns |     18.93 ns |      9.90 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 256  | Reversed           |       225.7 ns |      8.70 ns |      3.86 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 256  | Reversed           |       227.9 ns |      1.68 ns |      0.60 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | Reversed           |     1,718.9 ns |     25.40 ns |      9.06 ns |  0.25 |    0.00 |    3 |         - |          NA |
| QuadSort                 | 256  | Reversed           |       242.8 ns |      1.77 ns |      0.79 ns |  0.04 |    0.00 |    1 |         - |          NA |
| SimdMergeSort            | 256  | Reversed           |     2,116.7 ns |     21.97 ns |      7.83 ns |  0.31 |    0.00 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **PipeOrgan**          |     **5,249.7 ns** |    **200.71 ns** |     **89.12 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | PipeOrgan          |     5,307.8 ns |    322.91 ns |    168.89 ns |  1.01 |    0.03 |    6 |         - |          NA |
| BottomupMergeSort        | 256  | PipeOrgan          |     2,497.6 ns |     27.12 ns |     12.04 ns |  0.48 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 256  | PipeOrgan          |     1,910.8 ns |     17.95 ns |      7.97 ns |  0.36 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 256  | PipeOrgan          |     4,460.5 ns |    696.56 ns |    309.28 ns |  0.85 |    0.06 |    6 |         - |          NA |
| RotateMergeSortRecursive | 256  | PipeOrgan          |     5,756.9 ns |    276.25 ns |    122.66 ns |  1.10 |    0.03 |    6 |         - |          NA |
| SymMergeSort             | 256  | PipeOrgan          |     2,760.2 ns |    243.04 ns |    127.11 ns |  0.53 |    0.02 |    5 |         - |          NA |
| BlockMergeSort           | 256  | PipeOrgan          |     2,405.6 ns |     27.19 ns |     12.07 ns |  0.46 |    0.01 |    5 |         - |          NA |
| NaturalMergeSort         | 256  | PipeOrgan          |       411.0 ns |     22.65 ns |     10.06 ns |  0.08 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 256  | PipeOrgan          |     4,619.3 ns |    430.97 ns |    225.41 ns |  0.88 |    0.04 |    6 |         - |          NA |
| DropMergeSort            | 256  | PipeOrgan          |     5,221.1 ns |    223.35 ns |    116.82 ns |  0.99 |    0.03 |    6 |         - |          NA |
| TimSort                  | 256  | PipeOrgan          |       591.3 ns |     10.22 ns |      5.35 ns |  0.11 |    0.00 |    2 |         - |          NA |
| PowerSort                | 256  | PipeOrgan          |       384.1 ns |      4.82 ns |      2.14 ns |  0.07 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 256  | PipeOrgan          |     2,071.5 ns |     42.15 ns |     15.03 ns |  0.39 |    0.01 |    5 |         - |          NA |
| SpinSort                 | 256  | PipeOrgan          |     1,581.7 ns |     32.73 ns |     11.67 ns |  0.30 |    0.01 |    4 |         - |          NA |
| Glidesort                | 256  | PipeOrgan          |       977.1 ns |     20.74 ns |      9.21 ns |  0.19 |    0.00 |    3 |         - |          NA |
| Driftsort                | 256  | PipeOrgan          |       384.4 ns |      9.98 ns |      5.22 ns |  0.07 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 256  | PipeOrgan          |     1,500.1 ns |    193.25 ns |     85.80 ns |  0.29 |    0.02 |    4 |         - |          NA |
| QuadSort                 | 256  | PipeOrgan          |       668.6 ns |     26.73 ns |     11.87 ns |  0.13 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 256  | PipeOrgan          |     1,944.5 ns |     31.91 ns |     11.38 ns |  0.37 |    0.01 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **256**  | **ManyDuplicates**     |    **12,219.7 ns** |    **341.93 ns** |    **178.84 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 256  | ManyDuplicates     |    11,644.2 ns |    334.37 ns |    174.88 ns |  0.95 |    0.02 |    4 |         - |          NA |
| BottomupMergeSort        | 256  | ManyDuplicates     |     8,865.8 ns |    356.31 ns |    186.36 ns |  0.73 |    0.02 |    4 |         - |          NA |
| StdStableSort            | 256  | ManyDuplicates     |     4,847.2 ns |    291.46 ns |    129.41 ns |  0.40 |    0.01 |    3 |         - |          NA |
| RotateMergeSort          | 256  | ManyDuplicates     |    18,339.4 ns |    572.28 ns |    299.31 ns |  1.50 |    0.03 |    5 |         - |          NA |
| RotateMergeSortRecursive | 256  | ManyDuplicates     |    22,454.3 ns |    434.96 ns |    193.12 ns |  1.84 |    0.03 |    6 |         - |          NA |
| SymMergeSort             | 256  | ManyDuplicates     |    12,012.5 ns |    502.10 ns |    262.61 ns |  0.98 |    0.02 |    4 |         - |          NA |
| BlockMergeSort           | 256  | ManyDuplicates     |     9,422.7 ns |    293.54 ns |    153.53 ns |  0.77 |    0.02 |    4 |         - |          NA |
| NaturalMergeSort         | 256  | ManyDuplicates     |     8,817.5 ns |    212.50 ns |     94.35 ns |  0.72 |    0.01 |    4 |         - |          NA |
| PatienceSort             | 256  | ManyDuplicates     |    11,124.3 ns |    236.11 ns |    104.83 ns |  0.91 |    0.01 |    4 |         - |          NA |
| DropMergeSort            | 256  | ManyDuplicates     |     8,150.3 ns |    373.13 ns |    195.15 ns |  0.67 |    0.02 |    4 |         - |          NA |
| TimSort                  | 256  | ManyDuplicates     |     9,685.0 ns |    372.27 ns |    194.71 ns |  0.79 |    0.02 |    4 |         - |          NA |
| PowerSort                | 256  | ManyDuplicates     |     6,388.6 ns |    466.96 ns |    244.23 ns |  0.52 |    0.02 |    3 |         - |          NA |
| ShiftSort                | 256  | ManyDuplicates     |     9,761.8 ns |    322.19 ns |    168.51 ns |  0.80 |    0.02 |    4 |         - |          NA |
| SpinSort                 | 256  | ManyDuplicates     |     5,525.8 ns |    417.06 ns |    218.13 ns |  0.45 |    0.02 |    3 |         - |          NA |
| Glidesort                | 256  | ManyDuplicates     |     5,382.0 ns |    231.89 ns |    121.28 ns |  0.44 |    0.01 |    3 |         - |          NA |
| Driftsort                | 256  | ManyDuplicates     |     4,758.7 ns |    243.86 ns |    127.54 ns |  0.39 |    0.01 |    3 |         - |          NA |
| FlatStableSort           | 256  | ManyDuplicates     |     6,461.6 ns |    282.20 ns |    147.60 ns |  0.53 |    0.01 |    3 |         - |          NA |
| QuadSort                 | 256  | ManyDuplicates     |     3,561.9 ns |    291.80 ns |    152.62 ns |  0.29 |    0.01 |    2 |         - |          NA |
| SimdMergeSort            | 256  | ManyDuplicates     |     2,707.5 ns |     32.46 ns |     11.58 ns |  0.22 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **Random**             |    **58,556.9 ns** |    **491.74 ns** |    **257.19 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Random             |    55,793.3 ns |    218.20 ns |     96.88 ns |  0.95 |    0.00 |    4 |         - |          NA |
| BottomupMergeSort        | 1024 | Random             |    42,873.3 ns |    294.54 ns |    154.05 ns |  0.73 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 1024 | Random             |    30,191.0 ns |  1,677.86 ns |    877.55 ns |  0.52 |    0.01 |    3 |         - |          NA |
| RotateMergeSort          | 1024 | Random             |   108,953.9 ns |    905.27 ns |    473.47 ns |  1.86 |    0.01 |    5 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Random             |   127,719.3 ns |    912.39 ns |    477.20 ns |  2.18 |    0.01 |    5 |         - |          NA |
| SymMergeSort             | 1024 | Random             |    72,816.8 ns |    660.88 ns |    345.65 ns |  1.24 |    0.01 |    4 |         - |          NA |
| BlockMergeSort           | 1024 | Random             |    49,475.7 ns |    224.14 ns |    117.23 ns |  0.84 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 1024 | Random             |    44,426.1 ns |    154.47 ns |     80.79 ns |  0.76 |    0.00 |    4 |         - |          NA |
| PatienceSort             | 1024 | Random             |    64,337.1 ns |    294.94 ns |    130.95 ns |  1.10 |    0.00 |    4 |         - |          NA |
| DropMergeSort            | 1024 | Random             |    43,943.0 ns |    441.37 ns |    230.84 ns |  0.75 |    0.00 |    4 |         - |          NA |
| TimSort                  | 1024 | Random             |    50,118.8 ns |    398.55 ns |    208.45 ns |  0.86 |    0.00 |    4 |         - |          NA |
| PowerSort                | 1024 | Random             |    34,952.0 ns |    452.08 ns |    236.45 ns |  0.60 |    0.00 |    3 |         - |          NA |
| ShiftSort                | 1024 | Random             |    50,559.6 ns |    337.50 ns |    176.52 ns |  0.86 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 1024 | Random             |    31,434.6 ns |    221.92 ns |     98.54 ns |  0.54 |    0.00 |    3 |         - |          NA |
| Glidesort                | 1024 | Random             |    24,401.9 ns |    278.40 ns |    145.61 ns |  0.42 |    0.00 |    2 |         - |          NA |
| Driftsort                | 1024 | Random             |    21,106.8 ns |    290.40 ns |    151.89 ns |  0.36 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 1024 | Random             |    34,289.2 ns |    581.70 ns |    304.24 ns |  0.59 |    0.01 |    3 |         - |          NA |
| QuadSort                 | 1024 | Random             |    17,792.0 ns |    113.69 ns |     59.46 ns |  0.30 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 1024 | Random             |    13,037.9 ns |    127.57 ns |     66.72 ns |  0.22 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **SingleElementMoved** |    **13,967.4 ns** |    **188.19 ns** |     **83.56 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | SingleElementMoved |    16,856.9 ns |     94.54 ns |     41.98 ns |  1.21 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 1024 | SingleElementMoved |     5,055.3 ns |     43.09 ns |     19.13 ns |  0.36 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 1024 | SingleElementMoved |     2,738.1 ns |    340.67 ns |    178.18 ns |  0.20 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | SingleElementMoved |     1,609.2 ns |    314.91 ns |    164.71 ns |  0.12 |    0.01 |    3 |         - |          NA |
| RotateMergeSortRecursive | 1024 | SingleElementMoved |     2,150.8 ns |     31.01 ns |     13.77 ns |  0.15 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 1024 | SingleElementMoved |     1,228.3 ns |     66.00 ns |     23.54 ns |  0.09 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 1024 | SingleElementMoved |     9,623.5 ns |    217.14 ns |    113.57 ns |  0.69 |    0.01 |    7 |         - |          NA |
| NaturalMergeSort         | 1024 | SingleElementMoved |     1,038.0 ns |      4.37 ns |      1.56 ns |  0.07 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 1024 | SingleElementMoved |    38,224.8 ns |    930.19 ns |    486.51 ns |  2.74 |    0.04 |   10 |         - |          NA |
| DropMergeSort            | 1024 | SingleElementMoved |     1,911.3 ns |     48.69 ns |     21.62 ns |  0.14 |    0.00 |    4 |         - |          NA |
| TimSort                  | 1024 | SingleElementMoved |       758.6 ns |     30.38 ns |     15.89 ns |  0.05 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | SingleElementMoved |     1,066.3 ns |     11.68 ns |      6.11 ns |  0.08 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 1024 | SingleElementMoved |     1,218.9 ns |     29.18 ns |     12.95 ns |  0.09 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 1024 | SingleElementMoved |     2,966.6 ns |    312.50 ns |    163.45 ns |  0.21 |    0.01 |    5 |         - |          NA |
| Glidesort                | 1024 | SingleElementMoved |     1,854.7 ns |     27.00 ns |     11.99 ns |  0.13 |    0.00 |    4 |         - |          NA |
| Driftsort                | 1024 | SingleElementMoved |     1,070.7 ns |     12.77 ns |      5.67 ns |  0.08 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 1024 | SingleElementMoved |     4,594.6 ns |    295.47 ns |    154.53 ns |  0.33 |    0.01 |    6 |         - |          NA |
| QuadSort                 | 1024 | SingleElementMoved |     1,209.7 ns |     36.04 ns |     16.00 ns |  0.09 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 1024 | SingleElementMoved |    12,945.8 ns |    227.63 ns |    119.06 ns |  0.93 |    0.01 |    8 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **Sorted**             |    **13,404.8 ns** |    **198.84 ns** |    **104.00 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Sorted             |    16,333.6 ns |     59.89 ns |     31.32 ns |  1.22 |    0.01 |    8 |         - |          NA |
| BottomupMergeSort        | 1024 | Sorted             |     4,762.0 ns |    234.72 ns |    104.22 ns |  0.36 |    0.01 |    6 |         - |          NA |
| StdStableSort            | 1024 | Sorted             |     2,494.2 ns |    290.35 ns |    151.86 ns |  0.19 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | Sorted             |       931.2 ns |      2.54 ns |      1.33 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Sorted             |     1,532.6 ns |     10.19 ns |      3.63 ns |  0.11 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 1024 | Sorted             |       905.9 ns |     10.28 ns |      4.56 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Sorted             |     8,513.8 ns |    338.07 ns |    176.82 ns |  0.64 |    0.01 |    7 |         - |          NA |
| NaturalMergeSort         | 1024 | Sorted             |       526.9 ns |     47.61 ns |     21.14 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 1024 | Sorted             |    31,519.8 ns |    779.13 ns |    407.50 ns |  2.35 |    0.03 |    9 |         - |          NA |
| DropMergeSort            | 1024 | Sorted             |     1,715.9 ns |     16.37 ns |      7.27 ns |  0.13 |    0.00 |    4 |         - |          NA |
| TimSort                  | 1024 | Sorted             |       484.8 ns |      3.81 ns |      1.99 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Sorted             |       469.0 ns |      8.50 ns |      3.77 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Sorted             |       644.7 ns |     21.00 ns |      9.32 ns |  0.05 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 1024 | Sorted             |       422.1 ns |     18.56 ns |      8.24 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Sorted             |       463.9 ns |     41.85 ns |     18.58 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Sorted             |       519.8 ns |     28.74 ns |     12.76 ns |  0.04 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Sorted             |     4,067.0 ns |    148.81 ns |     66.07 ns |  0.30 |    0.01 |    6 |         - |          NA |
| QuadSort                 | 1024 | Sorted             |       679.6 ns |     20.87 ns |     10.92 ns |  0.05 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 1024 | Sorted             |    15,716.6 ns |    178.87 ns |     93.55 ns |  1.17 |    0.01 |    8 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **Reversed**           |    **29,036.6 ns** |    **194.39 ns** |    **101.67 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | Reversed           |    25,662.5 ns |    318.11 ns |    166.38 ns |  0.88 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 1024 | Reversed           |    15,772.7 ns |    151.63 ns |     67.32 ns |  0.54 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 1024 | Reversed           |    13,744.0 ns |    277.04 ns |    144.90 ns |  0.47 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 1024 | Reversed           |    20,484.1 ns |    253.57 ns |    132.62 ns |  0.71 |    0.00 |    5 |         - |          NA |
| RotateMergeSortRecursive | 1024 | Reversed           |    27,947.2 ns |    162.53 ns |     85.01 ns |  0.96 |    0.00 |    6 |         - |          NA |
| SymMergeSort             | 1024 | Reversed           |     7,642.5 ns |    327.98 ns |    171.54 ns |  0.26 |    0.01 |    3 |         - |          NA |
| BlockMergeSort           | 1024 | Reversed           |    11,654.8 ns |    294.54 ns |    154.05 ns |  0.40 |    0.01 |    4 |         - |          NA |
| NaturalMergeSort         | 1024 | Reversed           |       760.7 ns |      8.50 ns |      3.78 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 1024 | Reversed           |     4,531.4 ns |    320.67 ns |    167.72 ns |  0.16 |    0.01 |    2 |         - |          NA |
| DropMergeSort            | 1024 | Reversed           |     5,373.5 ns |     57.65 ns |     25.60 ns |  0.19 |    0.00 |    2 |         - |          NA |
| TimSort                  | 1024 | Reversed           |       732.7 ns |      5.60 ns |      2.49 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 1024 | Reversed           |       741.3 ns |    121.59 ns |     53.99 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | Reversed           |    14,597.0 ns |    231.37 ns |    121.01 ns |  0.50 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 1024 | Reversed           |       761.6 ns |      4.24 ns |      2.22 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 1024 | Reversed           |       781.3 ns |     10.34 ns |      4.59 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 1024 | Reversed           |       758.7 ns |      4.17 ns |      1.85 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | Reversed           |     7,000.0 ns |    230.77 ns |    120.69 ns |  0.24 |    0.00 |    3 |         - |          NA |
| QuadSort                 | 1024 | Reversed           |       767.3 ns |      8.90 ns |      3.95 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SimdMergeSort            | 1024 | Reversed           |    12,634.3 ns |    181.49 ns |     94.92 ns |  0.44 |    0.00 |    4 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **PipeOrgan**          |    **21,458.1 ns** |    **177.77 ns** |     **92.98 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | PipeOrgan          |    21,430.3 ns |    517.17 ns |    229.63 ns |  1.00 |    0.01 |    6 |         - |          NA |
| BottomupMergeSort        | 1024 | PipeOrgan          |    10,336.4 ns |    301.74 ns |    157.82 ns |  0.48 |    0.01 |    5 |         - |          NA |
| StdStableSort            | 1024 | PipeOrgan          |     8,433.9 ns |    307.02 ns |    160.58 ns |  0.39 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 1024 | PipeOrgan          |    20,286.5 ns |  2,207.09 ns |  1,154.35 ns |  0.95 |    0.05 |    6 |         - |          NA |
| RotateMergeSortRecursive | 1024 | PipeOrgan          |    25,264.6 ns |    365.92 ns |    191.39 ns |  1.18 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 1024 | PipeOrgan          |    11,619.9 ns |    582.64 ns |    304.73 ns |  0.54 |    0.01 |    5 |         - |          NA |
| BlockMergeSort           | 1024 | PipeOrgan          |    12,026.5 ns |    294.46 ns |    154.01 ns |  0.56 |    0.01 |    5 |         - |          NA |
| NaturalMergeSort         | 1024 | PipeOrgan          |     1,393.0 ns |     19.74 ns |      8.77 ns |  0.06 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 1024 | PipeOrgan          |    22,286.4 ns |    445.65 ns |    158.92 ns |  1.04 |    0.01 |    6 |         - |          NA |
| DropMergeSort            | 1024 | PipeOrgan          |    21,555.5 ns |    239.18 ns |    125.09 ns |  1.00 |    0.01 |    6 |         - |          NA |
| TimSort                  | 1024 | PipeOrgan          |     2,101.3 ns |      8.76 ns |      4.58 ns |  0.10 |    0.00 |    2 |         - |          NA |
| PowerSort                | 1024 | PipeOrgan          |     1,363.0 ns |      9.26 ns |      3.30 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 1024 | PipeOrgan          |     9,986.5 ns |  1,621.81 ns |    720.09 ns |  0.47 |    0.03 |    5 |         - |          NA |
| SpinSort                 | 1024 | PipeOrgan          |     7,007.6 ns |    257.57 ns |    134.72 ns |  0.33 |    0.01 |    4 |         - |          NA |
| Glidesort                | 1024 | PipeOrgan          |     3,709.4 ns |      8.79 ns |      3.13 ns |  0.17 |    0.00 |    3 |         - |          NA |
| Driftsort                | 1024 | PipeOrgan          |     1,199.9 ns |     41.32 ns |     14.73 ns |  0.06 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | PipeOrgan          |     6,362.1 ns |    274.53 ns |    143.58 ns |  0.30 |    0.01 |    4 |         - |          NA |
| QuadSort                 | 1024 | PipeOrgan          |     2,401.1 ns |     10.79 ns |      4.79 ns |  0.11 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 1024 | PipeOrgan          |    12,913.9 ns |    189.11 ns |     98.91 ns |  0.60 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **1024** | **ManyDuplicates**     |    **52,199.4 ns** |    **359.22 ns** |    **187.88 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 1024 | ManyDuplicates     |    49,396.3 ns |    297.66 ns |    132.16 ns |  0.95 |    0.00 |    4 |         - |          NA |
| BottomupMergeSort        | 1024 | ManyDuplicates     |    37,405.2 ns |    370.20 ns |    193.62 ns |  0.72 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 1024 | ManyDuplicates     |    23,040.2 ns |  1,081.84 ns |    480.34 ns |  0.44 |    0.01 |    3 |         - |          NA |
| RotateMergeSort          | 1024 | ManyDuplicates     |    95,670.8 ns |  1,026.67 ns |    536.97 ns |  1.83 |    0.01 |    6 |         - |          NA |
| RotateMergeSortRecursive | 1024 | ManyDuplicates     |   112,682.6 ns |    361.66 ns |    160.58 ns |  2.16 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 1024 | ManyDuplicates     |    62,879.9 ns |    358.69 ns |    159.26 ns |  1.20 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 1024 | ManyDuplicates     |    46,443.8 ns |    190.63 ns |     99.71 ns |  0.89 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 1024 | ManyDuplicates     |    38,614.5 ns |    276.81 ns |    144.77 ns |  0.74 |    0.00 |    4 |         - |          NA |
| PatienceSort             | 1024 | ManyDuplicates     |    40,891.1 ns |    510.34 ns |    266.92 ns |  0.78 |    0.01 |    4 |         - |          NA |
| DropMergeSort            | 1024 | ManyDuplicates     |    31,021.5 ns |    234.17 ns |    103.98 ns |  0.59 |    0.00 |    3 |         - |          NA |
| TimSort                  | 1024 | ManyDuplicates     |    43,653.3 ns |    207.46 ns |     92.11 ns |  0.84 |    0.00 |    4 |         - |          NA |
| PowerSort                | 1024 | ManyDuplicates     |    28,258.7 ns |    515.35 ns |    269.54 ns |  0.54 |    0.01 |    3 |         - |          NA |
| ShiftSort                | 1024 | ManyDuplicates     |    43,167.9 ns |    339.65 ns |    177.64 ns |  0.83 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 1024 | ManyDuplicates     |    25,265.4 ns |    141.19 ns |     73.85 ns |  0.48 |    0.00 |    3 |         - |          NA |
| Glidesort                | 1024 | ManyDuplicates     |    14,091.4 ns |    109.57 ns |     48.65 ns |  0.27 |    0.00 |    2 |         - |          NA |
| Driftsort                | 1024 | ManyDuplicates     |    11,621.3 ns |    462.03 ns |    241.65 ns |  0.22 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 1024 | ManyDuplicates     |    28,683.2 ns |    460.72 ns |    240.96 ns |  0.55 |    0.00 |    3 |         - |          NA |
| QuadSort                 | 1024 | ManyDuplicates     |    17,811.6 ns |    247.47 ns |    129.43 ns |  0.34 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 1024 | ManyDuplicates     |    15,964.0 ns |    132.86 ns |     69.49 ns |  0.31 |    0.00 |    2 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **Random**             |   **266,578.9 ns** |  **1,226.73 ns** |    **641.60 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Random             |   252,759.0 ns |  3,234.28 ns |  1,691.59 ns |  0.95 |    0.01 |    3 |         - |          NA |
| BottomupMergeSort        | 4096 | Random             |   198,022.1 ns |  1,181.59 ns |    524.63 ns |  0.74 |    0.00 |    3 |         - |          NA |
| StdStableSort            | 4096 | Random             |   148,526.0 ns |  2,570.69 ns |  1,344.52 ns |  0.56 |    0.00 |    3 |         - |          NA |
| RotateMergeSort          | 4096 | Random             |   570,392.1 ns |  1,343.79 ns |    596.65 ns |  2.14 |    0.01 |    5 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Random             |   663,809.0 ns |    290.44 ns |    103.57 ns |  2.49 |    0.01 |    5 |         - |          NA |
| SymMergeSort             | 4096 | Random             |   385,775.1 ns |  1,178.62 ns |    523.31 ns |  1.45 |    0.00 |    4 |         - |          NA |
| BlockMergeSort           | 4096 | Random             |   243,096.9 ns |    934.03 ns |    488.51 ns |  0.91 |    0.00 |    3 |         - |          NA |
| NaturalMergeSort         | 4096 | Random             |   208,162.9 ns |  1,256.54 ns |    557.91 ns |  0.78 |    0.00 |    3 |         - |          NA |
| PatienceSort             | 4096 | Random             |   303,583.4 ns |    914.55 ns |    478.33 ns |  1.14 |    0.00 |    3 |         - |          NA |
| DropMergeSort            | 4096 | Random             |   198,246.3 ns |  1,119.60 ns |    497.11 ns |  0.74 |    0.00 |    3 |         - |          NA |
| TimSort                  | 4096 | Random             |   234,660.2 ns |  1,392.94 ns |    728.53 ns |  0.88 |    0.00 |    3 |         - |          NA |
| PowerSort                | 4096 | Random             |   174,732.0 ns |  1,044.24 ns |    463.65 ns |  0.66 |    0.00 |    3 |         - |          NA |
| ShiftSort                | 4096 | Random             |   241,333.2 ns |    382.39 ns |    169.79 ns |  0.91 |    0.00 |    3 |         - |          NA |
| SpinSort                 | 4096 | Random             |   156,399.5 ns |  1,226.58 ns |    641.52 ns |  0.59 |    0.00 |    3 |         - |          NA |
| Glidesort                | 4096 | Random             |    99,937.1 ns |    391.34 ns |    173.76 ns |  0.37 |    0.00 |    2 |         - |          NA |
| Driftsort                | 4096 | Random             |    85,732.6 ns |    627.75 ns |    278.73 ns |  0.32 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 4096 | Random             |   162,983.3 ns |    690.29 ns |    361.03 ns |  0.61 |    0.00 |    3 |         - |          NA |
| QuadSort                 | 4096 | Random             |    82,654.3 ns |    263.71 ns |     94.04 ns |  0.31 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 4096 | Random             |    67,017.9 ns |    140.65 ns |     73.56 ns |  0.25 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **SingleElementMoved** |    **55,199.1 ns** |    **303.34 ns** |    **158.65 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | SingleElementMoved |    67,022.4 ns |    848.16 ns |    443.60 ns |  1.21 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 4096 | SingleElementMoved |    20,093.0 ns |    244.91 ns |    128.09 ns |  0.36 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 4096 | SingleElementMoved |    12,848.2 ns |    452.89 ns |    201.09 ns |  0.23 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 4096 | SingleElementMoved |     5,037.2 ns |    223.48 ns |     99.23 ns |  0.09 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 4096 | SingleElementMoved |     7,496.5 ns |    293.11 ns |    153.30 ns |  0.14 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 4096 | SingleElementMoved |     4,461.7 ns |    360.83 ns |    188.72 ns |  0.08 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 4096 | SingleElementMoved |    38,678.7 ns |    275.05 ns |    143.86 ns |  0.70 |    0.00 |    7 |         - |          NA |
| NaturalMergeSort         | 4096 | SingleElementMoved |     3,865.9 ns |    306.31 ns |    136.00 ns |  0.07 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 4096 | SingleElementMoved |   261,218.6 ns |  5,412.89 ns |  2,831.04 ns |  4.73 |    0.05 |   11 |         - |          NA |
| DropMergeSort            | 4096 | SingleElementMoved |     7,421.6 ns |    311.40 ns |    162.87 ns |  0.13 |    0.00 |    3 |         - |          NA |
| TimSort                  | 4096 | SingleElementMoved |     2,455.9 ns |     34.42 ns |     12.28 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | SingleElementMoved |     3,964.3 ns |    269.62 ns |    141.02 ns |  0.07 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 4096 | SingleElementMoved |     4,523.9 ns |    294.68 ns |    154.12 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 4096 | SingleElementMoved |     8,521.0 ns |    394.33 ns |    206.24 ns |  0.15 |    0.00 |    3 |         - |          NA |
| Glidesort                | 4096 | SingleElementMoved |     6,827.8 ns |    211.54 ns |    110.64 ns |  0.12 |    0.00 |    3 |         - |          NA |
| Driftsort                | 4096 | SingleElementMoved |     3,819.6 ns |    360.35 ns |    188.47 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 4096 | SingleElementMoved |    16,570.4 ns |    117.13 ns |     41.77 ns |  0.30 |    0.00 |    5 |         - |          NA |
| QuadSort                 | 4096 | SingleElementMoved |     4,124.8 ns |  1,108.52 ns |    492.19 ns |  0.07 |    0.01 |    2 |         - |          NA |
| SimdMergeSort            | 4096 | SingleElementMoved |    86,941.6 ns |    499.12 ns |    261.05 ns |  1.58 |    0.01 |   10 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **Sorted**             |    **53,151.7 ns** |     **89.42 ns** |     **31.89 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Sorted             |    69,142.1 ns | 18,422.81 ns |  8,179.84 ns |  1.30 |    0.14 |    9 |         - |          NA |
| BottomupMergeSort        | 4096 | Sorted             |    18,537.0 ns |    122.65 ns |     64.15 ns |  0.35 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 4096 | Sorted             |    12,186.0 ns |    213.05 ns |    111.43 ns |  0.23 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 4096 | Sorted             |     3,702.1 ns |     21.61 ns |      7.71 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Sorted             |     6,265.7 ns |    318.79 ns |    166.74 ns |  0.12 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 4096 | Sorted             |     3,719.5 ns |    327.25 ns |    171.16 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 4096 | Sorted             |    33,753.4 ns |    152.15 ns |     67.56 ns |  0.64 |    0.00 |    7 |         - |          NA |
| NaturalMergeSort         | 4096 | Sorted             |     1,857.1 ns |      8.85 ns |      4.63 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 4096 | Sorted             |   185,666.6 ns | 22,503.97 ns | 11,770.01 ns |  3.49 |    0.21 |   11 |         - |          NA |
| DropMergeSort            | 4096 | Sorted             |     6,852.5 ns |    300.83 ns |    157.34 ns |  0.13 |    0.00 |    4 |         - |          NA |
| TimSort                  | 4096 | Sorted             |     1,816.5 ns |     47.06 ns |     24.62 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Sorted             |     1,806.5 ns |      8.16 ns |      3.62 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Sorted             |     2,314.2 ns |     10.85 ns |      4.82 ns |  0.04 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 4096 | Sorted             |     1,564.5 ns |     32.20 ns |     14.29 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | Sorted             |     1,528.5 ns |     14.36 ns |      6.37 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Sorted             |     1,852.0 ns |     26.21 ns |     11.64 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Sorted             |    14,541.1 ns |    124.22 ns |     64.97 ns |  0.27 |    0.00 |    5 |         - |          NA |
| QuadSort                 | 4096 | Sorted             |     2,390.5 ns |     37.11 ns |     16.48 ns |  0.04 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 4096 | Sorted             |    84,474.8 ns |    427.46 ns |    189.79 ns |  1.59 |    0.00 |   10 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **Reversed**           |   **119,261.0 ns** |    **975.73 ns** |    **433.23 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | Reversed           |   104,195.1 ns |    588.54 ns |    261.32 ns |  0.87 |    0.00 |    6 |         - |          NA |
| BottomupMergeSort        | 4096 | Reversed           |    64,078.4 ns |    182.62 ns |     81.08 ns |  0.54 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 4096 | Reversed           |    58,010.5 ns |    228.30 ns |    101.36 ns |  0.49 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 4096 | Reversed           |    93,685.6 ns |    742.58 ns |    329.71 ns |  0.79 |    0.00 |    6 |         - |          NA |
| RotateMergeSortRecursive | 4096 | Reversed           |   124,164.5 ns |  1,508.56 ns |    669.81 ns |  1.04 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 4096 | Reversed           |    32,399.5 ns |    291.61 ns |    152.52 ns |  0.27 |    0.00 |    4 |         - |          NA |
| BlockMergeSort           | 4096 | Reversed           |    52,256.0 ns |    319.56 ns |    141.89 ns |  0.44 |    0.00 |    5 |         - |          NA |
| NaturalMergeSort         | 4096 | Reversed           |     2,850.4 ns |     24.28 ns |     10.78 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 4096 | Reversed           |    16,669.1 ns |    121.91 ns |     63.76 ns |  0.14 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 4096 | Reversed           |    24,043.1 ns |    217.42 ns |     96.54 ns |  0.20 |    0.00 |    3 |         - |          NA |
| TimSort                  | 4096 | Reversed           |     2,814.4 ns |     32.82 ns |     11.70 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 4096 | Reversed           |     2,766.5 ns |     24.38 ns |      8.69 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | Reversed           |    64,933.5 ns |    134.84 ns |     70.53 ns |  0.54 |    0.00 |    5 |         - |          NA |
| SpinSort                 | 4096 | Reversed           |     2,960.1 ns |     26.72 ns |     11.86 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Glidesort                | 4096 | Reversed           |     2,842.3 ns |      5.58 ns |      1.99 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 4096 | Reversed           |     2,848.8 ns |     16.29 ns |      5.81 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | Reversed           |    31,453.3 ns |    302.51 ns |    134.31 ns |  0.26 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 4096 | Reversed           |     3,088.4 ns |    279.21 ns |    146.03 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SimdMergeSort            | 4096 | Reversed           |    84,578.9 ns |    214.36 ns |     76.44 ns |  0.71 |    0.00 |    6 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **PipeOrgan**          |    **87,409.6 ns** |    **331.72 ns** |    **147.29 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | PipeOrgan          |    86,072.9 ns |    329.78 ns |    146.43 ns |  0.98 |    0.00 |    8 |         - |          NA |
| BottomupMergeSort        | 4096 | PipeOrgan          |    42,444.0 ns |    109.35 ns |     39.00 ns |  0.49 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 4096 | PipeOrgan          |    36,402.2 ns |    516.10 ns |    229.15 ns |  0.42 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 4096 | PipeOrgan          |    88,525.1 ns |  1,085.90 ns |    482.14 ns |  1.01 |    0.01 |    8 |         - |          NA |
| RotateMergeSortRecursive | 4096 | PipeOrgan          |   108,331.4 ns |    464.16 ns |    206.09 ns |  1.24 |    0.00 |    9 |         - |          NA |
| SymMergeSort             | 4096 | PipeOrgan          |    48,354.4 ns |    643.21 ns |    336.41 ns |  0.55 |    0.00 |    6 |         - |          NA |
| BlockMergeSort           | 4096 | PipeOrgan          |    48,968.6 ns |     90.71 ns |     40.28 ns |  0.56 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 4096 | PipeOrgan          |     8,262.5 ns |  1,982.47 ns |    880.23 ns |  0.09 |    0.01 |    3 |         - |          NA |
| PatienceSort             | 4096 | PipeOrgan          |   135,811.9 ns | 11,489.69 ns |  6,009.33 ns |  1.55 |    0.06 |   10 |         - |          NA |
| DropMergeSort            | 4096 | PipeOrgan          |    90,061.9 ns |  1,227.73 ns |    642.12 ns |  1.03 |    0.01 |    8 |         - |          NA |
| TimSort                  | 4096 | PipeOrgan          |     8,236.0 ns |    373.26 ns |    195.22 ns |  0.09 |    0.00 |    3 |         - |          NA |
| PowerSort                | 4096 | PipeOrgan          |     5,278.4 ns |    262.84 ns |    137.47 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 4096 | PipeOrgan          |    40,713.3 ns |    185.33 ns |     96.93 ns |  0.47 |    0.00 |    6 |         - |          NA |
| SpinSort                 | 4096 | PipeOrgan          |     6,693.8 ns |    360.70 ns |    188.65 ns |  0.08 |    0.00 |    2 |         - |          NA |
| Glidesort                | 4096 | PipeOrgan          |    14,762.8 ns |     66.08 ns |     34.56 ns |  0.17 |    0.00 |    4 |         - |          NA |
| Driftsort                | 4096 | PipeOrgan          |     4,495.7 ns |    150.89 ns |     66.99 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | PipeOrgan          |    28,396.8 ns |    169.51 ns |     88.66 ns |  0.32 |    0.00 |    5 |         - |          NA |
| QuadSort                 | 4096 | PipeOrgan          |     9,242.6 ns |    298.62 ns |    156.19 ns |  0.11 |    0.00 |    3 |         - |          NA |
| SimdMergeSort            | 4096 | PipeOrgan          |    67,842.7 ns |    135.23 ns |     60.04 ns |  0.78 |    0.00 |    7 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **4096** | **ManyDuplicates**     |   **216,020.9 ns** |  **1,244.81 ns** |    **651.06 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 4096 | ManyDuplicates     |   202,876.5 ns |  1,114.50 ns |    582.91 ns |  0.94 |    0.00 |    4 |         - |          NA |
| BottomupMergeSort        | 4096 | ManyDuplicates     |   152,825.3 ns |  1,015.18 ns |    450.75 ns |  0.71 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 4096 | ManyDuplicates     |   100,501.5 ns |  2,650.92 ns |  1,386.48 ns |  0.47 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 4096 | ManyDuplicates     |   427,545.1 ns |  1,843.23 ns |    818.40 ns |  1.98 |    0.01 |    6 |         - |          NA |
| RotateMergeSortRecursive | 4096 | ManyDuplicates     |   503,627.0 ns |  1,831.98 ns |    813.41 ns |  2.33 |    0.01 |    6 |         - |          NA |
| SymMergeSort             | 4096 | ManyDuplicates     |   288,204.1 ns |  1,391.93 ns |    728.01 ns |  1.33 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 4096 | ManyDuplicates     |   213,568.0 ns |  1,175.95 ns |    522.13 ns |  0.99 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 4096 | ManyDuplicates     |   158,630.2 ns |    898.90 ns |    470.14 ns |  0.73 |    0.00 |    4 |         - |          NA |
| PatienceSort             | 4096 | ManyDuplicates     |   139,920.5 ns |  1,259.19 ns |    559.09 ns |  0.65 |    0.00 |    4 |         - |          NA |
| DropMergeSort            | 4096 | ManyDuplicates     |   117,662.4 ns |    415.81 ns |    184.62 ns |  0.54 |    0.00 |    4 |         - |          NA |
| TimSort                  | 4096 | ManyDuplicates     |   183,599.6 ns |    531.76 ns |    236.11 ns |  0.85 |    0.00 |    4 |         - |          NA |
| PowerSort                | 4096 | ManyDuplicates     |   120,371.6 ns |    501.82 ns |    262.46 ns |  0.56 |    0.00 |    4 |         - |          NA |
| ShiftSort                | 4096 | ManyDuplicates     |   184,240.0 ns |    945.42 ns |    494.47 ns |  0.85 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 4096 | ManyDuplicates     |   108,365.6 ns |    655.11 ns |    342.64 ns |  0.50 |    0.00 |    4 |         - |          NA |
| Glidesort                | 4096 | ManyDuplicates     |    21,819.0 ns |    209.20 ns |     92.89 ns |  0.10 |    0.00 |    2 |         - |          NA |
| Driftsort                | 4096 | ManyDuplicates     |    16,946.3 ns |    197.84 ns |     87.84 ns |  0.08 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 4096 | ManyDuplicates     |   116,336.9 ns |  1,100.82 ns |    488.77 ns |  0.54 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 4096 | ManyDuplicates     |    76,919.6 ns |    596.69 ns |    264.94 ns |  0.36 |    0.00 |    3 |         - |          NA |
| SimdMergeSort            | 4096 | ManyDuplicates     |    67,941.0 ns |    330.84 ns |    146.89 ns |  0.31 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **Random**             |   **563,321.7 ns** |  **1,735.98 ns** |    **770.79 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Random             |   535,928.8 ns |  8,905.05 ns |  3,953.90 ns |  0.95 |    0.01 |    2 |         - |          NA |
| BottomupMergeSort        | 8192 | Random             |   422,709.3 ns |    897.45 ns |    398.48 ns |  0.75 |    0.00 |    2 |         - |          NA |
| StdStableSort            | 8192 | Random             |   341,600.5 ns |  7,538.31 ns |  3,942.68 ns |  0.61 |    0.01 |    2 |         - |          NA |
| RotateMergeSort          | 8192 | Random             | 1,287,974.8 ns |  6,783.59 ns |  3,547.95 ns |  2.29 |    0.01 |    4 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Random             | 1,487,583.6 ns |  2,091.34 ns |    928.57 ns |  2.64 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 8192 | Random             |   869,783.5 ns |  3,308.50 ns |  1,469.00 ns |  1.54 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | Random             |   525,750.6 ns |  1,289.59 ns |    572.59 ns |  0.93 |    0.00 |    2 |         - |          NA |
| NaturalMergeSort         | 8192 | Random             |   445,573.4 ns |  2,047.91 ns |  1,071.10 ns |  0.79 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 8192 | Random             |   657,637.5 ns |  2,511.50 ns |  1,313.57 ns |  1.17 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 8192 | Random             |   421,071.6 ns |  3,295.31 ns |  1,723.51 ns |  0.75 |    0.00 |    2 |         - |          NA |
| TimSort                  | 8192 | Random             |   514,683.5 ns |  2,669.49 ns |  1,185.27 ns |  0.91 |    0.00 |    2 |         - |          NA |
| PowerSort                | 8192 | Random             |   383,625.7 ns |  1,493.32 ns |    781.03 ns |  0.68 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | Random             |   529,521.6 ns |  1,564.10 ns |    694.47 ns |  0.94 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | Random             |   343,073.2 ns |  1,290.76 ns |    675.09 ns |  0.61 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | Random             |   202,041.5 ns |  1,233.37 ns |    547.62 ns |  0.36 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Random             |   194,373.4 ns | 58,425.98 ns | 30,557.92 ns |  0.35 |    0.05 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Random             |   369,774.0 ns |  1,399.25 ns |    731.83 ns |  0.66 |    0.00 |    2 |         - |          NA |
| QuadSort                 | 8192 | Random             |   176,745.2 ns |    632.90 ns |    281.01 ns |  0.31 |    0.00 |    1 |         - |          NA |
| SimdMergeSort            | 8192 | Random             |   190,835.2 ns |    514.27 ns |    228.34 ns |  0.34 |    0.00 |    1 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **SingleElementMoved** |   **110,124.8 ns** |    **800.65 ns** |    **355.50 ns** |  **1.00** |    **0.00** |    **7** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | SingleElementMoved |   134,716.0 ns |    793.29 ns |    352.22 ns |  1.22 |    0.00 |    8 |         - |          NA |
| BottomupMergeSort        | 8192 | SingleElementMoved |    42,187.9 ns |    562.92 ns |    200.74 ns |  0.38 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | SingleElementMoved |    24,301.3 ns |    598.04 ns |    265.53 ns |  0.22 |    0.00 |    4 |         - |          NA |
| RotateMergeSort          | 8192 | SingleElementMoved |     9,618.4 ns |    391.53 ns |    204.78 ns |  0.09 |    0.00 |    2 |         - |          NA |
| RotateMergeSortRecursive | 8192 | SingleElementMoved |    14,391.9 ns |     98.42 ns |     43.70 ns |  0.13 |    0.00 |    3 |         - |          NA |
| SymMergeSort             | 8192 | SingleElementMoved |     8,677.8 ns |    138.36 ns |     61.43 ns |  0.08 |    0.00 |    2 |         - |          NA |
| BlockMergeSort           | 8192 | SingleElementMoved |    76,159.4 ns |    801.24 ns |    355.75 ns |  0.69 |    0.00 |    6 |         - |          NA |
| NaturalMergeSort         | 8192 | SingleElementMoved |     7,575.2 ns |    190.33 ns |     84.51 ns |  0.07 |    0.00 |    2 |         - |          NA |
| PatienceSort             | 8192 | SingleElementMoved |   623,229.9 ns |  4,802.68 ns |  2,511.90 ns |  5.66 |    0.03 |    9 |         - |          NA |
| DropMergeSort            | 8192 | SingleElementMoved |    14,796.4 ns |    130.34 ns |     68.17 ns |  0.13 |    0.00 |    3 |         - |          NA |
| TimSort                  | 8192 | SingleElementMoved |     4,883.3 ns |    390.91 ns |    173.57 ns |  0.04 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | SingleElementMoved |     7,612.5 ns |     22.57 ns |     10.02 ns |  0.07 |    0.00 |    2 |         - |          NA |
| ShiftSort                | 8192 | SingleElementMoved |     8,962.3 ns |    246.05 ns |    128.69 ns |  0.08 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | SingleElementMoved |    14,318.5 ns |    275.28 ns |    143.98 ns |  0.13 |    0.00 |    3 |         - |          NA |
| Glidesort                | 8192 | SingleElementMoved |    13,451.8 ns |    227.24 ns |    118.85 ns |  0.12 |    0.00 |    3 |         - |          NA |
| Driftsort                | 8192 | SingleElementMoved |     7,740.4 ns |    577.00 ns |    301.78 ns |  0.07 |    0.00 |    2 |         - |          NA |
| FlatStableSort           | 8192 | SingleElementMoved |    23,235.5 ns |    182.04 ns |     80.83 ns |  0.21 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 8192 | SingleElementMoved |     7,566.6 ns |    246.99 ns |    129.18 ns |  0.07 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 8192 | SingleElementMoved |   158,184.4 ns |    289.20 ns |    128.41 ns |  1.44 |    0.00 |    8 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **Sorted**             |   **106,235.0 ns** |  **1,112.31 ns** |    **493.87 ns** |  **1.00** |    **0.00** |    **8** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Sorted             |   131,914.2 ns |  1,023.55 ns |    535.34 ns |  1.24 |    0.01 |    9 |         - |          NA |
| BottomupMergeSort        | 8192 | Sorted             |    39,665.6 ns |    455.01 ns |    202.03 ns |  0.37 |    0.00 |    6 |         - |          NA |
| StdStableSort            | 8192 | Sorted             |    23,257.3 ns |    450.39 ns |    235.56 ns |  0.22 |    0.00 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | Sorted             |     7,770.1 ns |     65.23 ns |     34.12 ns |  0.07 |    0.00 |    3 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Sorted             |    12,559.3 ns |    403.07 ns |    210.81 ns |  0.12 |    0.00 |    4 |         - |          NA |
| SymMergeSort             | 8192 | Sorted             |     7,488.1 ns |     68.84 ns |     36.01 ns |  0.07 |    0.00 |    3 |         - |          NA |
| BlockMergeSort           | 8192 | Sorted             |    66,391.1 ns |    340.67 ns |    178.18 ns |  0.62 |    0.00 |    7 |         - |          NA |
| NaturalMergeSort         | 8192 | Sorted             |     3,606.9 ns |     42.10 ns |     18.69 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 8192 | Sorted             |   611,998.0 ns |  3,627.68 ns |  1,897.35 ns |  5.76 |    0.03 |   11 |         - |          NA |
| DropMergeSort            | 8192 | Sorted             |    13,687.1 ns |    324.44 ns |    144.05 ns |  0.13 |    0.00 |    4 |         - |          NA |
| TimSort                  | 8192 | Sorted             |     3,564.5 ns |     18.11 ns |      8.04 ns |  0.03 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Sorted             |     3,577.3 ns |     37.77 ns |     13.47 ns |  0.03 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Sorted             |     4,690.2 ns |    352.35 ns |    184.29 ns |  0.04 |    0.00 |    2 |         - |          NA |
| SpinSort                 | 8192 | Sorted             |     3,119.6 ns |     27.43 ns |     12.18 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Sorted             |     3,097.1 ns |    357.94 ns |    158.93 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Sorted             |     3,603.8 ns |     48.53 ns |     17.31 ns |  0.03 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Sorted             |     4,407.3 ns |  1,894.86 ns |    991.05 ns |  0.04 |    0.01 |    2 |         - |          NA |
| QuadSort                 | 8192 | Sorted             |     4,694.6 ns |     59.25 ns |     21.13 ns |  0.04 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 8192 | Sorted             |   191,600.6 ns |  1,264.53 ns |    661.38 ns |  1.80 |    0.01 |   10 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **Reversed**           |   **242,261.9 ns** |  **1,631.05 ns** |    **724.19 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | Reversed           |   211,784.7 ns |  1,115.13 ns |    495.13 ns |  0.87 |    0.00 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | Reversed           |   131,081.9 ns |  2,984.07 ns |  1,064.15 ns |  0.54 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | Reversed           |   207,107.4 ns |  1,205.36 ns |    535.19 ns |  0.85 |    0.00 |    6 |         - |          NA |
| RotateMergeSort          | 8192 | Reversed           |   199,240.2 ns |    746.03 ns |    331.24 ns |  0.82 |    0.00 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | Reversed           |   259,358.7 ns |  1,527.78 ns |    799.06 ns |  1.07 |    0.00 |    6 |         - |          NA |
| SymMergeSort             | 8192 | Reversed           |    67,198.8 ns |    347.10 ns |    181.54 ns |  0.28 |    0.00 |    4 |         - |          NA |
| BlockMergeSort           | 8192 | Reversed           |   109,223.4 ns |    452.54 ns |    200.93 ns |  0.45 |    0.00 |    5 |         - |          NA |
| NaturalMergeSort         | 8192 | Reversed           |     5,612.8 ns |    290.14 ns |    128.82 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 8192 | Reversed           |    33,187.1 ns |    179.11 ns |     79.53 ns |  0.14 |    0.00 |    2 |         - |          NA |
| DropMergeSort            | 8192 | Reversed           |    50,553.6 ns |    537.18 ns |    280.95 ns |  0.21 |    0.00 |    3 |         - |          NA |
| TimSort                  | 8192 | Reversed           |     5,520.6 ns |     40.01 ns |     14.27 ns |  0.02 |    0.00 |    1 |         - |          NA |
| PowerSort                | 8192 | Reversed           |     5,655.4 ns |    365.90 ns |    191.37 ns |  0.02 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | Reversed           |   143,576.3 ns |  1,051.10 ns |    466.69 ns |  0.59 |    0.00 |    5 |         - |          NA |
| SpinSort                 | 8192 | Reversed           |     6,084.6 ns |    331.41 ns |    173.34 ns |  0.03 |    0.00 |    1 |         - |          NA |
| Glidesort                | 8192 | Reversed           |     5,576.8 ns |     18.28 ns |      8.12 ns |  0.02 |    0.00 |    1 |         - |          NA |
| Driftsort                | 8192 | Reversed           |     5,648.6 ns |    220.30 ns |    115.22 ns |  0.02 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | Reversed           |     5,981.6 ns |     46.67 ns |     16.64 ns |  0.02 |    0.00 |    1 |         - |          NA |
| QuadSort                 | 8192 | Reversed           |     6,110.2 ns |    360.35 ns |    188.47 ns |  0.03 |    0.00 |    1 |         - |          NA |
| SimdMergeSort            | 8192 | Reversed           |   195,541.2 ns |    264.41 ns |    117.40 ns |  0.81 |    0.00 |    6 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **PipeOrgan**          |   **177,212.0 ns** |    **186.70 ns** |     **66.58 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | PipeOrgan          |   173,732.6 ns |    724.98 ns |    321.90 ns |  0.98 |    0.00 |    6 |         - |          NA |
| BottomupMergeSort        | 8192 | PipeOrgan          |    88,034.9 ns |    901.73 ns |    400.37 ns |  0.50 |    0.00 |    5 |         - |          NA |
| StdStableSort            | 8192 | PipeOrgan          |   118,443.6 ns |  2,762.15 ns |  1,444.66 ns |  0.67 |    0.01 |    5 |         - |          NA |
| RotateMergeSort          | 8192 | PipeOrgan          |   184,985.3 ns |  1,640.75 ns |    858.15 ns |  1.04 |    0.00 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | PipeOrgan          |   224,577.7 ns |  1,045.14 ns |    464.05 ns |  1.27 |    0.00 |    6 |         - |          NA |
| SymMergeSort             | 8192 | PipeOrgan          |    99,588.6 ns |  2,151.27 ns |  1,125.15 ns |  0.56 |    0.01 |    5 |         - |          NA |
| BlockMergeSort           | 8192 | PipeOrgan          |   100,238.1 ns |  1,088.79 ns |    483.43 ns |  0.57 |    0.00 |    5 |         - |          NA |
| NaturalMergeSort         | 8192 | PipeOrgan          |    10,615.8 ns |    271.39 ns |    141.94 ns |  0.06 |    0.00 |    1 |         - |          NA |
| PatienceSort             | 8192 | PipeOrgan          |   489,751.0 ns |  5,758.10 ns |  2,556.63 ns |  2.76 |    0.01 |    7 |         - |          NA |
| DropMergeSort            | 8192 | PipeOrgan          |   181,343.4 ns |  1,262.67 ns |    560.63 ns |  1.02 |    0.00 |    6 |         - |          NA |
| TimSort                  | 8192 | PipeOrgan          |    16,317.0 ns |     74.50 ns |     33.08 ns |  0.09 |    0.00 |    2 |         - |          NA |
| PowerSort                | 8192 | PipeOrgan          |    10,453.0 ns |    410.61 ns |    214.76 ns |  0.06 |    0.00 |    1 |         - |          NA |
| ShiftSort                | 8192 | PipeOrgan          |    81,871.5 ns |    356.77 ns |    158.41 ns |  0.46 |    0.00 |    5 |         - |          NA |
| SpinSort                 | 8192 | PipeOrgan          |    13,942.8 ns |    378.37 ns |    197.89 ns |  0.08 |    0.00 |    2 |         - |          NA |
| Glidesort                | 8192 | PipeOrgan          |    29,442.1 ns |    458.15 ns |    239.62 ns |  0.17 |    0.00 |    3 |         - |          NA |
| Driftsort                | 8192 | PipeOrgan          |     8,724.3 ns |     36.08 ns |     16.02 ns |  0.05 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | PipeOrgan          |    51,093.2 ns |    358.74 ns |    187.63 ns |  0.29 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 8192 | PipeOrgan          |    18,748.2 ns |    296.77 ns |    155.22 ns |  0.11 |    0.00 |    2 |         - |          NA |
| SimdMergeSort            | 8192 | PipeOrgan          |   196,759.1 ns |    598.33 ns |    265.66 ns |  1.11 |    0.00 |    6 |         - |          NA |
|      |                    |                |              |              |       |         |      |           |             |
| **MergeSort**                | **8192** | **ManyDuplicates**     |   **438,109.1 ns** |  **1,934.73 ns** |    **689.94 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| PingpongMergeSort        | 8192 | ManyDuplicates     |   409,401.8 ns |  3,778.30 ns |  1,677.59 ns |  0.93 |    0.00 |    4 |         - |          NA |
| BottomupMergeSort        | 8192 | ManyDuplicates     |   309,423.2 ns |    758.04 ns |    336.57 ns |  0.71 |    0.00 |    4 |         - |          NA |
| StdStableSort            | 8192 | ManyDuplicates     |   222,117.0 ns |  5,610.70 ns |  2,934.50 ns |  0.51 |    0.01 |    4 |         - |          NA |
| RotateMergeSort          | 8192 | ManyDuplicates     |   886,435.3 ns |  3,649.28 ns |  1,908.64 ns |  2.02 |    0.01 |    6 |         - |          NA |
| RotateMergeSortRecursive | 8192 | ManyDuplicates     | 1,043,267.9 ns |  1,135.00 ns |    593.63 ns |  2.38 |    0.00 |    6 |         - |          NA |
| SymMergeSort             | 8192 | ManyDuplicates     |   602,124.6 ns |  2,491.22 ns |  1,302.96 ns |  1.37 |    0.00 |    5 |         - |          NA |
| BlockMergeSort           | 8192 | ManyDuplicates     |   438,703.8 ns |  2,182.02 ns |    968.83 ns |  1.00 |    0.00 |    4 |         - |          NA |
| NaturalMergeSort         | 8192 | ManyDuplicates     |   319,015.9 ns |    460.71 ns |    204.56 ns |  0.73 |    0.00 |    4 |         - |          NA |
| PatienceSort             | 8192 | ManyDuplicates     |   280,805.8 ns |  2,458.38 ns |  1,091.54 ns |  0.64 |    0.00 |    4 |         - |          NA |
| DropMergeSort            | 8192 | ManyDuplicates     |   233,315.9 ns |  1,079.28 ns |    564.48 ns |  0.53 |    0.00 |    4 |         - |          NA |
| TimSort                  | 8192 | ManyDuplicates     |   370,818.6 ns |  1,242.91 ns |    443.23 ns |  0.85 |    0.00 |    4 |         - |          NA |
| PowerSort                | 8192 | ManyDuplicates     |   246,907.6 ns |  1,002.69 ns |    445.20 ns |  0.56 |    0.00 |    4 |         - |          NA |
| ShiftSort                | 8192 | ManyDuplicates     |   383,529.8 ns |    643.04 ns |    229.32 ns |  0.88 |    0.00 |    4 |         - |          NA |
| SpinSort                 | 8192 | ManyDuplicates     |   223,160.0 ns |  1,388.07 ns |    725.99 ns |  0.51 |    0.00 |    4 |         - |          NA |
| Glidesort                | 8192 | ManyDuplicates     |    35,269.8 ns |    263.37 ns |    137.75 ns |  0.08 |    0.00 |    2 |         - |          NA |
| Driftsort                | 8192 | ManyDuplicates     |    24,817.8 ns |    467.78 ns |    244.66 ns |  0.06 |    0.00 |    1 |         - |          NA |
| FlatStableSort           | 8192 | ManyDuplicates     |   247,443.5 ns |  1,144.02 ns |    598.35 ns |  0.56 |    0.00 |    4 |         - |          NA |
| QuadSort                 | 8192 | ManyDuplicates     |   156,183.9 ns |    565.58 ns |    251.12 ns |  0.36 |    0.00 |    3 |         - |          NA |
| SimdMergeSort            | 8192 | ManyDuplicates     |   155,683.7 ns |    416.57 ns |    184.96 ns |  0.36 |    0.00 |    3 |         - |          NA |

### MergeSlowBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.67GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.5 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method     | Size | Pattern            | Mean         | Error       | StdDev      | Ratio | Rank | Allocated | Alloc Ratio |
| ----------- |----- |------------------- |-------------:|------------:|------------:|------:|-----:|----------:|------------:|
| **StrandSort** | **256**  | **Random**             |   **9,096.9 ns** |   **305.72 ns** |   **159.90 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **256**  | **SingleElementMoved** |     **728.9 ns** |     **6.29 ns** |     **2.79 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **256**  | **Sorted**             |     **489.3 ns** |    **25.43 ns** |    **13.30 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **256**  | **Reversed**           |  **38,477.5 ns** |   **291.76 ns** |   **129.54 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **256**  | **PipeOrgan**          |  **23,301.2 ns** |   **190.19 ns** |    **84.44 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **256**  | **ManyDuplicates**     |   **7,942.7 ns** |   **222.05 ns** |   **116.14 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **Random**             |  **57,401.2 ns** |   **269.44 ns** |   **119.63 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **SingleElementMoved** |   **2,177.1 ns** |    **14.12 ns** |     **6.27 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **Sorted**             |   **1,390.2 ns** |     **5.36 ns** |     **2.38 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **Reversed**           | **619,166.7 ns** | **4,174.97 ns** | **2,183.59 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **PipeOrgan**          | **371,758.3 ns** | **1,013.35 ns** |   **530.00 ns** |  **1.00** |    **1** |         **-** |          **NA** |
|      |                    |              |             |             |       |      |           |             |
| **StrandSort** | **1024** | **ManyDuplicates**     |  **35,687.3 ns** |   **654.79 ns** |   **342.47 ns** |  **1.00** |    **1** |         **-** |          **NA** |

### NetworkBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.67GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.5 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                            | Size | Pattern            | Mean           | Error       | StdDev      | Median         | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ---------------------------------- |----- |------------------- |---------------:|------------:|------------:|---------------:|------:|--------:|-----:|----------:|------------:|
| **BitonicSort**                       | **256**  | **Random**             |    **13,133.1 ns** |   **406.25 ns** |   **212.47 ns** |    **13,119.6 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | Random             |     3,704.1 ns |    42.61 ns |    18.92 ns |     3,694.9 ns |  0.28 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 256  | Random             |    26,157.3 ns |   332.19 ns |   147.50 ns |    26,177.4 ns |  1.99 |    0.03 |    5 |         - |          NA |
| BitonicSortAnyLength              | 256  | Random             |    20,658.8 ns |   398.65 ns |   142.16 ns |    20,735.7 ns |  1.57 |    0.03 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 256  | Random             |     1,053.0 ns |     8.73 ns |     3.88 ns |     1,052.8 ns |  0.08 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | Random             |    10,560.4 ns |   249.32 ns |   110.70 ns |    10,572.5 ns |  0.80 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | Random             |     3,380.3 ns |    46.83 ns |    16.70 ns |     3,372.7 ns |  0.26 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 256  | Random             |    12,155.5 ns |   340.31 ns |   177.99 ns |    12,158.7 ns |  0.93 |    0.02 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | Random             |     3,308.7 ns |    28.57 ns |    10.19 ns |     3,306.6 ns |  0.25 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **SingleElementMoved** |     **3,843.6 ns** |   **365.26 ns** |   **191.04 ns** |     **3,744.0 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | SingleElementMoved |     3,736.0 ns |    55.70 ns |    19.86 ns |     3,725.9 ns |  0.97 |    0.04 |    2 |         - |          NA |
| BitonicRecursiveSort              | 256  | SingleElementMoved |    18,223.4 ns |   210.46 ns |   110.08 ns |    18,201.7 ns |  4.75 |    0.22 |    4 |         - |          NA |
| BitonicSortAnyLength              | 256  | SingleElementMoved |    11,786.3 ns |   247.18 ns |   129.28 ns |    11,800.3 ns |  3.07 |    0.14 |    3 |         - |          NA |
| BitonicSortAnyLengthSimd          | 256  | SingleElementMoved |     1,129.9 ns |    11.64 ns |     5.17 ns |     1,130.0 ns |  0.29 |    0.01 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | SingleElementMoved |     3,907.8 ns |   291.06 ns |   152.23 ns |     3,912.0 ns |  1.02 |    0.06 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | SingleElementMoved |     3,398.5 ns |   140.04 ns |    62.18 ns |     3,365.4 ns |  0.89 |    0.04 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 256  | SingleElementMoved |     2,976.2 ns |   281.20 ns |   147.07 ns |     3,017.0 ns |  0.78 |    0.05 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | SingleElementMoved |     3,327.0 ns |    25.42 ns |     9.06 ns |     3,326.3 ns |  0.87 |    0.04 |    2 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **Sorted**             |     **3,525.6 ns** |   **135.54 ns** |    **70.89 ns** |     **3,527.3 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | Sorted             |     3,707.5 ns |    54.42 ns |    24.16 ns |     3,699.6 ns |  1.05 |    0.02 |    2 |         - |          NA |
| BitonicRecursiveSort              | 256  | Sorted             |    18,012.5 ns |   178.08 ns |    79.07 ns |    17,979.8 ns |  5.11 |    0.10 |    4 |         - |          NA |
| BitonicSortAnyLength              | 256  | Sorted             |    11,636.3 ns |   213.23 ns |   111.52 ns |    11,662.5 ns |  3.30 |    0.07 |    3 |         - |          NA |
| BitonicSortAnyLengthSimd          | 256  | Sorted             |     1,149.9 ns |     6.24 ns |     3.26 ns |     1,150.8 ns |  0.33 |    0.01 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | Sorted             |     3,726.6 ns |    12.94 ns |     5.74 ns |     3,725.9 ns |  1.06 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | Sorted             |     3,362.7 ns |    34.28 ns |    15.22 ns |     3,370.7 ns |  0.95 |    0.02 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 256  | Sorted             |     2,889.1 ns |   301.61 ns |   157.75 ns |     2,959.9 ns |  0.82 |    0.04 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | Sorted             |     3,328.2 ns |    11.88 ns |     4.23 ns |     3,328.5 ns |  0.94 |    0.02 |    2 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **Reversed**           |     **3,586.2 ns** |   **145.11 ns** |    **64.43 ns** |     **3,543.0 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | Reversed           |     3,697.8 ns |    53.90 ns |    19.22 ns |     3,694.2 ns |  1.03 |    0.02 |    3 |         - |          NA |
| BitonicRecursiveSort              | 256  | Reversed           |    17,996.1 ns |    57.01 ns |    29.82 ns |    17,978.6 ns |  5.02 |    0.08 |    5 |         - |          NA |
| BitonicSortAnyLength              | 256  | Reversed           |    11,690.0 ns |   293.40 ns |   153.45 ns |    11,761.9 ns |  3.26 |    0.07 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 256  | Reversed           |     1,964.4 ns |    45.45 ns |    16.21 ns |     1,960.0 ns |  0.55 |    0.01 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | Reversed           |     3,399.6 ns |    58.62 ns |    26.03 ns |     3,388.8 ns |  0.95 |    0.02 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | Reversed           |     3,389.4 ns |    31.74 ns |    11.32 ns |     3,391.4 ns |  0.95 |    0.02 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 256  | Reversed           |     2,670.0 ns |    27.03 ns |     9.64 ns |     2,668.5 ns |  0.74 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | Reversed           |     3,302.7 ns |    29.28 ns |    10.44 ns |     3,304.0 ns |  0.92 |    0.02 |    3 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **PipeOrgan**          |     **3,742.9 ns** |   **213.00 ns** |    **94.57 ns** |     **3,705.9 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | PipeOrgan          |     3,702.1 ns |    48.32 ns |    17.23 ns |     3,698.5 ns |  0.99 |    0.02 |    3 |         - |          NA |
| BitonicRecursiveSort              | 256  | PipeOrgan          |    17,942.9 ns |   153.78 ns |    80.43 ns |    17,919.2 ns |  4.80 |    0.11 |    5 |         - |          NA |
| BitonicSortAnyLength              | 256  | PipeOrgan          |    11,541.7 ns |   285.83 ns |   149.50 ns |    11,556.7 ns |  3.09 |    0.08 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 256  | PipeOrgan          |     1,157.1 ns |    22.21 ns |    11.61 ns |     1,150.9 ns |  0.31 |    0.01 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | PipeOrgan          |     4,585.8 ns | 2,867.37 ns | 1,499.69 ns |     3,660.1 ns |  1.23 |    0.38 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | PipeOrgan          |     3,364.0 ns |    57.05 ns |    20.34 ns |     3,360.6 ns |  0.90 |    0.02 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 256  | PipeOrgan          |     2,750.2 ns |   298.67 ns |   156.21 ns |     2,724.5 ns |  0.74 |    0.04 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | PipeOrgan          |     3,438.3 ns |   279.52 ns |   146.19 ns |     3,339.9 ns |  0.92 |    0.04 |    3 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **BitonicSort**                       | **256**  | **ManyDuplicates**     |    **11,557.7 ns** |   **121.50 ns** |    **53.95 ns** |    **11,551.1 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 256  | ManyDuplicates     |     3,891.2 ns |   342.34 ns |   179.05 ns |     3,865.4 ns |  0.34 |    0.01 |    2 |         - |          NA |
| BitonicRecursiveSort              | 256  | ManyDuplicates     |    25,641.8 ns |   172.49 ns |    76.59 ns |    25,670.5 ns |  2.22 |    0.01 |    5 |         - |          NA |
| BitonicSortAnyLength              | 256  | ManyDuplicates     |    20,104.8 ns |   442.66 ns |   231.52 ns |    20,062.2 ns |  1.74 |    0.02 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 256  | ManyDuplicates     |     1,121.6 ns |    13.46 ns |     5.98 ns |     1,119.2 ns |  0.10 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 256  | ManyDuplicates     |     9,942.4 ns |   382.34 ns |   199.97 ns |     9,942.5 ns |  0.86 |    0.02 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 256  | ManyDuplicates     |     3,554.7 ns |   394.79 ns |   206.48 ns |     3,431.0 ns |  0.31 |    0.02 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 256  | ManyDuplicates     |    11,270.7 ns |   349.77 ns |   155.30 ns |    11,317.8 ns |  0.98 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 256  | ManyDuplicates     |     3,329.9 ns |    22.67 ns |    10.07 ns |     3,334.2 ns |  0.29 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **Random**             |    **66,470.8 ns** |   **554.82 ns** |   **290.18 ns** |    **66,408.2 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | Random             |    22,607.7 ns |   301.07 ns |   157.47 ns |    22,626.7 ns |  0.34 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 1024 | Random             |   138,282.8 ns |   533.54 ns |   236.90 ns |   138,305.0 ns |  2.08 |    0.01 |    6 |         - |          NA |
| BitonicSortAnyLength              | 1024 | Random             |   109,867.8 ns | 2,473.73 ns | 1,098.35 ns |   109,439.0 ns |  1.65 |    0.02 |    5 |         - |          NA |
| BitonicSortAnyLengthSimd          | 1024 | Random             |     7,023.4 ns |   315.17 ns |   164.84 ns |     7,019.6 ns |  0.11 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | Random             |    54,502.4 ns |   696.10 ns |   364.07 ns |    54,592.8 ns |  0.82 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | Random             |    20,672.9 ns |   323.03 ns |   168.95 ns |    20,698.0 ns |  0.31 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | Random             |    67,009.7 ns |   210.24 ns |    93.35 ns |    67,051.9 ns |  1.01 |    0.00 |    4 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | Random             |    20,518.1 ns |   364.40 ns |   190.59 ns |    20,465.0 ns |  0.31 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **SingleElementMoved** |    **21,429.2 ns** |   **339.95 ns** |   **177.80 ns** |    **21,402.3 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | SingleElementMoved |    22,613.6 ns |   419.22 ns |   219.26 ns |    22,544.7 ns |  1.06 |    0.01 |    3 |         - |          NA |
| BitonicRecursiveSort              | 1024 | SingleElementMoved |    93,711.3 ns | 1,084.06 ns |   481.33 ns |    93,475.6 ns |  4.37 |    0.04 |    5 |         - |          NA |
| BitonicSortAnyLength              | 1024 | SingleElementMoved |    61,765.8 ns |   156.37 ns |    69.43 ns |    61,756.4 ns |  2.88 |    0.02 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 1024 | SingleElementMoved |     6,799.5 ns |   171.38 ns |    76.10 ns |     6,808.0 ns |  0.32 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | SingleElementMoved |    22,734.3 ns | 1,174.93 ns |   614.51 ns |    22,839.7 ns |  1.06 |    0.03 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | SingleElementMoved |    20,652.8 ns |   310.61 ns |   162.46 ns |    20,710.7 ns |  0.96 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | SingleElementMoved |    16,926.7 ns | 1,507.38 ns |   788.39 ns |    16,658.6 ns |  0.79 |    0.04 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | SingleElementMoved |    20,348.0 ns |    35.96 ns |    12.82 ns |    20,350.1 ns |  0.95 |    0.01 |    3 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **Sorted**             |    **20,833.4 ns** |   **289.60 ns** |   **128.59 ns** |    **20,828.3 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | Sorted             |    22,712.1 ns |   235.15 ns |   122.99 ns |    22,756.6 ns |  1.09 |    0.01 |    3 |         - |          NA |
| BitonicRecursiveSort              | 1024 | Sorted             |    93,917.0 ns |   210.62 ns |   110.16 ns |    93,956.9 ns |  4.51 |    0.03 |    5 |         - |          NA |
| BitonicSortAnyLength              | 1024 | Sorted             |    61,616.8 ns |   323.35 ns |   143.57 ns |    61,604.4 ns |  2.96 |    0.02 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 1024 | Sorted             |     6,714.8 ns |   175.95 ns |    92.03 ns |     6,717.7 ns |  0.32 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | Sorted             |    23,070.2 ns |   272.90 ns |   142.73 ns |    23,055.2 ns |  1.11 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | Sorted             |    20,660.0 ns |   419.02 ns |   186.05 ns |    20,641.8 ns |  0.99 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | Sorted             |    17,101.2 ns | 1,412.36 ns |   738.69 ns |    17,090.5 ns |  0.82 |    0.03 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | Sorted             |    20,556.6 ns |   181.65 ns |    95.01 ns |    20,509.2 ns |  0.99 |    0.01 |    3 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **Reversed**           |    **21,326.8 ns** |   **409.51 ns** |   **181.83 ns** |    **21,249.1 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | Reversed           |    22,701.4 ns |   293.53 ns |   130.33 ns |    22,647.5 ns |  1.06 |    0.01 |    3 |         - |          NA |
| BitonicRecursiveSort              | 1024 | Reversed           |    93,031.3 ns |   245.85 ns |   109.16 ns |    93,017.7 ns |  4.36 |    0.03 |    5 |         - |          NA |
| BitonicSortAnyLength              | 1024 | Reversed           |    61,826.6 ns |   208.68 ns |    92.66 ns |    61,827.9 ns |  2.90 |    0.02 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 1024 | Reversed           |     6,859.4 ns |   261.64 ns |   136.84 ns |     6,775.3 ns |  0.32 |    0.01 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | Reversed           |    21,613.3 ns |   573.75 ns |   254.75 ns |    21,625.8 ns |  1.01 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | Reversed           |    20,774.5 ns |   334.95 ns |   148.72 ns |    20,748.7 ns |  0.97 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | Reversed           |    16,965.3 ns | 1,348.65 ns |   705.37 ns |    16,594.9 ns |  0.80 |    0.03 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | Reversed           |    20,467.9 ns |   171.78 ns |    76.27 ns |    20,486.7 ns |  0.96 |    0.01 |    3 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **PipeOrgan**          |    **21,519.1 ns** |   **245.99 ns** |   **128.66 ns** |    **21,482.9 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | PipeOrgan          |    22,700.8 ns |   283.94 ns |   148.51 ns |    22,696.8 ns |  1.05 |    0.01 |    3 |         - |          NA |
| BitonicRecursiveSort              | 1024 | PipeOrgan          |    93,430.0 ns |   388.61 ns |   172.54 ns |    93,429.4 ns |  4.34 |    0.03 |    5 |         - |          NA |
| BitonicSortAnyLength              | 1024 | PipeOrgan          |    61,766.9 ns |   329.99 ns |   146.52 ns |    61,722.4 ns |  2.87 |    0.02 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 1024 | PipeOrgan          |     7,066.0 ns |   272.32 ns |   142.43 ns |     7,131.4 ns |  0.33 |    0.01 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | PipeOrgan          |    22,056.1 ns | 1,052.95 ns |   550.71 ns |    21,982.1 ns |  1.02 |    0.02 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | PipeOrgan          |    20,668.6 ns |   375.51 ns |   196.40 ns |    20,693.8 ns |  0.96 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | PipeOrgan          |    16,754.8 ns | 1,166.31 ns |   610.00 ns |    16,520.1 ns |  0.78 |    0.03 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | PipeOrgan          |    20,577.6 ns |   247.53 ns |   129.46 ns |    20,550.8 ns |  0.96 |    0.01 |    3 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **BitonicSort**                       | **1024** | **ManyDuplicates**     |    **55,000.5 ns** |   **379.82 ns** |   **168.64 ns** |    **55,058.0 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 1024 | ManyDuplicates     |    22,766.4 ns |   242.65 ns |   126.91 ns |    22,769.1 ns |  0.41 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 1024 | ManyDuplicates     |   128,844.2 ns |   443.13 ns |   196.75 ns |   128,816.8 ns |  2.34 |    0.01 |    5 |         - |          NA |
| BitonicSortAnyLength              | 1024 | ManyDuplicates     |    99,845.7 ns |   643.73 ns |   285.82 ns |    99,805.6 ns |  1.82 |    0.01 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 1024 | ManyDuplicates     |     6,818.2 ns |   211.03 ns |   110.37 ns |     6,750.3 ns |  0.12 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 1024 | ManyDuplicates     |    48,829.6 ns |   642.67 ns |   285.35 ns |    48,854.6 ns |  0.89 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 1024 | ManyDuplicates     |    20,638.2 ns |    92.73 ns |    33.07 ns |    20,636.6 ns |  0.38 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 1024 | ManyDuplicates     |    54,638.1 ns |   856.67 ns |   448.06 ns |    54,685.8 ns |  0.99 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 1024 | ManyDuplicates     |    20,315.6 ns |   129.10 ns |    57.32 ns |    20,294.9 ns |  0.37 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **Random**             |   **330,357.9 ns** | **1,487.67 ns** |   **660.54 ns** |   **330,270.5 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | Random             |   127,337.4 ns | 1,542.48 ns |   806.75 ns |   126,898.5 ns |  0.39 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 4096 | Random             |   693,050.0 ns | 1,400.68 ns |   621.91 ns |   692,895.8 ns |  2.10 |    0.00 |    5 |         - |          NA |
| BitonicSortAnyLength              | 4096 | Random             |   558,858.6 ns | 5,911.31 ns | 3,091.73 ns |   557,705.3 ns |  1.69 |    0.01 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 4096 | Random             |    34,420.9 ns |   331.31 ns |   147.10 ns |    34,368.2 ns |  0.10 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | Random             |   277,113.8 ns |   969.50 ns |   507.07 ns |   277,028.3 ns |  0.84 |    0.00 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | Random             |   115,986.2 ns | 1,481.09 ns |   657.62 ns |   116,000.7 ns |  0.35 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | Random             |   348,744.3 ns | 3,569.76 ns | 1,867.05 ns |   349,297.1 ns |  1.06 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | Random             |   114,913.3 ns |   975.61 ns |   433.18 ns |   114,798.6 ns |  0.35 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **SingleElementMoved** |   **117,506.4 ns** | **1,235.80 ns** |   **548.70 ns** |   **117,551.3 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | SingleElementMoved |   127,332.0 ns | 2,807.17 ns | 1,246.40 ns |   126,876.4 ns |  1.08 |    0.01 |    2 |         - |          NA |
| BitonicRecursiveSort              | 4096 | SingleElementMoved |   466,456.1 ns | 1,685.16 ns |   881.37 ns |   466,154.8 ns |  3.97 |    0.02 |    4 |         - |          NA |
| BitonicSortAnyLength              | 4096 | SingleElementMoved |   315,733.6 ns | 2,429.01 ns | 1,078.50 ns |   315,514.5 ns |  2.69 |    0.01 |    3 |         - |          NA |
| BitonicSortAnyLengthSimd          | 4096 | SingleElementMoved |    36,931.1 ns |   227.77 ns |   119.13 ns |    36,946.6 ns |  0.31 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | SingleElementMoved |   129,061.7 ns | 3,624.02 ns | 1,895.44 ns |   129,124.8 ns |  1.10 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | SingleElementMoved |   116,707.9 ns | 2,268.36 ns | 1,007.17 ns |   116,410.1 ns |  0.99 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | SingleElementMoved |    96,813.4 ns | 2,423.67 ns | 1,076.12 ns |    97,034.9 ns |  0.82 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | SingleElementMoved |   114,932.0 ns |   978.07 ns |   348.79 ns |   114,799.4 ns |  0.98 |    0.01 |    2 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **Sorted**             |   **116,831.0 ns** |   **897.15 ns** |   **398.34 ns** |   **116,954.5 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | Sorted             |   127,027.9 ns |   512.68 ns |   227.63 ns |   127,084.9 ns |  1.09 |    0.00 |    3 |         - |          NA |
| BitonicRecursiveSort              | 4096 | Sorted             |   467,651.0 ns | 2,859.01 ns | 1,495.32 ns |   467,294.1 ns |  4.00 |    0.02 |    5 |         - |          NA |
| BitonicSortAnyLength              | 4096 | Sorted             |   315,048.5 ns | 2,151.62 ns |   955.33 ns |   314,492.1 ns |  2.70 |    0.01 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 4096 | Sorted             |    37,367.2 ns |   464.98 ns |   206.45 ns |    37,260.8 ns |  0.32 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | Sorted             |   130,224.8 ns | 1,289.27 ns |   674.32 ns |   130,106.1 ns |  1.11 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | Sorted             |   116,087.7 ns |   677.04 ns |   300.61 ns |   116,201.5 ns |  0.99 |    0.00 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | Sorted             |    94,992.5 ns | 1,130.95 ns |   591.51 ns |    95,075.7 ns |  0.81 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | Sorted             |   114,803.9 ns |   272.17 ns |    97.06 ns |   114,785.9 ns |  0.98 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **Reversed**           |   **119,914.9 ns** | **1,837.92 ns** |   **961.27 ns** |   **120,060.1 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | Reversed           |   127,034.1 ns |   737.36 ns |   327.39 ns |   126,920.4 ns |  1.06 |    0.01 |    2 |         - |          NA |
| BitonicRecursiveSort              | 4096 | Reversed           |   464,770.0 ns | 3,245.26 ns | 1,697.33 ns |   464,665.1 ns |  3.88 |    0.03 |    4 |         - |          NA |
| BitonicSortAnyLength              | 4096 | Reversed           |   315,537.4 ns | 1,328.08 ns |   589.68 ns |   315,345.0 ns |  2.63 |    0.02 |    3 |         - |          NA |
| BitonicSortAnyLengthSimd          | 4096 | Reversed           |    38,794.3 ns |   345.57 ns |   180.74 ns |    38,751.7 ns |  0.32 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | Reversed           |   124,297.4 ns | 4,586.03 ns | 2,398.58 ns |   124,558.6 ns |  1.04 |    0.02 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | Reversed           |   115,959.4 ns |   396.77 ns |   141.49 ns |   115,987.2 ns |  0.97 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | Reversed           |    96,887.1 ns | 3,854.08 ns | 2,015.76 ns |    96,444.2 ns |  0.81 |    0.02 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | Reversed           |   114,855.2 ns |   711.18 ns |   253.61 ns |   114,888.4 ns |  0.96 |    0.01 |    2 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **PipeOrgan**          |   **117,765.4 ns** | **1,172.03 ns** |   **417.96 ns** |   **117,798.5 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | PipeOrgan          |   126,770.6 ns |   822.76 ns |   365.31 ns |   126,680.2 ns |  1.08 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 4096 | PipeOrgan          |   465,565.4 ns | 1,997.01 ns | 1,044.48 ns |   465,322.6 ns |  3.95 |    0.02 |    4 |         - |          NA |
| BitonicSortAnyLength              | 4096 | PipeOrgan          |   314,649.3 ns |   949.29 ns |   421.49 ns |   314,737.4 ns |  2.67 |    0.01 |    3 |         - |          NA |
| BitonicSortAnyLengthSimd          | 4096 | PipeOrgan          |    38,053.4 ns |   173.27 ns |    90.62 ns |    38,057.3 ns |  0.32 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | PipeOrgan          |   124,398.5 ns | 2,581.16 ns | 1,146.05 ns |   124,309.4 ns |  1.06 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | PipeOrgan          |   116,265.5 ns | 3,341.40 ns | 1,483.60 ns |   115,753.6 ns |  0.99 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | PipeOrgan          |    96,530.7 ns | 1,713.52 ns |   760.81 ns |    96,583.4 ns |  0.82 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | PipeOrgan          |   114,870.6 ns |   479.46 ns |   212.89 ns |   114,810.2 ns |  0.98 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **BitonicSort**                       | **4096** | **ManyDuplicates**     |   **253,573.8 ns** | **1,103.31 ns** |   **577.05 ns** |   **253,580.3 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 4096 | ManyDuplicates     |   126,539.6 ns |   309.97 ns |   137.63 ns |   126,476.3 ns |  0.50 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 4096 | ManyDuplicates     |   614,090.5 ns |   903.32 ns |   401.08 ns |   614,085.2 ns |  2.42 |    0.01 |    5 |         - |          NA |
| BitonicSortAnyLength              | 4096 | ManyDuplicates     |   476,212.4 ns | 1,233.18 ns |   547.54 ns |   476,300.6 ns |  1.88 |    0.00 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 4096 | ManyDuplicates     |    37,088.6 ns |   218.45 ns |    96.99 ns |    37,094.0 ns |  0.15 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 4096 | ManyDuplicates     |   232,218.3 ns | 1,586.39 ns |   829.71 ns |   232,376.8 ns |  0.92 |    0.00 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 4096 | ManyDuplicates     |   116,235.5 ns | 1,272.88 ns |   565.17 ns |   116,148.0 ns |  0.46 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 4096 | ManyDuplicates     |   252,979.6 ns | 3,520.37 ns | 1,841.22 ns |   252,879.2 ns |  1.00 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 4096 | ManyDuplicates     |   114,904.4 ns |   609.48 ns |   270.61 ns |   114,770.5 ns |  0.45 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **Random**             |   **733,319.7 ns** | **2,082.39 ns** |   **924.60 ns** |   **733,415.4 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | Random             |   293,793.6 ns | 1,025.48 ns |   536.35 ns |   293,724.9 ns |  0.40 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 8192 | Random             | 1,552,877.7 ns | 3,044.81 ns | 1,592.49 ns | 1,552,659.2 ns |  2.12 |    0.00 |    5 |         - |          NA |
| BitonicSortAnyLength              | 8192 | Random             | 1,256,370.5 ns | 5,664.00 ns | 2,962.38 ns | 1,256,321.0 ns |  1.71 |    0.00 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 8192 | Random             |    87,660.0 ns |   788.91 ns |   350.28 ns |    87,639.5 ns |  0.12 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | Random             |   614,827.3 ns | 2,289.30 ns | 1,197.35 ns |   614,593.9 ns |  0.84 |    0.00 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | Random             |   269,831.8 ns | 1,723.20 ns |   765.11 ns |   269,430.9 ns |  0.37 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | Random             |   791,624.2 ns | 2,013.46 ns |   893.99 ns |   791,974.1 ns |  1.08 |    0.00 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | Random             |   267,857.5 ns |   644.11 ns |   285.99 ns |   267,835.5 ns |  0.37 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **SingleElementMoved** |   **271,466.2 ns** | **3,030.73 ns** | **1,585.13 ns** |   **271,060.6 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | SingleElementMoved |   293,382.8 ns |   891.34 ns |   466.19 ns |   293,189.3 ns |  1.08 |    0.01 |    3 |         - |          NA |
| BitonicRecursiveSort              | 8192 | SingleElementMoved | 1,050,418.0 ns | 1,832.43 ns |   958.40 ns | 1,050,654.1 ns |  3.87 |    0.02 |    5 |         - |          NA |
| BitonicSortAnyLength              | 8192 | SingleElementMoved |   709,853.2 ns | 1,217.06 ns |   540.38 ns |   709,623.4 ns |  2.61 |    0.01 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 8192 | SingleElementMoved |    85,782.9 ns |   595.73 ns |   264.51 ns |    85,838.7 ns |  0.32 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | SingleElementMoved |   299,468.4 ns | 3,529.39 ns | 1,567.07 ns |   299,799.3 ns |  1.10 |    0.01 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | SingleElementMoved |   269,769.0 ns | 1,865.10 ns |   665.11 ns |   269,943.7 ns |  0.99 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | SingleElementMoved |   220,426.1 ns | 6,236.56 ns | 2,769.07 ns |   221,068.9 ns |  0.81 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | SingleElementMoved |   267,564.3 ns |   572.62 ns |   204.20 ns |   267,581.9 ns |  0.99 |    0.01 |    3 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **Sorted**             |   **270,671.4 ns** | **1,834.68 ns** |   **959.57 ns** |   **270,687.7 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | Sorted             |   293,566.8 ns | 1,278.28 ns |   567.56 ns |   293,683.1 ns |  1.08 |    0.00 |    3 |         - |          NA |
| BitonicRecursiveSort              | 8192 | Sorted             | 1,050,738.5 ns | 3,755.25 ns | 1,667.35 ns | 1,050,133.8 ns |  3.88 |    0.01 |    5 |         - |          NA |
| BitonicSortAnyLength              | 8192 | Sorted             |   710,105.9 ns | 2,505.49 ns | 1,310.42 ns |   710,113.3 ns |  2.62 |    0.01 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 8192 | Sorted             |    86,013.8 ns |   454.94 ns |   162.24 ns |    85,988.1 ns |  0.32 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | Sorted             |   301,411.5 ns | 1,425.74 ns |   745.69 ns |   301,460.1 ns |  1.11 |    0.00 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | Sorted             |   271,338.7 ns | 3,105.02 ns | 1,623.99 ns |   271,133.0 ns |  1.00 |    0.01 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | Sorted             |   222,092.4 ns | 5,604.77 ns | 2,931.40 ns |   222,484.1 ns |  0.82 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | Sorted             |   267,496.4 ns |   436.59 ns |   193.85 ns |   267,425.7 ns |  0.99 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **Reversed**           |   **275,893.6 ns** | **2,611.74 ns** | **1,159.63 ns** |   **275,644.9 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | Reversed           |   293,566.8 ns | 1,356.73 ns |   709.59 ns |   293,640.8 ns |  1.06 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 8192 | Reversed           | 1,044,623.0 ns | 2,105.75 ns | 1,101.35 ns | 1,044,790.5 ns |  3.79 |    0.02 |    4 |         - |          NA |
| BitonicSortAnyLength              | 8192 | Reversed           |   709,551.4 ns | 1,113.53 ns |   494.41 ns |   709,378.3 ns |  2.57 |    0.01 |    3 |         - |          NA |
| BitonicSortAnyLengthSimd          | 8192 | Reversed           |    86,035.7 ns |   967.28 ns |   429.48 ns |    85,847.4 ns |  0.31 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | Reversed           |   289,146.9 ns | 5,679.11 ns | 2,970.28 ns |   289,219.5 ns |  1.05 |    0.01 |    2 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | Reversed           |   271,406.7 ns | 3,343.04 ns | 1,484.33 ns |   271,328.9 ns |  0.98 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | Reversed           |   225,748.4 ns | 6,019.71 ns | 3,148.43 ns |   223,958.4 ns |  0.82 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | Reversed           |   267,578.6 ns | 1,615.37 ns |   717.23 ns |   267,355.5 ns |  0.97 |    0.00 |    2 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **PipeOrgan**          |   **272,818.1 ns** | **1,352.50 ns** |   **707.38 ns** |   **272,592.2 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | PipeOrgan          |   293,611.1 ns |   849.19 ns |   302.83 ns |   293,677.4 ns |  1.08 |    0.00 |    3 |         - |          NA |
| BitonicRecursiveSort              | 8192 | PipeOrgan          | 1,047,811.5 ns | 5,291.29 ns | 2,767.45 ns | 1,047,597.2 ns |  3.84 |    0.01 |    5 |         - |          NA |
| BitonicSortAnyLength              | 8192 | PipeOrgan          |   709,403.4 ns | 1,678.33 ns |   877.80 ns |   709,214.9 ns |  2.60 |    0.01 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 8192 | PipeOrgan          |    87,742.2 ns |   303.00 ns |   134.54 ns |    87,716.2 ns |  0.32 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | PipeOrgan          |   290,301.6 ns | 2,466.00 ns | 1,094.92 ns |   290,180.5 ns |  1.06 |    0.00 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | PipeOrgan          |   270,335.9 ns | 2,279.29 ns | 1,192.11 ns |   270,239.1 ns |  0.99 |    0.00 |    3 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | PipeOrgan          |   220,922.6 ns | 3,953.40 ns | 1,755.33 ns |   220,902.5 ns |  0.81 |    0.01 |    2 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | PipeOrgan          |   268,399.1 ns | 1,462.21 ns |   764.77 ns |   268,361.6 ns |  0.98 |    0.00 |    3 |         - |          NA |
|      |                    |                |             |             |                |       |         |      |           |             |
| **BitonicSort**                       | **8192** | **ManyDuplicates**     |   **543,262.7 ns** |   **992.37 ns** |   **440.62 ns** |   **543,180.7 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| BitonicSortBranchless             | 8192 | ManyDuplicates     |   293,265.7 ns | 1,321.59 ns |   586.79 ns |   293,012.5 ns |  0.54 |    0.00 |    2 |         - |          NA |
| BitonicRecursiveSort              | 8192 | ManyDuplicates     | 1,347,873.6 ns | 2,834.64 ns | 1,258.60 ns | 1,348,202.6 ns |  2.48 |    0.00 |    5 |         - |          NA |
| BitonicSortAnyLength              | 8192 | ManyDuplicates     | 1,039,741.0 ns | 4,134.14 ns | 2,162.24 ns | 1,039,480.8 ns |  1.91 |    0.00 |    4 |         - |          NA |
| BitonicSortAnyLengthSimd          | 8192 | ManyDuplicates     |    86,229.6 ns |   176.76 ns |    63.04 ns |    86,242.0 ns |  0.16 |    0.00 |    1 |         - |          NA |
| BatcherOddEvenMergeSort           | 8192 | ManyDuplicates     |   503,802.9 ns | 3,439.73 ns | 1,799.05 ns |   503,325.3 ns |  0.93 |    0.00 |    3 |         - |          NA |
| BatcherOddEvenMergeSortBranchless | 8192 | ManyDuplicates     |   269,681.6 ns | 1,975.78 ns | 1,033.37 ns |   269,277.1 ns |  0.50 |    0.00 |    2 |         - |          NA |
| PairwiseSortingNetwork            | 8192 | ManyDuplicates     |   541,231.6 ns | 4,505.25 ns | 2,000.36 ns |   541,780.4 ns |  1.00 |    0.00 |    3 |         - |          NA |
| PairwiseSortingNetworkBranchless  | 8192 | ManyDuplicates     |   268,610.4 ns | 1,782.98 ns |   932.53 ns |   268,241.3 ns |  0.49 |    0.00 |    2 |         - |          NA |

### PartitionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.67GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.5 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                       | Size | Pattern            | Mean           | Error        | StdDev      | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------------- |----- |------------------- |---------------:|-------------:|------------:|------:|--------:|-----:|----------:|------------:|
| **QuickSort**                    | **256**  | **Random**             |     **8,583.8 ns** |    **541.77 ns** |   **283.36 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 256  | Random             |     9,348.4 ns |    353.93 ns |   185.11 ns |  1.09 |    0.04 |    4 |         - |          NA |
| QuickSortMedian3             | 256  | Random             |     9,156.2 ns |    301.89 ns |   157.90 ns |  1.07 |    0.04 |    4 |         - |          NA |
| QuickSortMedian9             | 256  | Random             |     9,663.7 ns |    263.89 ns |   117.17 ns |  1.13 |    0.04 |    4 |         - |          NA |
| SampleSort                   | 256  | Random             |    11,071.7 ns |    414.80 ns |   216.95 ns |  1.29 |    0.05 |    4 |         - |          NA |
| SimdQuickSort                | 256  | Random             |       903.6 ns |      6.52 ns |     2.32 ns |  0.11 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort           | 256  | Random             |     5,845.4 ns |    152.88 ns |    79.96 ns |  0.68 |    0.02 |    3 |         - |          NA |
| StableQuickSort              | 256  | Random             |    19,691.8 ns |    136.99 ns |    60.83 ns |  2.30 |    0.07 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Random             |    11,521.2 ns |    355.55 ns |   185.96 ns |  1.34 |    0.05 |    4 |         - |          NA |
| DestswapStableQuickSort      | 256  | Random             |    19,177.0 ns |    295.61 ns |   154.61 ns |  2.24 |    0.07 |    5 |         - |          NA |
| IntroSort                    | 256  | Random             |     6,978.3 ns |    237.20 ns |   105.32 ns |  0.81 |    0.03 |    3 |         - |          NA |
| IntroSortDotnet              | 256  | Random             |     6,522.6 ns |    316.48 ns |   140.52 ns |  0.76 |    0.03 |    3 |         - |          NA |
| PDQSort                      | 256  | Random             |     6,111.9 ns |    377.90 ns |   197.65 ns |  0.71 |    0.03 |    3 |         - |          NA |
| PDQSortBranchless            | 256  | Random             |     4,416.7 ns |    330.51 ns |   172.86 ns |  0.52 |    0.02 |    2 |         - |          NA |
| Ipnsort                      | 256  | Random             |     3,836.2 ns |    396.75 ns |   207.51 ns |  0.45 |    0.03 |    2 |         - |          NA |
| StdSort                      | 256  | Random             |     4,482.3 ns |    241.28 ns |   126.20 ns |  0.52 |    0.02 |    2 |         - |          NA |
| BlockQuickSort               | 256  | Random             |     4,699.5 ns |    253.49 ns |   132.58 ns |  0.55 |    0.02 |    2 |         - |          NA |
| Ips4oSort                    | 256  | Random             |    10,771.4 ns |    271.03 ns |   141.76 ns |  1.26 |    0.04 |    4 |         - |          NA |
| FluxSort                     | 256  | Random             |     6,141.1 ns |    347.98 ns |   182.00 ns |  0.72 |    0.03 |    3 |         - |          NA |
| CrumSort                     | 256  | Random             |     3,537.2 ns |    279.57 ns |   146.22 ns |  0.41 |    0.02 |    2 |         - |          NA |
| BlitSort                     | 256  | Random             |     6,191.2 ns |    328.29 ns |   171.70 ns |  0.72 |    0.03 |    3 |         - |          NA |
| DotnetSort                   | 256  | Random             |     5,921.7 ns |    461.76 ns |   241.51 ns |  0.69 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **SingleElementMoved** |     **1,069.8 ns** |     **72.47 ns** |    **32.18 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 256  | SingleElementMoved |     2,274.2 ns |    371.25 ns |   194.17 ns |  2.13 |    0.18 |    4 |         - |          NA |
| QuickSortMedian3             | 256  | SingleElementMoved |     1,634.1 ns |    166.50 ns |    73.93 ns |  1.53 |    0.08 |    4 |         - |          NA |
| QuickSortMedian9             | 256  | SingleElementMoved |     1,897.6 ns |    322.41 ns |   168.63 ns |  1.78 |    0.16 |    4 |         - |          NA |
| SampleSort                   | 256  | SingleElementMoved |     5,564.1 ns |    664.10 ns |   347.34 ns |  5.21 |    0.34 |    6 |         - |          NA |
| SimdQuickSort                | 256  | SingleElementMoved |       902.1 ns |      5.83 ns |     2.59 ns |  0.84 |    0.02 |    3 |         - |          NA |
| DualPivotQuickSort           | 256  | SingleElementMoved |       816.7 ns |     45.38 ns |    20.15 ns |  0.76 |    0.03 |    3 |         - |          NA |
| StableQuickSort              | 256  | SingleElementMoved |     6,300.2 ns |    344.83 ns |   180.35 ns |  5.89 |    0.23 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 256  | SingleElementMoved |     3,639.6 ns |     89.51 ns |    31.92 ns |  3.40 |    0.10 |    5 |         - |          NA |
| DestswapStableQuickSort      | 256  | SingleElementMoved |     7,380.0 ns |     42.59 ns |    18.91 ns |  6.90 |    0.19 |    6 |         - |          NA |
| IntroSort                    | 256  | SingleElementMoved |     2,105.3 ns |     96.29 ns |    42.75 ns |  1.97 |    0.07 |    4 |         - |          NA |
| IntroSortDotnet              | 256  | SingleElementMoved |     1,105.5 ns |     78.50 ns |    34.86 ns |  1.03 |    0.04 |    3 |         - |          NA |
| PDQSort                      | 256  | SingleElementMoved |       850.3 ns |    114.81 ns |    60.05 ns |  0.80 |    0.06 |    3 |         - |          NA |
| PDQSortBranchless            | 256  | SingleElementMoved |     1,049.6 ns |     97.95 ns |    51.23 ns |  0.98 |    0.05 |    3 |         - |          NA |
| Ipnsort                      | 256  | SingleElementMoved |     2,888.9 ns |     63.66 ns |    28.26 ns |  2.70 |    0.08 |    4 |         - |          NA |
| StdSort                      | 256  | SingleElementMoved |     1,000.7 ns |    100.92 ns |    44.81 ns |  0.94 |    0.05 |    3 |         - |          NA |
| BlockQuickSort               | 256  | SingleElementMoved |     1,558.1 ns |     73.84 ns |    32.79 ns |  1.46 |    0.05 |    4 |         - |          NA |
| Ips4oSort                    | 256  | SingleElementMoved |       443.3 ns |      4.36 ns |     1.94 ns |  0.41 |    0.01 |    1 |         - |          NA |
| FluxSort                     | 256  | SingleElementMoved |     1,232.9 ns |     48.99 ns |    21.75 ns |  1.15 |    0.04 |    3 |         - |          NA |
| CrumSort                     | 256  | SingleElementMoved |       538.5 ns |     43.16 ns |    19.16 ns |  0.50 |    0.02 |    2 |         - |          NA |
| BlitSort                     | 256  | SingleElementMoved |     2,573.9 ns |    343.30 ns |   179.55 ns |  2.41 |    0.17 |    4 |         - |          NA |
| DotnetSort                   | 256  | SingleElementMoved |     1,076.5 ns |     95.40 ns |    42.36 ns |  1.01 |    0.05 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Sorted**             |       **713.6 ns** |     **37.14 ns** |    **16.49 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 256  | Sorted             |     1,408.1 ns |     36.72 ns |    16.30 ns |  1.97 |    0.05 |    5 |         - |          NA |
| QuickSortMedian3             | 256  | Sorted             |       925.4 ns |     15.76 ns |     7.00 ns |  1.30 |    0.03 |    4 |         - |          NA |
| QuickSortMedian9             | 256  | Sorted             |     1,006.2 ns |     25.59 ns |    11.36 ns |  1.41 |    0.03 |    4 |         - |          NA |
| SampleSort                   | 256  | Sorted             |     3,498.9 ns |    103.47 ns |    36.90 ns |  4.91 |    0.12 |    6 |         - |          NA |
| SimdQuickSort                | 256  | Sorted             |       674.3 ns |     11.62 ns |     5.16 ns |  0.95 |    0.02 |    3 |         - |          NA |
| DualPivotQuickSort           | 256  | Sorted             |       617.8 ns |     84.90 ns |    44.40 ns |  0.87 |    0.06 |    3 |         - |          NA |
| StableQuickSort              | 256  | Sorted             |     6,010.6 ns |    361.94 ns |   189.30 ns |  8.43 |    0.31 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Sorted             |     3,492.5 ns |    231.38 ns |   121.01 ns |  4.90 |    0.19 |    6 |         - |          NA |
| DestswapStableQuickSort      | 256  | Sorted             |     7,073.3 ns |     58.01 ns |    25.76 ns |  9.92 |    0.22 |    7 |         - |          NA |
| IntroSort                    | 256  | Sorted             |       624.8 ns |      4.50 ns |     2.00 ns |  0.88 |    0.02 |    3 |         - |          NA |
| IntroSortDotnet              | 256  | Sorted             |       759.5 ns |     21.42 ns |     7.64 ns |  1.06 |    0.03 |    3 |         - |          NA |
| PDQSort                      | 256  | Sorted             |       310.3 ns |      3.54 ns |     1.57 ns |  0.44 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | Sorted             |       320.0 ns |      7.51 ns |     2.68 ns |  0.45 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 256  | Sorted             |       130.8 ns |      6.03 ns |     3.15 ns |  0.18 |    0.01 |    1 |         - |          NA |
| StdSort                      | 256  | Sorted             |       337.1 ns |      6.64 ns |     3.47 ns |  0.47 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 256  | Sorted             |     1,235.6 ns |      5.47 ns |     2.43 ns |  1.73 |    0.04 |    5 |         - |          NA |
| Ips4oSort                    | 256  | Sorted             |       147.4 ns |      8.27 ns |     3.67 ns |  0.21 |    0.01 |    1 |         - |          NA |
| FluxSort                     | 256  | Sorted             |       222.5 ns |     13.32 ns |     5.92 ns |  0.31 |    0.01 |    2 |         - |          NA |
| CrumSort                     | 256  | Sorted             |       270.7 ns |     13.16 ns |     6.88 ns |  0.38 |    0.01 |    2 |         - |          NA |
| BlitSort                     | 256  | Sorted             |       236.5 ns |     11.53 ns |     5.12 ns |  0.33 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 256  | Sorted             |       645.1 ns |     37.79 ns |    13.48 ns |  0.90 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **Reversed**           |       **809.2 ns** |     **29.58 ns** |    **13.14 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 256  | Reversed           |     1,603.9 ns |     51.13 ns |    26.74 ns |  1.98 |    0.04 |    7 |         - |          NA |
| QuickSortMedian3             | 256  | Reversed           |     1,014.1 ns |      7.66 ns |     3.40 ns |  1.25 |    0.02 |    6 |         - |          NA |
| QuickSortMedian9             | 256  | Reversed           |     1,129.4 ns |     46.19 ns |    24.16 ns |  1.40 |    0.04 |    6 |         - |          NA |
| SampleSort                   | 256  | Reversed           |     3,954.4 ns |    458.18 ns |   239.63 ns |  4.89 |    0.29 |    8 |         - |          NA |
| SimdQuickSort                | 256  | Reversed           |       655.2 ns |      7.18 ns |     3.19 ns |  0.81 |    0.01 |    5 |         - |          NA |
| DualPivotQuickSort           | 256  | Reversed           |       745.4 ns |     12.28 ns |     5.45 ns |  0.92 |    0.02 |    5 |         - |          NA |
| StableQuickSort              | 256  | Reversed           |     6,397.4 ns |    372.43 ns |   194.79 ns |  7.91 |    0.26 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 256  | Reversed           |     3,446.2 ns |    144.25 ns |    64.05 ns |  4.26 |    0.10 |    8 |         - |          NA |
| DestswapStableQuickSort      | 256  | Reversed           |     6,553.5 ns |     41.52 ns |    18.43 ns |  8.10 |    0.12 |    9 |         - |          NA |
| IntroSort                    | 256  | Reversed           |       662.1 ns |      5.29 ns |     2.35 ns |  0.82 |    0.01 |    5 |         - |          NA |
| IntroSortDotnet              | 256  | Reversed           |     1,095.0 ns |     18.96 ns |     8.42 ns |  1.35 |    0.02 |    6 |         - |          NA |
| PDQSort                      | 256  | Reversed           |       456.2 ns |     12.81 ns |     5.69 ns |  0.56 |    0.01 |    4 |         - |          NA |
| PDQSortBranchless            | 256  | Reversed           |       596.9 ns |     19.68 ns |    10.29 ns |  0.74 |    0.02 |    5 |         - |          NA |
| Ipnsort                      | 256  | Reversed           |       189.8 ns |      2.64 ns |     1.17 ns |  0.23 |    0.00 |    1 |         - |          NA |
| StdSort                      | 256  | Reversed           |       665.5 ns |     18.72 ns |     8.31 ns |  0.82 |    0.02 |    5 |         - |          NA |
| BlockQuickSort               | 256  | Reversed           |     1,677.3 ns |     13.74 ns |     6.10 ns |  2.07 |    0.03 |    7 |         - |          NA |
| Ips4oSort                    | 256  | Reversed           |       382.7 ns |    405.05 ns |   179.84 ns |  0.47 |    0.21 |    3 |         - |          NA |
| FluxSort                     | 256  | Reversed           |       274.4 ns |      5.63 ns |     2.01 ns |  0.34 |    0.01 |    2 |         - |          NA |
| CrumSort                     | 256  | Reversed           |       242.8 ns |      7.92 ns |     3.51 ns |  0.30 |    0.01 |    2 |         - |          NA |
| BlitSort                     | 256  | Reversed           |       288.9 ns |     12.53 ns |     5.56 ns |  0.36 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 256  | Reversed           |     1,041.9 ns |    176.87 ns |    78.53 ns |  1.29 |    0.09 |    6 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **PipeOrgan**          |     **5,914.8 ns** |    **286.23 ns** |   **149.70 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 256  | PipeOrgan          |     2,833.9 ns |    185.20 ns |    82.23 ns |  0.48 |    0.02 |    4 |         - |          NA |
| QuickSortMedian3             | 256  | PipeOrgan          |     2,443.7 ns |    123.82 ns |    64.76 ns |  0.41 |    0.01 |    4 |         - |          NA |
| QuickSortMedian9             | 256  | PipeOrgan          |     2,015.8 ns |     74.26 ns |    38.84 ns |  0.34 |    0.01 |    3 |         - |          NA |
| SampleSort                   | 256  | PipeOrgan          |     3,872.4 ns |    268.24 ns |   140.30 ns |  0.66 |    0.03 |    4 |         - |          NA |
| SimdQuickSort                | 256  | PipeOrgan          |       680.5 ns |      6.87 ns |     3.05 ns |  0.12 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort           | 256  | PipeOrgan          |     1,537.2 ns |     49.08 ns |    21.79 ns |  0.26 |    0.01 |    3 |         - |          NA |
| StableQuickSort              | 256  | PipeOrgan          |     5,982.5 ns |    279.05 ns |   145.95 ns |  1.01 |    0.03 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 256  | PipeOrgan          |     3,440.1 ns |     31.58 ns |    11.26 ns |  0.58 |    0.01 |    4 |         - |          NA |
| DestswapStableQuickSort      | 256  | PipeOrgan          |     8,956.6 ns |    301.76 ns |   157.83 ns |  1.52 |    0.04 |    6 |         - |          NA |
| IntroSort                    | 256  | PipeOrgan          |     3,162.8 ns |     58.78 ns |    26.10 ns |  0.54 |    0.01 |    4 |         - |          NA |
| IntroSortDotnet              | 256  | PipeOrgan          |     1,659.5 ns |     63.76 ns |    28.31 ns |  0.28 |    0.01 |    3 |         - |          NA |
| PDQSort                      | 256  | PipeOrgan          |     1,319.0 ns |    282.08 ns |   125.25 ns |  0.22 |    0.02 |    2 |         - |          NA |
| PDQSortBranchless            | 256  | PipeOrgan          |     1,993.3 ns |     32.99 ns |    14.65 ns |  0.34 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 256  | PipeOrgan          |     3,185.7 ns |    267.81 ns |   140.07 ns |  0.54 |    0.03 |    4 |         - |          NA |
| StdSort                      | 256  | PipeOrgan          |     3,778.5 ns |    215.95 ns |   112.95 ns |  0.64 |    0.02 |    4 |         - |          NA |
| BlockQuickSort               | 256  | PipeOrgan          |     3,609.9 ns |     61.47 ns |    21.92 ns |  0.61 |    0.01 |    4 |         - |          NA |
| Ips4oSort                    | 256  | PipeOrgan          |     9,942.6 ns |    471.90 ns |   246.81 ns |  1.68 |    0.06 |    6 |         - |          NA |
| FluxSort                     | 256  | PipeOrgan          |     3,153.0 ns |    351.96 ns |   184.08 ns |  0.53 |    0.03 |    4 |         - |          NA |
| CrumSort                     | 256  | PipeOrgan          |       671.2 ns |     10.61 ns |     4.71 ns |  0.11 |    0.00 |    1 |         - |          NA |
| BlitSort                     | 256  | PipeOrgan          |     3,156.8 ns |     52.80 ns |    23.44 ns |  0.53 |    0.01 |    4 |         - |          NA |
| DotnetSort                   | 256  | PipeOrgan          |     1,880.1 ns |    256.20 ns |   134.00 ns |  0.32 |    0.02 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **256**  | **ManyDuplicates**     |     **6,850.0 ns** |     **60.75 ns** |    **21.66 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 256  | ManyDuplicates     |     6,027.3 ns |    180.20 ns |    80.01 ns |  0.88 |    0.01 |    3 |         - |          NA |
| QuickSortMedian3             | 256  | ManyDuplicates     |     7,809.8 ns |    245.76 ns |   128.54 ns |  1.14 |    0.02 |    3 |         - |          NA |
| QuickSortMedian9             | 256  | ManyDuplicates     |     8,128.7 ns |    137.68 ns |    61.13 ns |  1.19 |    0.01 |    3 |         - |          NA |
| SampleSort                   | 256  | ManyDuplicates     |     9,839.3 ns |    341.63 ns |   178.68 ns |  1.44 |    0.03 |    4 |         - |          NA |
| SimdQuickSort                | 256  | ManyDuplicates     |       903.3 ns |      5.72 ns |     2.99 ns |  0.13 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort           | 256  | ManyDuplicates     |     5,172.5 ns |    219.76 ns |   114.94 ns |  0.76 |    0.02 |    3 |         - |          NA |
| StableQuickSort              | 256  | ManyDuplicates     |    12,864.5 ns |    320.51 ns |   167.63 ns |  1.88 |    0.02 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 256  | ManyDuplicates     |     6,488.1 ns |     74.09 ns |    32.90 ns |  0.95 |    0.01 |    3 |         - |          NA |
| DestswapStableQuickSort      | 256  | ManyDuplicates     |    10,930.9 ns |    434.90 ns |   227.46 ns |  1.60 |    0.03 |    4 |         - |          NA |
| IntroSort                    | 256  | ManyDuplicates     |     6,144.5 ns |    133.59 ns |    69.87 ns |  0.90 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet              | 256  | ManyDuplicates     |     6,052.2 ns |    245.66 ns |   128.49 ns |  0.88 |    0.02 |    3 |         - |          NA |
| PDQSort                      | 256  | ManyDuplicates     |     5,593.3 ns |    224.17 ns |   117.24 ns |  0.82 |    0.02 |    3 |         - |          NA |
| PDQSortBranchless            | 256  | ManyDuplicates     |     3,838.8 ns |    255.19 ns |   113.30 ns |  0.56 |    0.02 |    2 |         - |          NA |
| Ipnsort                      | 256  | ManyDuplicates     |     3,754.2 ns |    216.84 ns |   113.41 ns |  0.55 |    0.02 |    2 |         - |          NA |
| StdSort                      | 256  | ManyDuplicates     |     4,076.1 ns |    308.64 ns |   161.43 ns |  0.60 |    0.02 |    2 |         - |          NA |
| BlockQuickSort               | 256  | ManyDuplicates     |     4,282.3 ns |    286.01 ns |   149.59 ns |  0.63 |    0.02 |    2 |         - |          NA |
| Ips4oSort                    | 256  | ManyDuplicates     |    10,767.2 ns |    380.42 ns |   198.97 ns |  1.57 |    0.03 |    4 |         - |          NA |
| FluxSort                     | 256  | ManyDuplicates     |     6,166.4 ns |    524.06 ns |   274.09 ns |  0.90 |    0.04 |    3 |         - |          NA |
| CrumSort                     | 256  | ManyDuplicates     |     3,609.4 ns |    338.04 ns |   176.80 ns |  0.53 |    0.02 |    2 |         - |          NA |
| BlitSort                     | 256  | ManyDuplicates     |     6,053.9 ns |    137.11 ns |    71.71 ns |  0.88 |    0.01 |    3 |         - |          NA |
| DotnetSort                   | 256  | ManyDuplicates     |     5,554.2 ns |    416.07 ns |   217.61 ns |  0.81 |    0.03 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Random**             |    **40,558.7 ns** |    **551.66 ns** |   **244.94 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Random             |    44,128.2 ns |    318.92 ns |   166.80 ns |  1.09 |    0.01 |    2 |         - |          NA |
| QuickSortMedian3             | 1024 | Random             |    42,614.8 ns |    484.30 ns |   215.03 ns |  1.05 |    0.01 |    2 |         - |          NA |
| QuickSortMedian9             | 1024 | Random             |    45,225.0 ns |    367.58 ns |   192.25 ns |  1.12 |    0.01 |    2 |         - |          NA |
| SampleSort                   | 1024 | Random             |    51,327.2 ns |    465.88 ns |   243.67 ns |  1.27 |    0.01 |    2 |         - |          NA |
| SimdQuickSort                | 1024 | Random             |     3,709.7 ns |    377.17 ns |   197.27 ns |  0.09 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort           | 1024 | Random             |    29,981.1 ns |    324.38 ns |   169.66 ns |  0.74 |    0.01 |    2 |         - |          NA |
| StableQuickSort              | 1024 | Random             |    98,110.0 ns |    388.43 ns |   172.47 ns |  2.42 |    0.01 |    3 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Random             |    53,632.8 ns |    278.53 ns |   123.67 ns |  1.32 |    0.01 |    2 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Random             |    87,138.3 ns |    848.71 ns |   376.83 ns |  2.15 |    0.01 |    3 |         - |          NA |
| IntroSort                    | 1024 | Random             |    34,238.1 ns |     86.43 ns |    38.37 ns |  0.84 |    0.00 |    2 |         - |          NA |
| IntroSortDotnet              | 1024 | Random             |    32,375.1 ns |    281.76 ns |   147.37 ns |  0.80 |    0.01 |    2 |         - |          NA |
| PDQSort                      | 1024 | Random             |    30,842.7 ns |    192.24 ns |    85.36 ns |  0.76 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | Random             |    19,565.2 ns |    178.95 ns |    93.59 ns |  0.48 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 1024 | Random             |    19,206.7 ns |    217.53 ns |    96.59 ns |  0.47 |    0.00 |    2 |         - |          NA |
| StdSort                      | 1024 | Random             |    20,373.3 ns |    123.02 ns |    54.62 ns |  0.50 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | Random             |    21,617.2 ns |     98.54 ns |    35.14 ns |  0.53 |    0.00 |    2 |         - |          NA |
| Ips4oSort                    | 1024 | Random             |    19,050.1 ns |    276.59 ns |   144.66 ns |  0.47 |    0.00 |    2 |         - |          NA |
| FluxSort                     | 1024 | Random             |    25,543.5 ns |    279.67 ns |   124.18 ns |  0.63 |    0.00 |    2 |         - |          NA |
| CrumSort                     | 1024 | Random             |    25,509.9 ns |    292.87 ns |   153.18 ns |  0.63 |    0.01 |    2 |         - |          NA |
| BlitSort                     | 1024 | Random             |    26,610.1 ns |    239.87 ns |   125.45 ns |  0.66 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 1024 | Random             |    29,783.1 ns |    496.60 ns |   259.73 ns |  0.73 |    0.01 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **SingleElementMoved** |     **4,519.1 ns** |    **211.01 ns** |    **93.69 ns** |  **1.00** |    **0.00** |    **2** |         **-** |          **NA** |
| QuickSort3way                | 1024 | SingleElementMoved |     8,296.7 ns |    723.79 ns |   321.37 ns |  1.84 |    0.08 |    3 |         - |          NA |
| QuickSortMedian3             | 1024 | SingleElementMoved |     6,559.3 ns |    739.36 ns |   328.28 ns |  1.45 |    0.07 |    3 |         - |          NA |
| QuickSortMedian9             | 1024 | SingleElementMoved |     7,455.3 ns |    611.42 ns |   319.79 ns |  1.65 |    0.07 |    3 |         - |          NA |
| SampleSort                   | 1024 | SingleElementMoved |    25,853.2 ns |  1,308.27 ns |   684.25 ns |  5.72 |    0.18 |    6 |         - |          NA |
| SimdQuickSort                | 1024 | SingleElementMoved |     3,586.1 ns |     55.63 ns |    24.70 ns |  0.79 |    0.02 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | SingleElementMoved |     3,895.7 ns |    475.99 ns |   248.95 ns |  0.86 |    0.05 |    2 |         - |          NA |
| StableQuickSort              | 1024 | SingleElementMoved |    30,862.9 ns |    286.22 ns |   149.70 ns |  6.83 |    0.14 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | SingleElementMoved |    16,162.3 ns |    110.19 ns |    57.63 ns |  3.58 |    0.07 |    5 |         - |          NA |
| DestswapStableQuickSort      | 1024 | SingleElementMoved |    32,602.5 ns |    493.63 ns |   258.18 ns |  7.22 |    0.15 |    7 |         - |          NA |
| IntroSort                    | 1024 | SingleElementMoved |    11,305.7 ns |    489.53 ns |   217.35 ns |  2.50 |    0.07 |    4 |         - |          NA |
| IntroSortDotnet              | 1024 | SingleElementMoved |     5,084.9 ns |    209.65 ns |    93.09 ns |  1.13 |    0.03 |    2 |         - |          NA |
| PDQSort                      | 1024 | SingleElementMoved |     3,090.7 ns |     42.04 ns |    18.67 ns |  0.68 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | SingleElementMoved |     3,621.4 ns |     61.77 ns |    22.03 ns |  0.80 |    0.02 |    2 |         - |          NA |
| Ipnsort                      | 1024 | SingleElementMoved |    14,494.4 ns |    148.90 ns |    77.88 ns |  3.21 |    0.06 |    5 |         - |          NA |
| StdSort                      | 1024 | SingleElementMoved |     3,495.4 ns |    433.23 ns |   192.36 ns |  0.77 |    0.04 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | SingleElementMoved |     7,429.7 ns |    333.54 ns |   174.45 ns |  1.64 |    0.05 |    3 |         - |          NA |
| Ips4oSort                    | 1024 | SingleElementMoved |    11,480.8 ns |    246.53 ns |   128.94 ns |  2.54 |    0.06 |    4 |         - |          NA |
| FluxSort                     | 1024 | SingleElementMoved |     1,371.3 ns |     84.34 ns |    44.11 ns |  0.30 |    0.01 |    1 |         - |          NA |
| CrumSort                     | 1024 | SingleElementMoved |     1,291.6 ns |     83.50 ns |    37.08 ns |  0.29 |    0.01 |    1 |         - |          NA |
| BlitSort                     | 1024 | SingleElementMoved |     1,296.0 ns |    101.27 ns |    44.96 ns |  0.29 |    0.01 |    1 |         - |          NA |
| DotnetSort                   | 1024 | SingleElementMoved |     5,166.0 ns |    222.81 ns |    98.93 ns |  1.14 |    0.03 |    2 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Sorted**             |     **3,454.2 ns** |    **278.64 ns** |   **145.73 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Sorted             |     6,338.7 ns |    442.07 ns |   231.21 ns |  1.84 |    0.10 |    4 |         - |          NA |
| QuickSortMedian3             | 1024 | Sorted             |     4,308.8 ns |     17.75 ns |     9.29 ns |  1.25 |    0.05 |    3 |         - |          NA |
| QuickSortMedian9             | 1024 | Sorted             |     4,630.8 ns |     72.50 ns |    37.92 ns |  1.34 |    0.05 |    3 |         - |          NA |
| SampleSort                   | 1024 | Sorted             |    15,526.1 ns |    452.82 ns |   236.84 ns |  4.50 |    0.19 |    5 |         - |          NA |
| SimdQuickSort                | 1024 | Sorted             |     3,355.0 ns |     43.01 ns |    19.10 ns |  0.97 |    0.04 |    3 |         - |          NA |
| DualPivotQuickSort           | 1024 | Sorted             |     3,405.4 ns |    427.07 ns |   223.37 ns |  0.99 |    0.07 |    3 |         - |          NA |
| StableQuickSort              | 1024 | Sorted             |    30,870.2 ns |    557.15 ns |   247.38 ns |  8.95 |    0.36 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Sorted             |    15,743.9 ns |    304.62 ns |   135.25 ns |  4.56 |    0.18 |    5 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Sorted             |    31,987.6 ns |    453.59 ns |   237.24 ns |  9.27 |    0.37 |    6 |         - |          NA |
| IntroSort                    | 1024 | Sorted             |     3,231.0 ns |    291.35 ns |   152.38 ns |  0.94 |    0.06 |    3 |         - |          NA |
| IntroSortDotnet              | 1024 | Sorted             |     3,645.5 ns |     25.09 ns |    13.12 ns |  1.06 |    0.04 |    3 |         - |          NA |
| PDQSort                      | 1024 | Sorted             |     1,181.3 ns |     12.64 ns |     6.61 ns |  0.34 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | Sorted             |     1,187.1 ns |      8.98 ns |     3.99 ns |  0.34 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 1024 | Sorted             |       506.2 ns |     18.80 ns |     9.83 ns |  0.15 |    0.01 |    1 |         - |          NA |
| StdSort                      | 1024 | Sorted             |     1,219.4 ns |     13.80 ns |     7.22 ns |  0.35 |    0.01 |    2 |         - |          NA |
| BlockQuickSort               | 1024 | Sorted             |     6,220.6 ns |     69.68 ns |    24.85 ns |  1.80 |    0.07 |    4 |         - |          NA |
| Ips4oSort                    | 1024 | Sorted             |       426.7 ns |      8.45 ns |     4.42 ns |  0.12 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 1024 | Sorted             |       598.4 ns |     10.49 ns |     4.66 ns |  0.17 |    0.01 |    1 |         - |          NA |
| CrumSort                     | 1024 | Sorted             |       610.7 ns |      1.93 ns |     0.69 ns |  0.18 |    0.01 |    1 |         - |          NA |
| BlitSort                     | 1024 | Sorted             |       639.2 ns |      4.98 ns |     2.60 ns |  0.19 |    0.01 |    1 |         - |          NA |
| DotnetSort                   | 1024 | Sorted             |     3,225.3 ns |    298.61 ns |   156.18 ns |  0.94 |    0.06 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **Reversed**           |     **3,654.7 ns** |     **49.49 ns** |    **21.97 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 1024 | Reversed           |     6,890.4 ns |    259.87 ns |   135.92 ns |  1.89 |    0.04 |    7 |         - |          NA |
| QuickSortMedian3             | 1024 | Reversed           |     4,550.7 ns |     62.84 ns |    27.90 ns |  1.25 |    0.01 |    6 |         - |          NA |
| QuickSortMedian9             | 1024 | Reversed           |     5,138.3 ns |    172.21 ns |    90.07 ns |  1.41 |    0.02 |    6 |         - |          NA |
| SampleSort                   | 1024 | Reversed           |    16,692.5 ns |    497.22 ns |   260.06 ns |  4.57 |    0.07 |    9 |         - |          NA |
| SimdQuickSort                | 1024 | Reversed           |     4,946.3 ns |     33.97 ns |    12.12 ns |  1.35 |    0.01 |    6 |         - |          NA |
| DualPivotQuickSort           | 1024 | Reversed           |     3,622.3 ns |     34.94 ns |    18.27 ns |  0.99 |    0.01 |    5 |         - |          NA |
| StableQuickSort              | 1024 | Reversed           |    31,673.8 ns |    966.76 ns |   505.63 ns |  8.67 |    0.14 |   10 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | Reversed           |    15,908.6 ns |    237.16 ns |   124.04 ns |  4.35 |    0.04 |    9 |         - |          NA |
| DestswapStableQuickSort      | 1024 | Reversed           |    30,464.4 ns |    709.76 ns |   371.22 ns |  8.34 |    0.11 |   10 |         - |          NA |
| IntroSort                    | 1024 | Reversed           |     3,464.0 ns |    382.08 ns |   199.84 ns |  0.95 |    0.05 |    5 |         - |          NA |
| IntroSortDotnet              | 1024 | Reversed           |     5,476.0 ns |     31.20 ns |    13.85 ns |  1.50 |    0.01 |    6 |         - |          NA |
| PDQSort                      | 1024 | Reversed           |     1,629.2 ns |      7.30 ns |     3.82 ns |  0.45 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | Reversed           |     2,024.3 ns |     18.21 ns |     9.53 ns |  0.55 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 1024 | Reversed           |       738.1 ns |     16.76 ns |     8.77 ns |  0.20 |    0.00 |    1 |         - |          NA |
| StdSort                      | 1024 | Reversed           |     2,577.7 ns |    506.30 ns |   264.81 ns |  0.71 |    0.07 |    4 |         - |          NA |
| BlockQuickSort               | 1024 | Reversed           |     8,454.7 ns |    239.43 ns |   125.23 ns |  2.31 |    0.03 |    8 |         - |          NA |
| Ips4oSort                    | 1024 | Reversed           |       726.8 ns |      4.93 ns |     2.19 ns |  0.20 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 1024 | Reversed           |       884.3 ns |      8.68 ns |     4.54 ns |  0.24 |    0.00 |    1 |         - |          NA |
| CrumSort                     | 1024 | Reversed           |       897.3 ns |     12.03 ns |     5.34 ns |  0.25 |    0.00 |    1 |         - |          NA |
| BlitSort                     | 1024 | Reversed           |       895.6 ns |     28.32 ns |    12.57 ns |  0.25 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 1024 | Reversed           |     5,284.6 ns |    155.21 ns |    81.18 ns |  1.45 |    0.02 |    6 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **PipeOrgan**          |    **83,746.5 ns** |    **436.47 ns** |   **155.65 ns** |  **1.00** |    **0.00** |    **6** |         **-** |          **NA** |
| QuickSort3way                | 1024 | PipeOrgan          |    15,620.0 ns |  1,033.31 ns |   540.44 ns |  0.19 |    0.01 |    3 |         - |          NA |
| QuickSortMedian3             | 1024 | PipeOrgan          |    12,725.3 ns |    540.94 ns |   282.92 ns |  0.15 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 1024 | PipeOrgan          |     9,167.6 ns |    414.44 ns |   216.76 ns |  0.11 |    0.00 |    2 |         - |          NA |
| SampleSort                   | 1024 | PipeOrgan          |    17,199.3 ns |    783.97 ns |   410.03 ns |  0.21 |    0.00 |    3 |         - |          NA |
| SimdQuickSort                | 1024 | PipeOrgan          |     6,076.3 ns |    340.80 ns |   151.32 ns |  0.07 |    0.00 |    2 |         - |          NA |
| DualPivotQuickSort           | 1024 | PipeOrgan          |     7,765.6 ns |    246.37 ns |   128.85 ns |  0.09 |    0.00 |    2 |         - |          NA |
| StableQuickSort              | 1024 | PipeOrgan          |    30,998.8 ns |    321.56 ns |   142.77 ns |  0.37 |    0.00 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | PipeOrgan          |    15,907.2 ns |     58.82 ns |    26.12 ns |  0.19 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 1024 | PipeOrgan          |    40,350.4 ns |    220.00 ns |    97.68 ns |  0.48 |    0.00 |    5 |         - |          NA |
| IntroSort                    | 1024 | PipeOrgan          |    21,744.2 ns |  5,027.21 ns | 2,232.11 ns |  0.26 |    0.02 |    3 |         - |          NA |
| IntroSortDotnet              | 1024 | PipeOrgan          |    11,035.1 ns |    307.38 ns |   136.48 ns |  0.13 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 1024 | PipeOrgan          |     6,661.2 ns |    358.81 ns |   187.66 ns |  0.08 |    0.00 |    2 |         - |          NA |
| PDQSortBranchless            | 1024 | PipeOrgan          |    14,106.6 ns |  5,200.15 ns | 2,719.78 ns |  0.17 |    0.03 |    3 |         - |          NA |
| Ipnsort                      | 1024 | PipeOrgan          |    16,707.9 ns |     91.53 ns |    47.87 ns |  0.20 |    0.00 |    3 |         - |          NA |
| StdSort                      | 1024 | PipeOrgan          |    18,206.3 ns |    134.90 ns |    59.90 ns |  0.22 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 1024 | PipeOrgan          |    17,905.1 ns |    166.59 ns |    87.13 ns |  0.21 |    0.00 |    3 |         - |          NA |
| Ips4oSort                    | 1024 | PipeOrgan          |    13,416.8 ns |    221.21 ns |   115.70 ns |  0.16 |    0.00 |    3 |         - |          NA |
| FluxSort                     | 1024 | PipeOrgan          |     2,283.7 ns |     40.90 ns |    14.59 ns |  0.03 |    0.00 |    1 |         - |          NA |
| CrumSort                     | 1024 | PipeOrgan          |     2,308.5 ns |      8.67 ns |     3.85 ns |  0.03 |    0.00 |    1 |         - |          NA |
| BlitSort                     | 1024 | PipeOrgan          |     2,306.7 ns |     26.77 ns |    11.88 ns |  0.03 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 1024 | PipeOrgan          |    12,842.6 ns |    542.04 ns |   240.67 ns |  0.15 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **1024** | **ManyDuplicates**     |    **26,584.7 ns** |    **150.29 ns** |    **78.61 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 1024 | ManyDuplicates     |    20,226.2 ns |    303.86 ns |   158.93 ns |  0.76 |    0.01 |    5 |         - |          NA |
| QuickSortMedian3             | 1024 | ManyDuplicates     |    29,853.8 ns |    150.73 ns |    66.92 ns |  1.12 |    0.00 |    5 |         - |          NA |
| QuickSortMedian9             | 1024 | ManyDuplicates     |    31,391.4 ns |    406.95 ns |   145.12 ns |  1.18 |    0.01 |    5 |         - |          NA |
| SampleSort                   | 1024 | ManyDuplicates     |    39,181.6 ns |    547.03 ns |   286.11 ns |  1.47 |    0.01 |    5 |         - |          NA |
| SimdQuickSort                | 1024 | ManyDuplicates     |     4,753.8 ns |    365.19 ns |   162.15 ns |  0.18 |    0.01 |    1 |         - |          NA |
| DualPivotQuickSort           | 1024 | ManyDuplicates     |    19,565.5 ns |    195.46 ns |    86.79 ns |  0.74 |    0.00 |    5 |         - |          NA |
| StableQuickSort              | 1024 | ManyDuplicates     |    45,115.6 ns |    796.63 ns |   353.71 ns |  1.70 |    0.01 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 1024 | ManyDuplicates     |    21,052.8 ns |    317.34 ns |   165.98 ns |  0.79 |    0.01 |    5 |         - |          NA |
| DestswapStableQuickSort      | 1024 | ManyDuplicates     |    35,318.1 ns |    199.15 ns |    88.42 ns |  1.33 |    0.00 |    5 |         - |          NA |
| IntroSort                    | 1024 | ManyDuplicates     |    24,976.3 ns |    256.63 ns |   134.22 ns |  0.94 |    0.01 |    5 |         - |          NA |
| IntroSortDotnet              | 1024 | ManyDuplicates     |    25,313.7 ns |    302.94 ns |   134.51 ns |  0.95 |    0.01 |    5 |         - |          NA |
| PDQSort                      | 1024 | ManyDuplicates     |    20,399.6 ns |    233.20 ns |   121.97 ns |  0.77 |    0.00 |    5 |         - |          NA |
| PDQSortBranchless            | 1024 | ManyDuplicates     |     9,656.8 ns |    344.45 ns |   180.15 ns |  0.36 |    0.01 |    2 |         - |          NA |
| Ipnsort                      | 1024 | ManyDuplicates     |    15,258.8 ns |    186.08 ns |    97.32 ns |  0.57 |    0.00 |    4 |         - |          NA |
| StdSort                      | 1024 | ManyDuplicates     |    15,107.1 ns |    195.34 ns |   102.17 ns |  0.57 |    0.00 |    4 |         - |          NA |
| BlockQuickSort               | 1024 | ManyDuplicates     |    16,100.5 ns |     94.15 ns |    41.80 ns |  0.61 |    0.00 |    4 |         - |          NA |
| Ips4oSort                    | 1024 | ManyDuplicates     |    12,244.0 ns |    200.84 ns |   105.05 ns |  0.46 |    0.00 |    3 |         - |          NA |
| FluxSort                     | 1024 | ManyDuplicates     |    24,719.1 ns |    460.53 ns |   240.87 ns |  0.93 |    0.01 |    5 |         - |          NA |
| CrumSort                     | 1024 | ManyDuplicates     |    24,716.4 ns |    108.38 ns |    38.65 ns |  0.93 |    0.00 |    5 |         - |          NA |
| BlitSort                     | 1024 | ManyDuplicates     |    25,821.4 ns |    227.09 ns |   118.77 ns |  0.97 |    0.01 |    5 |         - |          NA |
| DotnetSort                   | 1024 | ManyDuplicates     |    23,260.2 ns |    269.45 ns |   140.93 ns |  0.87 |    0.01 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Random**             |   **186,539.7 ns** |  **2,258.27 ns** | **1,181.12 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Random             |   203,593.5 ns |  2,839.65 ns | 1,485.19 ns |  1.09 |    0.01 |    3 |         - |          NA |
| QuickSortMedian3             | 4096 | Random             |   194,587.3 ns |  1,617.18 ns |   845.82 ns |  1.04 |    0.01 |    3 |         - |          NA |
| QuickSortMedian9             | 4096 | Random             |   206,152.8 ns |  1,652.78 ns |   864.44 ns |  1.11 |    0.01 |    3 |         - |          NA |
| SampleSort                   | 4096 | Random             |   234,889.0 ns |    731.36 ns |   382.52 ns |  1.26 |    0.01 |    3 |         - |          NA |
| SimdQuickSort                | 4096 | Random             |    19,464.0 ns |    193.63 ns |    85.97 ns |  0.10 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort           | 4096 | Random             |   144,278.5 ns |    850.48 ns |   377.62 ns |  0.77 |    0.00 |    3 |         - |          NA |
| StableQuickSort              | 4096 | Random             |   468,615.4 ns |  2,044.91 ns | 1,069.53 ns |  2.51 |    0.02 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Random             |   242,951.0 ns |  3,558.50 ns | 1,861.17 ns |  1.30 |    0.01 |    3 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Random             |   390,206.1 ns |  1,506.13 ns |   787.73 ns |  2.09 |    0.01 |    4 |         - |          NA |
| IntroSort                    | 4096 | Random             |   160,421.2 ns |    790.05 ns |   413.21 ns |  0.86 |    0.01 |    3 |         - |          NA |
| IntroSortDotnet              | 4096 | Random             |   155,591.2 ns |    783.35 ns |   409.71 ns |  0.83 |    0.01 |    3 |         - |          NA |
| PDQSort                      | 4096 | Random             |   148,879.8 ns |  1,023.93 ns |   454.63 ns |  0.80 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 4096 | Random             |    83,994.5 ns |  1,270.75 ns |   564.22 ns |  0.45 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 4096 | Random             |    90,833.3 ns |    440.84 ns |   195.73 ns |  0.49 |    0.00 |    2 |         - |          NA |
| StdSort                      | 4096 | Random             |    89,073.9 ns |    356.27 ns |   186.33 ns |  0.48 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 4096 | Random             |    95,692.6 ns |    519.68 ns |   230.74 ns |  0.51 |    0.00 |    2 |         - |          NA |
| Ips4oSort                    | 4096 | Random             |    78,939.0 ns |    894.74 ns |   397.27 ns |  0.42 |    0.00 |    2 |         - |          NA |
| FluxSort                     | 4096 | Random             |   109,741.4 ns |    747.93 ns |   391.18 ns |  0.59 |    0.00 |    2 |         - |          NA |
| CrumSort                     | 4096 | Random             |   109,856.2 ns |    416.96 ns |   218.08 ns |  0.59 |    0.00 |    2 |         - |          NA |
| BlitSort                     | 4096 | Random             |   118,040.2 ns |    577.64 ns |   256.47 ns |  0.63 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 4096 | Random             |   144,233.4 ns |  1,027.29 ns |   537.29 ns |  0.77 |    0.01 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **SingleElementMoved** |    **19,017.4 ns** |    **329.08 ns** |   **146.11 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 4096 | SingleElementMoved |    33,057.3 ns |    979.81 ns |   435.04 ns |  1.74 |    0.02 |    4 |         - |          NA |
| QuickSortMedian3             | 4096 | SingleElementMoved |    26,636.3 ns |    627.14 ns |   328.01 ns |  1.40 |    0.02 |    4 |         - |          NA |
| QuickSortMedian9             | 4096 | SingleElementMoved |    30,594.1 ns |    395.41 ns |   175.56 ns |  1.61 |    0.01 |    4 |         - |          NA |
| SampleSort                   | 4096 | SingleElementMoved |   130,637.9 ns |  3,287.27 ns | 1,719.31 ns |  6.87 |    0.10 |    7 |         - |          NA |
| SimdQuickSort                | 4096 | SingleElementMoved |    40,593.1 ns |    169.51 ns |    88.66 ns |  2.13 |    0.02 |    4 |         - |          NA |
| DualPivotQuickSort           | 4096 | SingleElementMoved |    16,872.2 ns |  1,554.69 ns |   813.13 ns |  0.89 |    0.04 |    4 |         - |          NA |
| StableQuickSort              | 4096 | SingleElementMoved |   148,898.7 ns |  2,466.25 ns | 1,095.03 ns |  7.83 |    0.08 |    7 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | SingleElementMoved |    71,549.6 ns |    213.71 ns |   111.78 ns |  3.76 |    0.03 |    6 |         - |          NA |
| DestswapStableQuickSort      | 4096 | SingleElementMoved |   142,005.6 ns |  1,241.16 ns |   649.15 ns |  7.47 |    0.06 |    7 |         - |          NA |
| IntroSort                    | 4096 | SingleElementMoved |    51,711.6 ns |    325.68 ns |   170.34 ns |  2.72 |    0.02 |    5 |         - |          NA |
| IntroSortDotnet              | 4096 | SingleElementMoved |    21,125.5 ns |    418.23 ns |   218.74 ns |  1.11 |    0.01 |    4 |         - |          NA |
| PDQSort                      | 4096 | SingleElementMoved |    12,005.7 ns |    229.73 ns |   120.15 ns |  0.63 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 4096 | SingleElementMoved |    13,760.0 ns |    224.98 ns |    99.89 ns |  0.72 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 4096 | SingleElementMoved |    70,293.3 ns |    378.84 ns |   198.14 ns |  3.70 |    0.03 |    6 |         - |          NA |
| StdSort                      | 4096 | SingleElementMoved |    12,982.7 ns |     69.57 ns |    30.89 ns |  0.68 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | SingleElementMoved |    34,309.6 ns |  1,070.44 ns |   475.28 ns |  1.80 |    0.03 |    4 |         - |          NA |
| Ips4oSort                    | 4096 | SingleElementMoved |    51,465.5 ns |    237.58 ns |   124.26 ns |  2.71 |    0.02 |    5 |         - |          NA |
| FluxSort                     | 4096 | SingleElementMoved |     4,381.8 ns |    298.32 ns |   132.46 ns |  0.23 |    0.01 |    1 |         - |          NA |
| CrumSort                     | 4096 | SingleElementMoved |     5,634.1 ns |    174.09 ns |    77.30 ns |  0.30 |    0.00 |    2 |         - |          NA |
| BlitSort                     | 4096 | SingleElementMoved |     5,698.5 ns |    292.62 ns |   153.05 ns |  0.30 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 4096 | SingleElementMoved |    24,628.4 ns |    626.12 ns |   278.00 ns |  1.30 |    0.02 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Sorted**             |    **15,690.0 ns** |    **175.06 ns** |    **91.56 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Sorted             |    27,899.2 ns |  1,765.28 ns |   923.28 ns |  1.78 |    0.06 |    5 |         - |          NA |
| QuickSortMedian3             | 4096 | Sorted             |    19,650.8 ns |    220.45 ns |   115.30 ns |  1.25 |    0.01 |    4 |         - |          NA |
| QuickSortMedian9             | 4096 | Sorted             |    20,834.1 ns |    205.18 ns |    91.10 ns |  1.33 |    0.01 |    4 |         - |          NA |
| SampleSort                   | 4096 | Sorted             |    69,730.9 ns |  1,157.81 ns |   605.56 ns |  4.44 |    0.04 |    7 |         - |          NA |
| SimdQuickSort                | 4096 | Sorted             |    42,416.0 ns |    151.78 ns |    79.39 ns |  2.70 |    0.02 |    6 |         - |          NA |
| DualPivotQuickSort           | 4096 | Sorted             |    16,013.3 ns |  1,418.11 ns |   741.70 ns |  1.02 |    0.04 |    4 |         - |          NA |
| StableQuickSort              | 4096 | Sorted             |   147,749.9 ns |  1,967.54 ns |   873.60 ns |  9.42 |    0.07 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Sorted             |    70,682.2 ns |    307.54 ns |   136.55 ns |  4.51 |    0.03 |    7 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Sorted             |   140,427.7 ns |    356.99 ns |   186.72 ns |  8.95 |    0.05 |    8 |         - |          NA |
| IntroSort                    | 4096 | Sorted             |    14,943.4 ns |    273.17 ns |   121.29 ns |  0.95 |    0.01 |    4 |         - |          NA |
| IntroSortDotnet              | 4096 | Sorted             |    17,267.0 ns |    308.90 ns |   161.56 ns |  1.10 |    0.01 |    4 |         - |          NA |
| PDQSort                      | 4096 | Sorted             |     4,559.1 ns |     19.15 ns |     6.83 ns |  0.29 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 4096 | Sorted             |     4,543.9 ns |     20.29 ns |     7.24 ns |  0.29 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 4096 | Sorted             |     1,783.2 ns |     25.18 ns |    11.18 ns |  0.11 |    0.00 |    1 |         - |          NA |
| StdSort                      | 4096 | Sorted             |     4,646.3 ns |    236.35 ns |   104.94 ns |  0.30 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | Sorted             |    30,549.0 ns |    175.33 ns |    91.70 ns |  1.95 |    0.01 |    5 |         - |          NA |
| Ips4oSort                    | 4096 | Sorted             |     1,495.1 ns |     16.68 ns |     7.41 ns |  0.10 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 4096 | Sorted             |     2,220.6 ns |      9.95 ns |     3.55 ns |  0.14 |    0.00 |    2 |         - |          NA |
| CrumSort                     | 4096 | Sorted             |     2,249.9 ns |     24.67 ns |    10.96 ns |  0.14 |    0.00 |    2 |         - |          NA |
| BlitSort                     | 4096 | Sorted             |     2,217.7 ns |     23.34 ns |    10.36 ns |  0.14 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 4096 | Sorted             |    15,083.6 ns |    181.88 ns |    80.76 ns |  0.96 |    0.01 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **Reversed**           |    **16,888.5 ns** |    **402.61 ns** |   **178.76 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 4096 | Reversed           |    30,757.0 ns |  2,250.15 ns | 1,176.87 ns |  1.82 |    0.07 |    6 |         - |          NA |
| QuickSortMedian3             | 4096 | Reversed           |    20,890.8 ns |    192.75 ns |    85.58 ns |  1.24 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9             | 4096 | Reversed           |    22,007.0 ns |    467.29 ns |   244.40 ns |  1.30 |    0.02 |    5 |         - |          NA |
| SampleSort                   | 4096 | Reversed           |    74,006.5 ns |  2,210.30 ns | 1,156.03 ns |  4.38 |    0.08 |    8 |         - |          NA |
| SimdQuickSort                | 4096 | Reversed           |    27,611.3 ns |    357.15 ns |   158.58 ns |  1.64 |    0.02 |    6 |         - |          NA |
| DualPivotQuickSort           | 4096 | Reversed           |    16,934.8 ns |  1,238.85 ns |   647.95 ns |  1.00 |    0.04 |    4 |         - |          NA |
| StableQuickSort              | 4096 | Reversed           |   151,270.5 ns |  1,692.97 ns |   885.45 ns |  8.96 |    0.10 |    9 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | Reversed           |    70,910.8 ns |    939.74 ns |   491.50 ns |  4.20 |    0.05 |    8 |         - |          NA |
| DestswapStableQuickSort      | 4096 | Reversed           |   135,658.8 ns |  3,119.47 ns | 1,631.54 ns |  8.03 |    0.12 |    9 |         - |          NA |
| IntroSort                    | 4096 | Reversed           |    15,526.1 ns |    419.87 ns |   186.43 ns |  0.92 |    0.01 |    4 |         - |          NA |
| IntroSortDotnet              | 4096 | Reversed           |    26,919.7 ns |    643.75 ns |   336.69 ns |  1.59 |    0.02 |    6 |         - |          NA |
| PDQSort                      | 4096 | Reversed           |     6,416.7 ns |    337.25 ns |   176.39 ns |  0.38 |    0.01 |    2 |         - |          NA |
| PDQSortBranchless            | 4096 | Reversed           |     7,854.9 ns |    260.90 ns |   136.45 ns |  0.47 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 4096 | Reversed           |     2,805.5 ns |     14.42 ns |     6.40 ns |  0.17 |    0.00 |    1 |         - |          NA |
| StdSort                      | 4096 | Reversed           |     9,205.5 ns |    380.19 ns |   198.85 ns |  0.55 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | Reversed           |    37,560.9 ns |    299.41 ns |   156.60 ns |  2.22 |    0.02 |    7 |         - |          NA |
| Ips4oSort                    | 4096 | Reversed           |     2,697.3 ns |     26.36 ns |    11.70 ns |  0.16 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 4096 | Reversed           |     3,332.7 ns |     18.73 ns |     6.68 ns |  0.20 |    0.00 |    1 |         - |          NA |
| CrumSort                     | 4096 | Reversed           |     3,376.8 ns |     24.35 ns |     8.69 ns |  0.20 |    0.00 |    1 |         - |          NA |
| BlitSort                     | 4096 | Reversed           |     3,382.0 ns |     31.13 ns |    11.10 ns |  0.20 |    0.00 |    1 |         - |          NA |
| DotnetSort                   | 4096 | Reversed           |    27,883.9 ns |  2,465.68 ns | 1,094.78 ns |  1.65 |    0.06 |    6 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **PipeOrgan**          | **1,215,812.0 ns** |  **2,573.35 ns** | **1,142.58 ns** | **1.000** |    **0.00** |    **7** |         **-** |          **NA** |
| QuickSort3way                | 4096 | PipeOrgan          |    82,179.6 ns |  8,637.45 ns | 4,517.55 ns | 0.068 |    0.00 |    4 |         - |          NA |
| QuickSortMedian3             | 4096 | PipeOrgan          |    64,416.5 ns |    981.40 ns |   513.29 ns | 0.053 |    0.00 |    4 |         - |          NA |
| QuickSortMedian9             | 4096 | PipeOrgan          |    42,916.4 ns |  1,894.56 ns |   990.89 ns | 0.035 |    0.00 |    3 |         - |          NA |
| SampleSort                   | 4096 | PipeOrgan          |    74,682.3 ns |  1,375.65 ns |   610.80 ns | 0.061 |    0.00 |    4 |         - |          NA |
| SimdQuickSort                | 4096 | PipeOrgan          |    30,760.8 ns |    163.20 ns |    72.46 ns | 0.025 |    0.00 |    3 |         - |          NA |
| DualPivotQuickSort           | 4096 | PipeOrgan          |    36,099.6 ns |  1,008.21 ns |   447.65 ns | 0.030 |    0.00 |    3 |         - |          NA |
| StableQuickSort              | 4096 | PipeOrgan          |   149,408.8 ns |  1,202.20 ns |   533.78 ns | 0.123 |    0.00 |    6 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | PipeOrgan          |    71,281.5 ns |    406.18 ns |   212.44 ns | 0.059 |    0.00 |    4 |         - |          NA |
| DestswapStableQuickSort      | 4096 | PipeOrgan          |   179,454.2 ns |  3,621.10 ns | 1,607.79 ns | 0.148 |    0.00 |    6 |         - |          NA |
| IntroSort                    | 4096 | PipeOrgan          |   105,071.2 ns |    814.32 ns |   361.56 ns | 0.086 |    0.00 |    5 |         - |          NA |
| IntroSortDotnet              | 4096 | PipeOrgan          |    64,654.6 ns |    998.07 ns |   443.15 ns | 0.053 |    0.00 |    4 |         - |          NA |
| PDQSort                      | 4096 | PipeOrgan          |    31,705.7 ns |    852.87 ns |   446.07 ns | 0.026 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 4096 | PipeOrgan          |    47,510.0 ns |    632.97 ns |   331.06 ns | 0.039 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 4096 | PipeOrgan          |    83,632.2 ns |    341.56 ns |   151.65 ns | 0.069 |    0.00 |    4 |         - |          NA |
| StdSort                      | 4096 | PipeOrgan          |   101,552.5 ns |    616.54 ns |   273.75 ns | 0.084 |    0.00 |    5 |         - |          NA |
| BlockQuickSort               | 4096 | PipeOrgan          |    80,330.9 ns |    923.13 ns |   409.87 ns | 0.066 |    0.00 |    4 |         - |          NA |
| Ips4oSort                    | 4096 | PipeOrgan          |    59,962.8 ns |    266.35 ns |   118.26 ns | 0.049 |    0.00 |    4 |         - |          NA |
| FluxSort                     | 4096 | PipeOrgan          |     8,905.3 ns |    341.60 ns |   178.66 ns | 0.007 |    0.00 |    1 |         - |          NA |
| CrumSort                     | 4096 | PipeOrgan          |    10,740.0 ns |    251.98 ns |   131.79 ns | 0.009 |    0.00 |    2 |         - |          NA |
| BlitSort                     | 4096 | PipeOrgan          |    10,732.4 ns |    257.69 ns |   134.78 ns | 0.009 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 4096 | PipeOrgan          |    72,581.5 ns |  4,800.36 ns | 2,510.68 ns | 0.060 |    0.00 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **4096** | **ManyDuplicates**     |   **103,467.2 ns** |    **301.65 ns** |   **133.93 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 4096 | ManyDuplicates     |    74,957.2 ns |    645.70 ns |   286.69 ns |  0.72 |    0.00 |    4 |         - |          NA |
| QuickSortMedian3             | 4096 | ManyDuplicates     |   113,209.6 ns |    436.18 ns |   193.66 ns |  1.09 |    0.00 |    5 |         - |          NA |
| QuickSortMedian9             | 4096 | ManyDuplicates     |   119,124.4 ns |    755.06 ns |   394.91 ns |  1.15 |    0.00 |    5 |         - |          NA |
| SampleSort                   | 4096 | ManyDuplicates     |   155,161.8 ns |    973.36 ns |   432.18 ns |  1.50 |    0.00 |    5 |         - |          NA |
| SimdQuickSort                | 4096 | ManyDuplicates     |    19,611.3 ns |    321.35 ns |   168.07 ns |  0.19 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort           | 4096 | ManyDuplicates     |    73,108.5 ns |    961.13 ns |   502.69 ns |  0.71 |    0.00 |    4 |         - |          NA |
| StableQuickSort              | 4096 | ManyDuplicates     |   177,484.5 ns |  4,187.91 ns | 2,190.36 ns |  1.72 |    0.02 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 4096 | ManyDuplicates     |    77,978.5 ns |    447.31 ns |   159.51 ns |  0.75 |    0.00 |    4 |         - |          NA |
| DestswapStableQuickSort      | 4096 | ManyDuplicates     |   130,060.9 ns |    956.95 ns |   500.50 ns |  1.26 |    0.00 |    5 |         - |          NA |
| IntroSort                    | 4096 | ManyDuplicates     |    96,885.4 ns |    443.47 ns |   196.91 ns |  0.94 |    0.00 |    5 |         - |          NA |
| IntroSortDotnet              | 4096 | ManyDuplicates     |   103,204.7 ns |    558.79 ns |   248.11 ns |  1.00 |    0.00 |    5 |         - |          NA |
| PDQSort                      | 4096 | ManyDuplicates     |    74,068.4 ns |    410.97 ns |   214.94 ns |  0.72 |    0.00 |    4 |         - |          NA |
| PDQSortBranchless            | 4096 | ManyDuplicates     |    26,818.5 ns |    188.23 ns |    83.57 ns |  0.26 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 4096 | ManyDuplicates     |    48,665.7 ns |    132.03 ns |    69.05 ns |  0.47 |    0.00 |    3 |         - |          NA |
| StdSort                      | 4096 | ManyDuplicates     |    42,436.0 ns |    319.97 ns |   167.35 ns |  0.41 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 4096 | ManyDuplicates     |    55,955.6 ns |    498.43 ns |   260.69 ns |  0.54 |    0.00 |    3 |         - |          NA |
| Ips4oSort                    | 4096 | ManyDuplicates     |    41,739.0 ns |  1,017.28 ns |   362.77 ns |  0.40 |    0.00 |    3 |         - |          NA |
| FluxSort                     | 4096 | ManyDuplicates     |    35,562.2 ns |    173.21 ns |    90.59 ns |  0.34 |    0.00 |    3 |         - |          NA |
| CrumSort                     | 4096 | ManyDuplicates     |    34,368.7 ns |    216.60 ns |   113.28 ns |  0.33 |    0.00 |    3 |         - |          NA |
| BlitSort                     | 4096 | ManyDuplicates     |    42,097.1 ns |    203.56 ns |    72.59 ns |  0.41 |    0.00 |    3 |         - |          NA |
| DotnetSort                   | 4096 | ManyDuplicates     |    94,228.4 ns |    363.43 ns |   161.36 ns |  0.91 |    0.00 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Random**             |   **393,311.9 ns** |  **1,232.95 ns** |   **644.85 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Random             |   432,614.1 ns |  2,336.20 ns | 1,037.29 ns |  1.10 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | Random             |   412,191.0 ns |  2,457.80 ns | 1,285.48 ns |  1.05 |    0.00 |    3 |         - |          NA |
| QuickSortMedian9             | 8192 | Random             |   438,220.4 ns |  1,433.39 ns |   636.43 ns |  1.11 |    0.00 |    3 |         - |          NA |
| SampleSort                   | 8192 | Random             |   499,072.5 ns |  1,166.92 ns |   518.12 ns |  1.27 |    0.00 |    3 |         - |          NA |
| SimdQuickSort                | 8192 | Random             |    42,176.3 ns |    371.18 ns |   194.13 ns |  0.11 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort           | 8192 | Random             |   311,726.3 ns |  1,241.77 ns |   551.35 ns |  0.79 |    0.00 |    3 |         - |          NA |
| StableQuickSort              | 8192 | Random             | 1,011,052.2 ns |  1,783.20 ns |   791.75 ns |  2.57 |    0.00 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Random             |   519,200.8 ns |  2,621.99 ns | 1,164.18 ns |  1.32 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Random             |   823,367.9 ns |  1,867.02 ns |   828.97 ns |  2.09 |    0.00 |    4 |         - |          NA |
| IntroSort                    | 8192 | Random             |   346,596.8 ns |  1,397.40 ns |   730.87 ns |  0.88 |    0.00 |    3 |         - |          NA |
| IntroSortDotnet              | 8192 | Random             |   336,748.1 ns |  1,121.53 ns |   497.97 ns |  0.86 |    0.00 |    3 |         - |          NA |
| PDQSort                      | 8192 | Random             |   323,178.5 ns |    777.31 ns |   406.55 ns |  0.82 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | Random             |   174,049.9 ns |    618.79 ns |   274.75 ns |  0.44 |    0.00 |    2 |         - |          NA |
| Ipnsort                      | 8192 | Random             |   195,064.7 ns |    336.47 ns |   149.39 ns |  0.50 |    0.00 |    2 |         - |          NA |
| StdSort                      | 8192 | Random             |   187,053.2 ns |    938.92 ns |   491.07 ns |  0.48 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | Random             |   200,951.9 ns |  4,804.28 ns | 2,133.13 ns |  0.51 |    0.01 |    2 |         - |          NA |
| Ips4oSort                    | 8192 | Random             |   173,789.1 ns |    803.96 ns |   356.96 ns |  0.44 |    0.00 |    2 |         - |          NA |
| FluxSort                     | 8192 | Random             |   227,303.0 ns |  1,941.91 ns | 1,015.66 ns |  0.58 |    0.00 |    2 |         - |          NA |
| CrumSort                     | 8192 | Random             |   227,021.5 ns |    603.02 ns |   267.74 ns |  0.58 |    0.00 |    2 |         - |          NA |
| BlitSort                     | 8192 | Random             |   250,344.8 ns |  1,137.81 ns |   505.19 ns |  0.64 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | Random             |   315,316.4 ns |  1,264.55 ns |   661.38 ns |  0.80 |    0.00 |    3 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **SingleElementMoved** |    **40,337.3 ns** |    **696.49 ns** |   **309.24 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | SingleElementMoved |    69,871.9 ns |  3,367.19 ns | 1,495.06 ns |  1.73 |    0.04 |    5 |         - |          NA |
| QuickSortMedian3             | 8192 | SingleElementMoved |    54,537.6 ns |    731.80 ns |   382.75 ns |  1.35 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9             | 8192 | SingleElementMoved |    62,980.3 ns |    781.86 ns |   408.93 ns |  1.56 |    0.01 |    5 |         - |          NA |
| SampleSort                   | 8192 | SingleElementMoved |   321,703.5 ns |  3,360.55 ns | 1,757.63 ns |  7.98 |    0.07 |    8 |         - |          NA |
| SimdQuickSort                | 8192 | SingleElementMoved |   140,315.5 ns |  2,227.64 ns | 1,165.10 ns |  3.48 |    0.04 |    7 |         - |          NA |
| DualPivotQuickSort           | 8192 | SingleElementMoved |    39,780.4 ns |  1,843.84 ns |   964.36 ns |  0.99 |    0.02 |    4 |         - |          NA |
| StableQuickSort              | 8192 | SingleElementMoved |   323,519.8 ns |  3,511.00 ns | 1,836.32 ns |  8.02 |    0.07 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | SingleElementMoved |   151,283.4 ns |    341.53 ns |   151.64 ns |  3.75 |    0.03 |    7 |         - |          NA |
| DestswapStableQuickSort      | 8192 | SingleElementMoved |   295,041.1 ns |  2,030.75 ns |   901.67 ns |  7.31 |    0.06 |    8 |         - |          NA |
| IntroSort                    | 8192 | SingleElementMoved |   108,561.7 ns |    716.77 ns |   318.25 ns |  2.69 |    0.02 |    6 |         - |          NA |
| IntroSortDotnet              | 8192 | SingleElementMoved |    44,291.7 ns |    234.68 ns |   122.74 ns |  1.10 |    0.01 |    4 |         - |          NA |
| PDQSort                      | 8192 | SingleElementMoved |    23,696.0 ns |    353.65 ns |   184.97 ns |  0.59 |    0.01 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | SingleElementMoved |    26,954.4 ns |    281.50 ns |   124.99 ns |  0.67 |    0.01 |    3 |         - |          NA |
| Ipnsort                      | 8192 | SingleElementMoved |   153,918.4 ns |    813.91 ns |   361.38 ns |  3.82 |    0.03 |    7 |         - |          NA |
| StdSort                      | 8192 | SingleElementMoved |    25,257.5 ns |    469.32 ns |   245.47 ns |  0.63 |    0.01 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | SingleElementMoved |    73,557.9 ns |  1,093.87 ns |   572.11 ns |  1.82 |    0.02 |    5 |         - |          NA |
| Ips4oSort                    | 8192 | SingleElementMoved |    98,577.9 ns |    693.46 ns |   362.69 ns |  2.44 |    0.02 |    6 |         - |          NA |
| FluxSort                     | 8192 | SingleElementMoved |     8,604.8 ns |    287.64 ns |   150.44 ns |  0.21 |    0.00 |    1 |         - |          NA |
| CrumSort                     | 8192 | SingleElementMoved |    11,366.1 ns |    605.58 ns |   316.73 ns |  0.28 |    0.01 |    2 |         - |          NA |
| BlitSort                     | 8192 | SingleElementMoved |    11,148.8 ns |    221.93 ns |    98.54 ns |  0.28 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | SingleElementMoved |    53,253.0 ns |    326.22 ns |   116.33 ns |  1.32 |    0.01 |    5 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Sorted**             |    **33,602.8 ns** |    **375.38 ns** |   **196.33 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Sorted             |    59,493.1 ns |  3,961.57 ns | 2,071.98 ns |  1.77 |    0.06 |    5 |         - |          NA |
| QuickSortMedian3             | 8192 | Sorted             |    41,437.3 ns |    139.99 ns |    62.16 ns |  1.23 |    0.01 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | Sorted             |    44,091.2 ns |    612.95 ns |   320.58 ns |  1.31 |    0.01 |    4 |         - |          NA |
| SampleSort                   | 8192 | Sorted             |   200,451.2 ns |  9,821.47 ns | 5,136.82 ns |  5.97 |    0.15 |    7 |         - |          NA |
| SimdQuickSort                | 8192 | Sorted             |   143,964.1 ns |    612.64 ns |   272.02 ns |  4.28 |    0.02 |    6 |         - |          NA |
| DualPivotQuickSort           | 8192 | Sorted             |    37,718.9 ns |  2,289.21 ns | 1,197.30 ns |  1.12 |    0.03 |    4 |         - |          NA |
| StableQuickSort              | 8192 | Sorted             |   324,225.5 ns |  2,780.11 ns | 1,454.05 ns |  9.65 |    0.07 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Sorted             |   149,440.5 ns |  1,609.86 ns |   841.99 ns |  4.45 |    0.03 |    6 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Sorted             |   292,784.2 ns |    673.73 ns |   299.14 ns |  8.71 |    0.05 |    8 |         - |          NA |
| IntroSort                    | 8192 | Sorted             |    32,159.6 ns |    378.66 ns |   198.05 ns |  0.96 |    0.01 |    4 |         - |          NA |
| IntroSortDotnet              | 8192 | Sorted             |    36,692.4 ns |    349.08 ns |   182.58 ns |  1.09 |    0.01 |    4 |         - |          NA |
| PDQSort                      | 8192 | Sorted             |     9,007.2 ns |     55.26 ns |    19.71 ns |  0.27 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | Sorted             |     9,245.5 ns |    267.63 ns |   118.83 ns |  0.28 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 8192 | Sorted             |     3,575.1 ns |    177.13 ns |    78.65 ns |  0.11 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Sorted             |     9,303.9 ns |    109.74 ns |    57.40 ns |  0.28 |    0.00 |    3 |         - |          NA |
| BlockQuickSort               | 8192 | Sorted             |    65,980.7 ns |    330.36 ns |   146.68 ns |  1.96 |    0.01 |    5 |         - |          NA |
| Ips4oSort                    | 8192 | Sorted             |     2,969.1 ns |     38.88 ns |    17.26 ns |  0.09 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 8192 | Sorted             |     4,383.7 ns |    148.36 ns |    77.59 ns |  0.13 |    0.00 |    2 |         - |          NA |
| CrumSort                     | 8192 | Sorted             |     4,345.6 ns |     15.35 ns |     6.81 ns |  0.13 |    0.00 |    2 |         - |          NA |
| BlitSort                     | 8192 | Sorted             |     4,381.5 ns |     47.80 ns |    17.05 ns |  0.13 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | Sorted             |    34,643.0 ns |  4,011.51 ns | 2,098.10 ns |  1.03 |    0.06 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **Reversed**           |    **36,713.2 ns** |    **593.85 ns** |   **310.60 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 8192 | Reversed           |    65,006.7 ns |  4,954.53 ns | 2,199.84 ns |  1.77 |    0.06 |    6 |         - |          NA |
| QuickSortMedian3             | 8192 | Reversed           |    43,860.6 ns |    164.93 ns |    86.26 ns |  1.19 |    0.01 |    5 |         - |          NA |
| QuickSortMedian9             | 8192 | Reversed           |    46,547.5 ns |    520.04 ns |   230.90 ns |  1.27 |    0.01 |    5 |         - |          NA |
| SampleSort                   | 8192 | Reversed           |   280,139.3 ns |  7,418.98 ns | 3,880.27 ns |  7.63 |    0.12 |    8 |         - |          NA |
| SimdQuickSort                | 8192 | Reversed           |    68,822.8 ns |    320.74 ns |   142.41 ns |  1.87 |    0.02 |    6 |         - |          NA |
| DualPivotQuickSort           | 8192 | Reversed           |    38,985.3 ns |  1,820.81 ns |   952.32 ns |  1.06 |    0.03 |    5 |         - |          NA |
| StableQuickSort              | 8192 | Reversed           |   329,763.0 ns |  3,814.26 ns | 1,994.93 ns |  8.98 |    0.09 |    8 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | Reversed           |   151,285.0 ns |  2,676.52 ns | 1,399.87 ns |  4.12 |    0.05 |    7 |         - |          NA |
| DestswapStableQuickSort      | 8192 | Reversed           |   284,619.7 ns |  3,196.81 ns | 1,672.00 ns |  7.75 |    0.08 |    8 |         - |          NA |
| IntroSort                    | 8192 | Reversed           |    33,760.6 ns |    619.25 ns |   274.95 ns |  0.92 |    0.01 |    5 |         - |          NA |
| IntroSortDotnet              | 8192 | Reversed           |    57,800.1 ns |    501.81 ns |   262.46 ns |  1.57 |    0.01 |    6 |         - |          NA |
| PDQSort                      | 8192 | Reversed           |    12,813.9 ns |    366.39 ns |   162.68 ns |  0.35 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | Reversed           |    15,692.7 ns |    458.00 ns |   239.55 ns |  0.43 |    0.01 |    4 |         - |          NA |
| Ipnsort                      | 8192 | Reversed           |     5,541.2 ns |     39.07 ns |    13.93 ns |  0.15 |    0.00 |    1 |         - |          NA |
| StdSort                      | 8192 | Reversed           |    18,245.0 ns |    384.14 ns |   200.91 ns |  0.50 |    0.01 |    4 |         - |          NA |
| BlockQuickSort               | 8192 | Reversed           |    80,588.7 ns |  1,191.16 ns |   528.88 ns |  2.20 |    0.02 |    6 |         - |          NA |
| Ips4oSort                    | 8192 | Reversed           |     5,331.7 ns |     47.98 ns |    17.11 ns |  0.15 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 8192 | Reversed           |     6,760.5 ns |    293.78 ns |   153.65 ns |  0.18 |    0.00 |    2 |         - |          NA |
| CrumSort                     | 8192 | Reversed           |     6,761.6 ns |    277.51 ns |   145.14 ns |  0.18 |    0.00 |    2 |         - |          NA |
| BlitSort                     | 8192 | Reversed           |     6,798.1 ns |    396.22 ns |   207.23 ns |  0.19 |    0.01 |    2 |         - |          NA |
| DotnetSort                   | 8192 | Reversed           |    60,873.1 ns |  3,673.08 ns | 1,921.09 ns |  1.66 |    0.05 |    6 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **PipeOrgan**          | **4,737,484.5 ns** | **12,901.74 ns** | **6,747.86 ns** | **1.000** |    **0.00** |    **5** |         **-** |          **NA** |
| QuickSort3way                | 8192 | PipeOrgan          |   203,912.0 ns | 11,553.76 ns | 6,042.84 ns | 0.043 |    0.00 |    4 |         - |          NA |
| QuickSortMedian3             | 8192 | PipeOrgan          |   149,988.6 ns |  3,084.69 ns | 1,613.35 ns | 0.032 |    0.00 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | PipeOrgan          |    88,659.7 ns |  2,783.43 ns | 1,455.79 ns | 0.019 |    0.00 |    3 |         - |          NA |
| SampleSort                   | 8192 | PipeOrgan          |   304,680.5 ns |  6,017.75 ns | 2,671.92 ns | 0.064 |    0.00 |    4 |         - |          NA |
| SimdQuickSort                | 8192 | PipeOrgan          |    65,534.0 ns |    340.63 ns |   151.24 ns | 0.014 |    0.00 |    3 |         - |          NA |
| DualPivotQuickSort           | 8192 | PipeOrgan          |    77,064.1 ns |  2,282.39 ns | 1,013.40 ns | 0.016 |    0.00 |    3 |         - |          NA |
| StableQuickSort              | 8192 | PipeOrgan          |   322,479.3 ns |  2,591.34 ns | 1,150.57 ns | 0.068 |    0.00 |    4 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | PipeOrgan          |   150,556.6 ns |  1,360.03 ns |   711.32 ns | 0.032 |    0.00 |    4 |         - |          NA |
| DestswapStableQuickSort      | 8192 | PipeOrgan          |   375,975.1 ns |  1,153.47 ns |   512.15 ns | 0.079 |    0.00 |    4 |         - |          NA |
| IntroSort                    | 8192 | PipeOrgan          |   234,198.8 ns |  4,791.88 ns | 2,506.24 ns | 0.049 |    0.00 |    4 |         - |          NA |
| IntroSortDotnet              | 8192 | PipeOrgan          |   266,864.3 ns | 14,321.83 ns | 7,490.60 ns | 0.056 |    0.00 |    4 |         - |          NA |
| PDQSort                      | 8192 | PipeOrgan          |    67,846.8 ns |  2,209.73 ns | 1,155.73 ns | 0.014 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | PipeOrgan          |   102,929.3 ns |    393.71 ns |   174.81 ns | 0.022 |    0.00 |    3 |         - |          NA |
| Ipnsort                      | 8192 | PipeOrgan          |   185,431.5 ns |    274.08 ns |   121.69 ns | 0.039 |    0.00 |    4 |         - |          NA |
| StdSort                      | 8192 | PipeOrgan          |   249,446.6 ns |  1,863.34 ns |   974.56 ns | 0.053 |    0.00 |    4 |         - |          NA |
| BlockQuickSort               | 8192 | PipeOrgan          |   158,186.3 ns |  2,040.26 ns |   905.89 ns | 0.033 |    0.00 |    4 |         - |          NA |
| Ips4oSort                    | 8192 | PipeOrgan          |   142,869.9 ns |  1,340.54 ns |   595.21 ns | 0.030 |    0.00 |    4 |         - |          NA |
| FluxSort                     | 8192 | PipeOrgan          |    17,945.3 ns |    135.11 ns |    70.67 ns | 0.004 |    0.00 |    1 |         - |          NA |
| CrumSort                     | 8192 | PipeOrgan          |    23,105.4 ns |    611.41 ns |   319.78 ns | 0.005 |    0.00 |    2 |         - |          NA |
| BlitSort                     | 8192 | PipeOrgan          |    22,786.4 ns |    196.36 ns |   102.70 ns | 0.005 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | PipeOrgan          |   278,210.4 ns |  5,963.49 ns | 2,647.83 ns | 0.059 |    0.00 |    4 |         - |          NA |
|      |                    |                |              |             |       |         |      |           |             |
| **QuickSort**                    | **8192** | **ManyDuplicates**     |   **205,764.7 ns** |    **931.84 ns** |   **413.74 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| QuickSort3way                | 8192 | ManyDuplicates     |   146,931.0 ns |  1,112.65 ns |   581.94 ns |  0.71 |    0.00 |    3 |         - |          NA |
| QuickSortMedian3             | 8192 | ManyDuplicates     |   222,844.1 ns |    602.66 ns |   267.59 ns |  1.08 |    0.00 |    4 |         - |          NA |
| QuickSortMedian9             | 8192 | ManyDuplicates     |   234,633.4 ns |  1,289.72 ns |   674.55 ns |  1.14 |    0.00 |    4 |         - |          NA |
| SampleSort                   | 8192 | ManyDuplicates     |   311,140.0 ns |  1,639.86 ns |   857.68 ns |  1.51 |    0.00 |    5 |         - |          NA |
| SimdQuickSort                | 8192 | ManyDuplicates     |    50,566.5 ns |    428.21 ns |   190.13 ns |  0.25 |    0.00 |    1 |         - |          NA |
| DualPivotQuickSort           | 8192 | ManyDuplicates     |   144,185.2 ns |  1,412.25 ns |   738.64 ns |  0.70 |    0.00 |    3 |         - |          NA |
| StableQuickSort              | 8192 | ManyDuplicates     |   348,153.2 ns |  8,013.59 ns | 4,191.26 ns |  1.69 |    0.02 |    5 |         - |          NA |
| BidirectionalStableQuickSort | 8192 | ManyDuplicates     |   153,412.2 ns |  1,090.39 ns |   570.30 ns |  0.75 |    0.00 |    3 |         - |          NA |
| DestswapStableQuickSort      | 8192 | ManyDuplicates     |   252,854.0 ns |  1,893.56 ns |   990.37 ns |  1.23 |    0.01 |    4 |         - |          NA |
| IntroSort                    | 8192 | ManyDuplicates     |   193,083.9 ns |    549.61 ns |   244.03 ns |  0.94 |    0.00 |    4 |         - |          NA |
| IntroSortDotnet              | 8192 | ManyDuplicates     |   207,794.2 ns |  1,014.33 ns |   530.52 ns |  1.01 |    0.00 |    4 |         - |          NA |
| PDQSort                      | 8192 | ManyDuplicates     |   144,136.9 ns |  1,037.90 ns |   460.83 ns |  0.70 |    0.00 |    3 |         - |          NA |
| PDQSortBranchless            | 8192 | ManyDuplicates     |    48,782.7 ns |    371.56 ns |   164.97 ns |  0.24 |    0.00 |    1 |         - |          NA |
| Ipnsort                      | 8192 | ManyDuplicates     |    93,920.7 ns |    864.74 ns |   383.95 ns |  0.46 |    0.00 |    2 |         - |          NA |
| StdSort                      | 8192 | ManyDuplicates     |    77,360.0 ns |    261.49 ns |   116.11 ns |  0.38 |    0.00 |    2 |         - |          NA |
| BlockQuickSort               | 8192 | ManyDuplicates     |   108,501.4 ns |    823.45 ns |   365.62 ns |  0.53 |    0.00 |    2 |         - |          NA |
| Ips4oSort                    | 8192 | ManyDuplicates     |    61,244.0 ns |  1,116.45 ns |   583.92 ns |  0.30 |    0.00 |    1 |         - |          NA |
| FluxSort                     | 8192 | ManyDuplicates     |    57,618.5 ns |    287.51 ns |   150.38 ns |  0.28 |    0.00 |    1 |         - |          NA |
| CrumSort                     | 8192 | ManyDuplicates     |    56,295.2 ns |    255.94 ns |   113.64 ns |  0.27 |    0.00 |    1 |         - |          NA |
| BlitSort                     | 8192 | ManyDuplicates     |    80,068.7 ns |    515.43 ns |   228.85 ns |  0.39 |    0.00 |    2 |         - |          NA |
| DotnetSort                   | 8192 | ManyDuplicates     |   190,440.9 ns |  1,162.54 ns |   608.03 ns |  0.93 |    0.00 |    4 |         - |          NA |

### SelectionBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.67GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.5 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                  | Size | Pattern            | Mean         | Error        | StdDev       | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ------------------------ |----- |------------------- |-------------:|-------------:|-------------:|------:|--------:|-----:|----------:|------------:|
| **SelectionSort**           | **256**  | **Random**             |  **25,924.9 ns** |    **416.82 ns** |    **185.07 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | Random             |   9,690.0 ns |    385.96 ns |    137.64 ns |  0.37 |    0.01 |    1 |         - |          NA |
| DoubleSelectionSort     | 256  | Random             |  18,476.5 ns |    170.69 ns |     75.79 ns |  0.71 |    0.01 |    2 |         - |          NA |
| DoubleSelectionSortSimd | 256  | Random             |  11,049.9 ns |    251.88 ns |    131.74 ns |  0.43 |    0.01 |    1 |         - |          NA |
| CycleSort               | 256  | Random             |  48,899.7 ns |    160.32 ns |     83.85 ns |  1.89 |    0.01 |    4 |         - |          NA |
| PancakeSort             | 256  | Random             |  32,708.2 ns |    513.52 ns |    268.58 ns |  1.26 |    0.01 |    3 |         - |          NA |
| BingoSort               | 256  | Random             |  29,194.0 ns |    365.34 ns |    191.08 ns |  1.13 |    0.01 |    3 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**           | **256**  | **SingleElementMoved** |  **19,381.8 ns** |    **164.21 ns** |     **85.88 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | SingleElementMoved |   5,290.6 ns |    422.65 ns |    221.05 ns |  0.27 |    0.01 |    2 |         - |          NA |
| DoubleSelectionSort     | 256  | SingleElementMoved |  10,046.0 ns |  1,146.80 ns |    599.80 ns |  0.52 |    0.03 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 256  | SingleElementMoved |   4,184.9 ns |    180.92 ns |     94.62 ns |  0.22 |    0.00 |    1 |         - |          NA |
| CycleSort               | 256  | SingleElementMoved |  25,479.3 ns |    228.53 ns |    119.53 ns |  1.31 |    0.01 |    6 |         - |          NA |
| PancakeSort             | 256  | SingleElementMoved |  15,977.6 ns |    149.71 ns |     53.39 ns |  0.82 |    0.00 |    4 |         - |          NA |
| BingoSort               | 256  | SingleElementMoved |  19,638.7 ns |    185.27 ns |     66.07 ns |  1.01 |    0.01 |    5 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**           | **256**  | **Sorted**             |  **19,127.7 ns** |    **156.70 ns** |     **69.58 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | Sorted             |   3,715.3 ns |    827.82 ns |    367.56 ns |  0.19 |    0.02 |    2 |         - |          NA |
| DoubleSelectionSort     | 256  | Sorted             |   8,106.0 ns |    276.69 ns |    144.71 ns |  0.42 |    0.01 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 256  | Sorted             |   2,916.8 ns |     37.17 ns |     16.50 ns |  0.15 |    0.00 |    1 |         - |          NA |
| CycleSort               | 256  | Sorted             |  17,439.6 ns |     83.74 ns |     37.18 ns |  0.91 |    0.00 |    5 |         - |          NA |
| PancakeSort             | 256  | Sorted             |  13,087.4 ns |    175.53 ns |     91.81 ns |  0.68 |    0.01 |    4 |         - |          NA |
| BingoSort               | 256  | Sorted             |  19,145.2 ns |    184.70 ns |     96.60 ns |  1.00 |    0.01 |    5 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**           | **256**  | **Reversed**           |  **18,158.5 ns** |  **2,376.21 ns** |  **1,242.80 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | Reversed           |   5,849.4 ns |    205.20 ns |    107.32 ns |  0.32 |    0.02 |    2 |         - |          NA |
| DoubleSelectionSort     | 256  | Reversed           |   8,259.5 ns |    231.35 ns |    102.72 ns |  0.46 |    0.03 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 256  | Reversed           |   4,414.1 ns |    243.34 ns |    127.27 ns |  0.24 |    0.02 |    1 |         - |          NA |
| CycleSort               | 256  | Reversed           |  30,191.4 ns |    173.60 ns |     77.08 ns |  1.67 |    0.11 |    5 |         - |          NA |
| PancakeSort             | 256  | Reversed           |  15,664.7 ns |  2,282.52 ns |  1,193.80 ns |  0.87 |    0.08 |    4 |         - |          NA |
| BingoSort               | 256  | Reversed           |  21,382.1 ns |  2,512.55 ns |  1,314.11 ns |  1.18 |    0.10 |    4 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**           | **256**  | **PipeOrgan**          |  **18,468.1 ns** |    **693.91 ns** |    **362.93 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | PipeOrgan          |   7,110.9 ns |    124.02 ns |     64.87 ns |  0.39 |    0.01 |    2 |         - |          NA |
| DoubleSelectionSort     | 256  | PipeOrgan          |   9,229.2 ns |    404.09 ns |    211.35 ns |  0.50 |    0.01 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 256  | PipeOrgan          |   4,283.1 ns |    279.68 ns |    146.28 ns |  0.23 |    0.01 |    1 |         - |          NA |
| CycleSort               | 256  | PipeOrgan          |  42,810.3 ns |    194.54 ns |    101.75 ns |  2.32 |    0.04 |    5 |         - |          NA |
| PancakeSort             | 256  | PipeOrgan          |  20,605.8 ns |    130.54 ns |     46.55 ns |  1.12 |    0.02 |    4 |         - |          NA |
| BingoSort               | 256  | PipeOrgan          |  20,160.6 ns |    866.45 ns |    453.17 ns |  1.09 |    0.03 |    4 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**           | **256**  | **ManyDuplicates**     |  **24,162.3 ns** |    **284.60 ns** |    **126.36 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 256  | ManyDuplicates     |   9,175.9 ns |    209.86 ns |     93.18 ns |  0.38 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 256  | ManyDuplicates     |  19,393.8 ns |    298.61 ns |    156.18 ns |  0.80 |    0.01 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 256  | ManyDuplicates     |  10,845.4 ns |    333.95 ns |    174.66 ns |  0.45 |    0.01 |    2 |         - |          NA |
| CycleSort               | 256  | ManyDuplicates     |  50,108.8 ns |    539.99 ns |    282.42 ns |  2.07 |    0.01 |    6 |         - |          NA |
| PancakeSort             | 256  | ManyDuplicates     |  30,574.8 ns |    104.12 ns |     54.46 ns |  1.27 |    0.01 |    5 |         - |          NA |
| BingoSort               | 256  | ManyDuplicates     |   5,929.5 ns |     98.14 ns |     43.57 ns |  0.25 |    0.00 |    1 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**           | **1024** | **Random**             | **326,676.6 ns** |  **1,030.65 ns** |    **457.62 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | Random             |  58,610.8 ns |    446.67 ns |    233.62 ns |  0.18 |    0.00 |    1 |         - |          NA |
| DoubleSelectionSort     | 1024 | Random             | 194,336.5 ns |    598.55 ns |    213.45 ns |  0.59 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | Random             |  59,007.3 ns |    321.46 ns |    168.13 ns |  0.18 |    0.00 |    1 |         - |          NA |
| CycleSort               | 1024 | Random             | 773,012.4 ns |  3,275.65 ns |  1,454.41 ns |  2.37 |    0.01 |    4 |         - |          NA |
| PancakeSort             | 1024 | Random             | 379,391.7 ns |    446.50 ns |    159.22 ns |  1.16 |    0.00 |    3 |         - |          NA |
| BingoSort               | 1024 | Random             | 340,730.2 ns |  2,947.77 ns |  1,308.83 ns |  1.04 |    0.00 |    3 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**           | **1024** | **SingleElementMoved** | **292,640.1 ns** |    **725.37 ns** |    **322.07 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | SingleElementMoved |  33,511.7 ns |    492.13 ns |    257.39 ns |  0.11 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 1024 | SingleElementMoved | 142,478.4 ns | 14,805.04 ns |  7,743.32 ns |  0.49 |    0.02 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | SingleElementMoved |  22,346.6 ns |    639.00 ns |    334.21 ns |  0.08 |    0.00 |    1 |         - |          NA |
| CycleSort               | 1024 | SingleElementMoved | 390,366.8 ns |  1,357.26 ns |    602.63 ns |  1.33 |    0.00 |    6 |         - |          NA |
| PancakeSort             | 1024 | SingleElementMoved | 238,134.1 ns | 10,204.72 ns |  4,530.96 ns |  0.81 |    0.01 |    4 |         - |          NA |
| BingoSort               | 1024 | SingleElementMoved | 293,374.4 ns |    771.30 ns |    342.46 ns |  1.00 |    0.00 |    5 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**           | **1024** | **Sorted**             | **291,374.6 ns** |  **1,271.06 ns** |    **564.36 ns** |  **1.00** |    **0.00** |    **5** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | Sorted             |  24,937.5 ns |    246.79 ns |    109.57 ns |  0.09 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 1024 | Sorted             | 124,126.7 ns |    168.10 ns |     59.95 ns |  0.43 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | Sorted             |  16,447.0 ns |    158.56 ns |     82.93 ns |  0.06 |    0.00 |    1 |         - |          NA |
| CycleSort               | 1024 | Sorted             | 263,056.1 ns |    813.56 ns |    361.23 ns |  0.90 |    0.00 |    5 |         - |          NA |
| PancakeSort             | 1024 | Sorted             | 196,872.5 ns |  1,353.42 ns |    707.86 ns |  0.68 |    0.00 |    4 |         - |          NA |
| BingoSort               | 1024 | Sorted             | 292,530.8 ns |  1,093.31 ns |    485.44 ns |  1.00 |    0.00 |    5 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**           | **1024** | **Reversed**           | **262,037.2 ns** |  **9,244.26 ns** |  **4,834.93 ns** |  **1.00** |    **0.00** |    **3** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | Reversed           |  39,295.0 ns |    412.01 ns |    182.94 ns |  0.15 |    0.00 |    1 |         - |          NA |
| DoubleSelectionSort     | 1024 | Reversed           | 124,684.9 ns |    541.55 ns |    240.45 ns |  0.48 |    0.01 |    2 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | Reversed           |  38,305.7 ns |    332.71 ns |    147.72 ns |  0.15 |    0.00 |    1 |         - |          NA |
| CycleSort               | 1024 | Reversed           | 461,720.7 ns |  1,477.52 ns |    772.77 ns |  1.76 |    0.03 |    5 |         - |          NA |
| PancakeSort             | 1024 | Reversed           | 218,687.0 ns | 10,309.31 ns |  5,391.97 ns |  0.83 |    0.02 |    3 |         - |          NA |
| BingoSort               | 1024 | Reversed           | 340,441.8 ns | 23,074.39 ns | 12,068.35 ns |  1.30 |    0.05 |    4 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**           | **1024** | **PipeOrgan**          | **302,258.5 ns** |  **2,748.99 ns** |  **1,437.77 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | PipeOrgan          |  49,130.8 ns |    318.20 ns |    141.28 ns |  0.16 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 1024 | PipeOrgan          | 154,175.9 ns |  1,902.40 ns |    844.68 ns |  0.51 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | PipeOrgan          |  30,342.7 ns |    171.52 ns |     76.16 ns |  0.10 |    0.00 |    1 |         - |          NA |
| CycleSort               | 1024 | PipeOrgan          | 679,046.3 ns |  1,643.30 ns |    859.48 ns |  2.25 |    0.01 |    5 |         - |          NA |
| PancakeSort             | 1024 | PipeOrgan          | 314,042.7 ns |    615.43 ns |    273.25 ns |  1.04 |    0.00 |    4 |         - |          NA |
| BingoSort               | 1024 | PipeOrgan          | 332,729.6 ns |  3,938.95 ns |  2,060.15 ns |  1.10 |    0.01 |    4 |         - |          NA |
|      |                    |              |              |              |       |         |      |           |             |
| **SelectionSort**           | **1024** | **ManyDuplicates**     | **313,626.9 ns** |  **1,466.98 ns** |    **651.35 ns** |  **1.00** |    **0.00** |    **4** |         **-** |          **NA** |
| SelectionSortSimd       | 1024 | ManyDuplicates     |  56,672.1 ns |    350.41 ns |    124.96 ns |  0.18 |    0.00 |    2 |         - |          NA |
| DoubleSelectionSort     | 1024 | ManyDuplicates     | 213,305.1 ns |  2,087.02 ns |  1,091.55 ns |  0.68 |    0.00 |    3 |         - |          NA |
| DoubleSelectionSortSimd | 1024 | ManyDuplicates     |  57,348.8 ns |    420.61 ns |    219.99 ns |  0.18 |    0.00 |    2 |         - |          NA |
| CycleSort               | 1024 | ManyDuplicates     | 765,595.3 ns |  1,734.55 ns |    907.20 ns |  2.44 |    0.01 |    5 |         - |          NA |
| PancakeSort             | 1024 | ManyDuplicates     | 358,477.7 ns |  3,580.30 ns |  1,589.68 ns |  1.14 |    0.01 |    4 |         - |          NA |
| BingoSort               | 1024 | ManyDuplicates     |  20,391.9 ns |    143.63 ns |     63.77 ns |  0.07 |    0.00 |    1 |         - |          NA |

### TreeBenchmark

```
BenchmarkDotNet v0.16.0-preview.2, Linux Ubuntu 24.04.5 LTS (Noble Numbat)
AMD EPYC 9V74 3.67GHz, 1 CPU, 4 logical and 2 physical cores
Memory: 15.61 GB Total, 12.5 GB Available
.NET SDK 11.0.100
  [Host]     : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
  Job-GKDVVL : .NET 11.0.0 (11.0.0, 11.0.26.53018), X64 RyuJIT x86-64-v4
EnvironmentVariables=DOTNET_TieredCompilation=0  InvocationCount=64  IterationCount=8  
UnrollFactor=1  WarmupCount=2  
```

| Method                 | Size | Pattern            | Mean         | Error       | StdDev    | Ratio | RatioSD | Rank | Allocated | Alloc Ratio |
| ----------------------- |----- |------------------- |-------------:|------------:|----------:|------:|--------:|-----:|----------:|------------:|
| **BalancedBinaryTreeSort** | **256**  | **Random**             |  **16,328.0 ns** |   **238.66 ns** | **124.82 ns** |  **1.39** |    **0.02** |    **1** |         **-** |          **NA** |
| BTreeSort              | 256  | Random             |  15,907.6 ns |   181.77 ns |  95.07 ns |  1.35 |    0.01 |    1 |         - |          NA |
| BPlusTreeSort          | 256  | Random             |  15,415.9 ns |   141.41 ns |  73.96 ns |  1.31 |    0.01 |    1 |         - |          NA |
| CartesianTreeSort      | 256  | Random             |  13,051.9 ns |   188.91 ns |  98.80 ns |  1.11 |    0.01 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | Random             |  11,786.9 ns |   204.61 ns | 107.01 ns |  1.00 |    0.00 |    1 |         - |          NA |
| SplaySort              | 256  | Random             |  30,334.5 ns |   446.61 ns | 233.58 ns |  2.57 |    0.03 |    2 |         - |          NA |
| TreapSort              | 256  | Random             |  18,016.2 ns |   150.85 ns |  66.98 ns |  1.53 |    0.01 |    1 |         - |          NA |
|      |                    |              |             |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **SingleElementMoved** |   **6,385.5 ns** |   **128.94 ns** |  **67.44 ns** |  **0.12** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 256  | SingleElementMoved |   4,754.9 ns |   282.51 ns | 147.76 ns |  0.09 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 256  | SingleElementMoved |   4,257.9 ns |   141.90 ns |  63.01 ns |  0.08 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 256  | SingleElementMoved |   1,596.4 ns |    10.02 ns |   4.45 ns |  0.03 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | SingleElementMoved |  54,083.4 ns |   150.92 ns |  78.93 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | SingleElementMoved |   3,346.4 ns |    19.23 ns |   6.86 ns |  0.06 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | SingleElementMoved |   4,683.0 ns |   501.78 ns | 262.44 ns |  0.09 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Sorted**             |   **6,075.2 ns** |   **283.23 ns** | **148.13 ns** |  **0.10** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 256  | Sorted             |   4,453.0 ns |    48.33 ns |  17.24 ns |  0.08 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 256  | Sorted             |   4,063.2 ns |   230.97 ns | 102.55 ns |  0.07 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 256  | Sorted             |   1,523.6 ns |     5.71 ns |   2.99 ns |  0.03 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | Sorted             |  58,511.2 ns |   239.10 ns | 125.05 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | Sorted             |   2,987.8 ns |    59.01 ns |  21.04 ns |  0.05 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | Sorted             |   4,059.3 ns |   162.61 ns |  72.20 ns |  0.07 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **Reversed**           |   **6,013.4 ns** |   **251.38 ns** | **131.48 ns** |  **0.10** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 256  | Reversed           |   7,228.7 ns |    38.38 ns |  13.69 ns |  0.12 |    0.00 |    4 |         - |          NA |
| BPlusTreeSort          | 256  | Reversed           |   6,830.0 ns |    82.64 ns |  36.69 ns |  0.11 |    0.00 |    4 |         - |          NA |
| CartesianTreeSort      | 256  | Reversed           |   1,498.2 ns |     5.02 ns |   2.63 ns |  0.02 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | Reversed           |  62,023.2 ns |   315.68 ns | 140.16 ns |  1.00 |    0.00 |    5 |         - |          NA |
| SplaySort              | 256  | Reversed           |   2,982.8 ns |   248.41 ns | 129.92 ns |  0.05 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | Reversed           |   3,826.0 ns |   110.00 ns |  48.84 ns |  0.06 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **PipeOrgan**          |   **5,359.1 ns** |   **633.69 ns** | **331.43 ns** |  **0.18** |    **0.01** |    **3** |         **-** |          **NA** |
| BTreeSort              | 256  | PipeOrgan          |   5,231.0 ns |   171.06 ns |  89.47 ns |  0.18 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 256  | PipeOrgan          |   4,908.5 ns |   198.94 ns | 104.05 ns |  0.17 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 256  | PipeOrgan          |   1,689.6 ns |     9.75 ns |   4.33 ns |  0.06 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 256  | PipeOrgan          |  29,082.0 ns |   315.27 ns | 164.89 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 256  | PipeOrgan          |   3,372.3 ns |    12.88 ns |   4.59 ns |  0.12 |    0.00 |    2 |         - |          NA |
| TreapSort              | 256  | PipeOrgan          |   5,388.9 ns |   411.76 ns | 215.36 ns |  0.19 |    0.01 |    3 |         - |          NA |
|      |                    |              |             |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **256**  | **ManyDuplicates**     |  **15,832.3 ns** |   **299.29 ns** | **132.89 ns** |  **1.50** |    **0.02** |    **2** |         **-** |          **NA** |
| BTreeSort              | 256  | ManyDuplicates     |  15,276.9 ns |   217.32 ns |  96.49 ns |  1.45 |    0.02 |    2 |         - |          NA |
| BPlusTreeSort          | 256  | ManyDuplicates     |  14,694.0 ns |   337.54 ns | 149.87 ns |  1.40 |    0.02 |    2 |         - |          NA |
| CartesianTreeSort      | 256  | ManyDuplicates     |  15,623.5 ns |   100.97 ns |  52.81 ns |  1.48 |    0.02 |    2 |         - |          NA |
| BinaryTreeSort         | 256  | ManyDuplicates     |  10,522.6 ns |   216.63 ns | 113.30 ns |  1.00 |    0.00 |    1 |         - |          NA |
| SplaySort              | 256  | ManyDuplicates     |  28,445.1 ns |   230.13 ns | 120.36 ns |  2.70 |    0.03 |    3 |         - |          NA |
| TreapSort              | 256  | ManyDuplicates     |  17,783.9 ns |   108.05 ns |  56.51 ns |  1.69 |    0.02 |    2 |         - |          NA |
|      |                    |              |             |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Random**             |  **74,323.3 ns** |   **301.22 ns** | **157.55 ns** |  **1.22** |    **0.00** |    **1** |         **-** |          **NA** |
| BTreeSort              | 1024 | Random             |  75,173.3 ns |   483.36 ns | 252.81 ns |  1.23 |    0.00 |    1 |         - |          NA |
| BPlusTreeSort          | 1024 | Random             |  72,776.3 ns |   351.63 ns | 156.13 ns |  1.19 |    0.00 |    1 |         - |          NA |
| CartesianTreeSort      | 1024 | Random             |  64,775.1 ns |   443.83 ns | 232.13 ns |  1.06 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | Random             |  60,927.1 ns |   233.75 ns | 122.26 ns |  1.00 |    0.00 |    1 |         - |          NA |
| SplaySort              | 1024 | Random             | 155,261.5 ns |   918.32 ns | 407.74 ns |  2.55 |    0.01 |    2 |         - |          NA |
| TreapSort              | 1024 | Random             |  81,260.3 ns |   497.73 ns | 221.00 ns |  1.33 |    0.00 |    1 |         - |          NA |
|      |                    |              |             |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **SingleElementMoved** |  **28,216.3 ns** |   **255.13 ns** | **133.44 ns** | **0.033** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 1024 | SingleElementMoved |  21,319.5 ns |   270.47 ns | 141.46 ns | 0.025 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 1024 | SingleElementMoved |  19,583.0 ns |   201.45 ns | 105.36 ns | 0.023 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 1024 | SingleElementMoved |   6,162.6 ns |   286.67 ns | 149.93 ns | 0.007 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | SingleElementMoved | 851,507.6 ns |   591.92 ns | 309.58 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | SingleElementMoved |  13,130.2 ns |   140.73 ns |  73.61 ns | 0.015 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | SingleElementMoved |  19,904.4 ns |   668.24 ns | 349.50 ns | 0.023 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Sorted**             |  **27,439.4 ns** |   **585.08 ns** | **306.01 ns** | **0.029** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 1024 | Sorted             |  20,284.3 ns |   397.18 ns | 207.73 ns | 0.022 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 1024 | Sorted             |  18,695.9 ns |   136.84 ns |  71.57 ns | 0.020 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 1024 | Sorted             |   5,906.3 ns |     7.01 ns |   2.50 ns | 0.006 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | Sorted             | 933,486.0 ns |   702.42 ns | 311.88 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | Sorted             |  12,058.6 ns |   171.77 ns |  89.84 ns | 0.013 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | Sorted             |  17,513.8 ns |   580.41 ns | 303.57 ns | 0.019 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **Reversed**           |  **27,105.8 ns** |   **287.00 ns** | **150.11 ns** | **0.027** |    **0.00** |    **4** |         **-** |          **NA** |
| BTreeSort              | 1024 | Reversed           |  31,282.4 ns |   134.85 ns |  70.53 ns | 0.032 |    0.00 |    4 |         - |          NA |
| BPlusTreeSort          | 1024 | Reversed           |  28,549.6 ns |   290.98 ns | 152.19 ns | 0.029 |    0.00 |    4 |         - |          NA |
| CartesianTreeSort      | 1024 | Reversed           |   5,817.6 ns |    47.14 ns |  20.93 ns | 0.006 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | Reversed           | 991,783.0 ns |   915.76 ns | 478.96 ns | 1.000 |    0.00 |    5 |         - |          NA |
| SplaySort              | 1024 | Reversed           |  11,471.8 ns |   241.96 ns | 126.55 ns | 0.012 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | Reversed           |  17,036.1 ns |   182.76 ns |  95.59 ns | 0.017 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **PipeOrgan**          |  **23,729.3 ns** |   **662.46 ns** | **294.14 ns** |  **0.05** |    **0.00** |    **3** |         **-** |          **NA** |
| BTreeSort              | 1024 | PipeOrgan          |  23,464.5 ns |   372.09 ns | 194.61 ns |  0.05 |    0.00 |    3 |         - |          NA |
| BPlusTreeSort          | 1024 | PipeOrgan          |  22,225.1 ns |   925.52 ns | 484.07 ns |  0.05 |    0.00 |    3 |         - |          NA |
| CartesianTreeSort      | 1024 | PipeOrgan          |   6,596.8 ns |   228.95 ns | 119.74 ns |  0.01 |    0.00 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | PipeOrgan          | 467,823.7 ns | 2,077.52 ns | 922.43 ns |  1.00 |    0.00 |    4 |         - |          NA |
| SplaySort              | 1024 | PipeOrgan          |  13,586.0 ns |   204.08 ns | 106.74 ns |  0.03 |    0.00 |    2 |         - |          NA |
| TreapSort              | 1024 | PipeOrgan          |  24,417.8 ns |   351.71 ns | 156.16 ns |  0.05 |    0.00 |    3 |         - |          NA |
|      |                    |              |             |           |       |         |      |           |             |
| **BalancedBinaryTreeSort** | **1024** | **ManyDuplicates**     |  **65,007.9 ns** |   **562.38 ns** | **294.14 ns** |  **1.11** |    **0.01** |    **1** |         **-** |          **NA** |
| BTreeSort              | 1024 | ManyDuplicates     |  68,254.1 ns |   288.68 ns | 150.98 ns |  1.17 |    0.01 |    1 |         - |          NA |
| BPlusTreeSort          | 1024 | ManyDuplicates     |  65,280.1 ns |   306.77 ns | 136.21 ns |  1.12 |    0.01 |    1 |         - |          NA |
| CartesianTreeSort      | 1024 | ManyDuplicates     |  76,372.7 ns |   528.11 ns | 234.48 ns |  1.31 |    0.01 |    1 |         - |          NA |
| BinaryTreeSort         | 1024 | ManyDuplicates     |  58,344.8 ns |   723.25 ns | 378.27 ns |  1.00 |    0.00 |    1 |         - |          NA |
| SplaySort              | 1024 | ManyDuplicates     | 123,156.0 ns |   731.55 ns | 324.81 ns |  2.11 |    0.01 |    2 |         - |          NA |
| TreapSort              | 1024 | ManyDuplicates     |  78,102.6 ns |   387.78 ns | 172.18 ns |  1.34 |    0.01 |    1 |         - |          NA |

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
