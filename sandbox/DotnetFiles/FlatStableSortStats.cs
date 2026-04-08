#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

foreach (var n in new[] { 10, 20, 50, 100 })
{
    Console.WriteLine($"=== n={n} ===");

    // Sorted
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        FlatStableSort.Sort(sorted.AsSpan(), stats);
        Console.WriteLine($"  Sorted   : Compares={stats.CompareCount}, Writes={stats.IndexWriteCount}, Reads={stats.IndexReadCount}, Swaps={stats.SwapCount}");
    }

    // Reversed
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        FlatStableSort.Sort(reversed.AsSpan(), stats);
        Console.WriteLine($"  Reversed : Compares={stats.CompareCount}, Writes={stats.IndexWriteCount}, Reads={stats.IndexReadCount}, Swaps={stats.SwapCount}");
    }

    // Random (multiple runs)
    for (var seed = 0; seed < 3; seed++)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => seed * 1000 + Guid.NewGuid().GetHashCode()).ToArray();
        FlatStableSort.Sort(random.AsSpan(), stats);
        Console.WriteLine($"  Random{seed}  : Compares={stats.CompareCount}, Writes={stats.IndexWriteCount}, Reads={stats.IndexReadCount}, Swaps={stats.SwapCount}");
    }
}
