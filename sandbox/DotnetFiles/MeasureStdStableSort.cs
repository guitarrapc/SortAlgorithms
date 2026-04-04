#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

foreach (var n in new[] { 10, 20, 50, 100 })
{
    // Sorted
    {
        var stats = new StatisticsContext();
        var arr = Enumerable.Range(0, n).ToArray();
        StdStableSort.Sort(arr.AsSpan(), stats);
        Console.WriteLine($"Sorted   n={n,3}: Compares={stats.CompareCount,6}, Writes={stats.IndexWriteCount,6}, Reads={stats.IndexReadCount,6}, Swaps={stats.SwapCount}");
    }
    // Reversed
    {
        var stats = new StatisticsContext();
        var arr = Enumerable.Range(0, n).Reverse().ToArray();
        StdStableSort.Sort(arr.AsSpan(), stats);
        Console.WriteLine($"Reversed n={n,3}: Compares={stats.CompareCount,6}, Writes={stats.IndexWriteCount,6}, Reads={stats.IndexReadCount,6}, Swaps={stats.SwapCount}");
    }
    // Random (fixed seed via deterministic shuffle)
    {
        var stats = new StatisticsContext();
        var arr = Enumerable.Range(0, n).OrderBy(x => (x * 2654435761u) % (uint)n).ToArray();
        StdStableSort.Sort(arr.AsSpan(), stats);
        Console.WriteLine($"Random   n={n,3}: Compares={stats.CompareCount,6}, Writes={stats.IndexWriteCount,6}, Reads={stats.IndexReadCount,6}, Swaps={stats.SwapCount}");
    }
    Console.WriteLine();
}
