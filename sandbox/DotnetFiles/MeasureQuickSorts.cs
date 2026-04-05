#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

static void Measure(string label, Action<StatisticsContext> action)
{
    var stats = new StatisticsContext();
    action(stats);
    Console.WriteLine($"{label}: Compares={stats.CompareCount}, Swaps={stats.SwapCount}, Reads={stats.IndexReadCount}, Writes={stats.IndexWriteCount}");
}

foreach (var n in new[] { 10, 20, 50, 100 })
{
    Console.WriteLine($"\n=== n={n} ===");
    var sorted = Enumerable.Range(0, n).ToArray();
    var reversed = Enumerable.Range(0, n).Reverse().ToArray();
    var random = Enumerable.Range(0, n).OrderBy(_ => new Random(42).Next()).ToArray();

    Measure($"[BidirectionalStable] Sorted   n={n}", stats => BidirectionalStableQuickSort.Sort(sorted.ToArray().AsSpan(), stats));
    Measure($"[BidirectionalStable] Reversed n={n}", stats => BidirectionalStableQuickSort.Sort(reversed.ToArray().AsSpan(), stats));
    Measure($"[BidirectionalStable] Random   n={n}", stats => BidirectionalStableQuickSort.Sort(random.ToArray().AsSpan(), stats));

    Measure($"[DestswapStable]      Sorted   n={n}", stats => DestswapStableQuickSort.Sort(sorted.ToArray().AsSpan(), stats));
    Measure($"[DestswapStable]      Reversed n={n}", stats => DestswapStableQuickSort.Sort(reversed.ToArray().AsSpan(), stats));
    Measure($"[DestswapStable]      Random   n={n}", stats => DestswapStableQuickSort.Sort(random.ToArray().AsSpan(), stats));
}
