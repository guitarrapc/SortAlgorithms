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
        var sorted = Enumerable.Range(0, n).ToArray();
        Glidesort.Sort(sorted.AsSpan(), stats);
        Console.WriteLine($"Sorted n={n}: Compares={stats.CompareCount}, Writes={stats.IndexWriteCount}, Swaps={stats.SwapCount}, Reads={stats.IndexReadCount}");
    }

    // Reversed
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        Glidesort.Sort(reversed.AsSpan(), stats);
        Console.WriteLine($"Reversed n={n}: Compares={stats.CompareCount}, Writes={stats.IndexWriteCount}, Swaps={stats.SwapCount}, Reads={stats.IndexReadCount}");
    }

    // All-equal (duplicate stress test)
    {
        var stats = new StatisticsContext();
        var allEqual = Enumerable.Repeat(42, n).ToArray();
        Glidesort.Sort(allEqual.AsSpan(), stats);
        Console.WriteLine($"AllEqual n={n}: Compares={stats.CompareCount}, Writes={stats.IndexWriteCount}, Swaps={stats.SwapCount}, Reads={stats.IndexReadCount}");
    }

    // Two distinct values
    {
        var stats = new StatisticsContext();
        var twoVals = new int[n];
        for (int i = 0; i < n; i++) twoVals[i] = i % 2;
        Glidesort.Sort(twoVals.AsSpan(), stats);
        Console.WriteLine($"TwoVals n={n}: Compares={stats.CompareCount}, Writes={stats.IndexWriteCount}, Swaps={stats.SwapCount}, Reads={stats.IndexReadCount}");
    }

    // Random (1 trial)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        Glidesort.Sort(random.AsSpan(), stats);
        Console.WriteLine($"Random n={n}: Compares={stats.CompareCount}, Writes={stats.IndexWriteCount}, Swaps={stats.SwapCount}, Reads={stats.IndexReadCount}");
    }
    Console.WriteLine();
}
