#:project ../../src/SortAlgorithm

// Verifies that GenerateQuickSortAdversary forces quadratic behavior, and pins down exactly
// which quicksort variants it defeats.
//
// Reported as comparisons / (n log2 n). A value near 1-3 means "no worse than random"; a value
// that grows with n means quadratic, with n / (2 log2 n) as the ceiling.

using System.Diagnostics;
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using SortAlgorithm.Utils;

var variants = new (string Name, Action<Span<int>, StatisticsContext> Sort)[]
{
    ("QuickSort (mid)     ", (s, c) => QuickSort.Sort(s, c)),
    ("QuickSortMedian3    ", (s, c) => QuickSortMedian3.Sort(s, c)),
    ("QuickSort3way       ", (s, c) => QuickSort3way.Sort(s, c)),
    ("QuickSortMedian9    ", (s, c) => QuickSortMedian9.Sort(s, c)),
    ("StableQuickSort     ", (s, c) => StableQuickSort.Sort(s, c)),
    ("BidirectionalStable ", (s, c) => BidirectionalStableQuickSort.Sort(s, c)),
    ("DestswapStable      ", (s, c) => DestswapStableQuickSort.Sort(s, c)),
    ("DualPivotQuickSort  ", (s, c) => DualPivotQuickSort.Sort(s, c)),
};

foreach (var n in new[] { 1000, 4000, 16000 })
{
    Console.WriteLine($"=== n = {n}  (quadratic ceiling is c/(n log2 n) = {n / (2 * Math.Log2(n)):F0}) ===");
    Console.WriteLine($"{"variant",-21} {"random",8} {"sorted",8} {"reverse",8} {"pipeOrgan",10} {"quickSortAdv",13}");

    foreach (var (name, sort) in variants)
    {
        Console.WriteLine($"{name,-21} " +
            $"{Ratio(Shuffled(n), sort),8:F2} " +
            $"{Ratio([.. Enumerable.Range(0, n)], sort),8:F2} " +
            $"{Ratio([.. Enumerable.Range(0, n).Reverse()], sort),8:F2} " +
            $"{Ratio(ArrayPatterns.GeneratePipeOrgan(n), sort),10:F2} " +
            $"{Ratio(ArrayPatterns.GenerateQuickSortAdversary(n), sort),13:F2}");
    }
    Console.WriteLine();
}

// Generation is one full run of the target under the adversary, so it costs exactly the
// quadratic work it provokes. This is the number a consumer has to plan a cache around.
Console.WriteLine("--- generation cost ---");
_ = ArrayPatterns.GenerateQuickSortAdversary(64);
foreach (var n in new[] { 1024, 2048, 4096, 8192, 16384, 32768 })
{
    var sw = Stopwatch.StartNew();
    _ = ArrayPatterns.GenerateQuickSortAdversary(n);
    sw.Stop();
    Console.WriteLine($"n={n,6}  {sw.Elapsed.TotalMilliseconds,9:F1} ms");
}

Console.WriteLine();
Console.WriteLine("--- operation totals (compare + swap + read + write) at n = 32768 ---");
Console.WriteLine($"{"algorithm",-22} {"random",14} {"quickSortAdv",14}");
Ops("quicksort", (s, c) => QuickSort.Sort(s, c));
Ops("quicksortmedian3", (s, c) => QuickSortMedian3.Sort(s, c));
Ops("quicksort3way", (s, c) => QuickSort3way.Sort(s, c));
Ops("quicksortmedian9", (s, c) => QuickSortMedian9.Sort(s, c));
Ops("quicksortstable", (s, c) => StableQuickSort.Sort(s, c));
Ops("quicksortdestswapstable", (s, c) => DestswapStableQuickSort.Sort(s, c));
Ops("pdqsort", (s, c) => PDQSort.Sort(s, c));
Ops("introsort", (s, c) => IntroSort.Sort(s, c));
Ops("timsort", (s, c) => TimSort.Sort(s, c));
Ops("mergesort", (s, c) => MergeSort.Sort(s, c));
Ops("heapsort", (s, c) => HeapSort.Sort(s, c));

static void Ops(string label, Action<Span<int>, StatisticsContext> sort)
{
    const int N = 32768;
    Console.WriteLine($"{label,-22} {Total(Shuffled(N), sort),14:N0} {Total(ArrayPatterns.GenerateQuickSortAdversary(N), sort),14:N0}");
}

static ulong Total(int[] data, Action<Span<int>, StatisticsContext> sort)
{
    var work = (int[])data.Clone();
    var stats = new StatisticsContext();
    sort(work.AsSpan(), stats);
    return stats.CompareCount + stats.SwapCount + stats.IndexReadCount + stats.IndexWriteCount;
}

static double Ratio(int[] data, Action<Span<int>, StatisticsContext> sort)
{
    var work = (int[])data.Clone();
    var stats = new StatisticsContext();
    sort(work.AsSpan(), stats);

    for (var i = 1; i < work.Length; i++)
    {
        if (work[i - 1] > work[i]) return double.NaN;
    }

    return stats.CompareCount / (data.Length * Math.Log2(data.Length));
}

static int[] Shuffled(int size)
{
    var random = new Random(20260802);
    var array = Enumerable.Range(0, size).ToArray();
    for (var i = array.Length - 1; i > 0; i--)
    {
        var j = random.Next(i + 1);
        (array[i], array[j]) = (array[j], array[i]);
    }
    return array;
}
