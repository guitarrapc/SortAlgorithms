using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

var testCases = new[] { 10, 20, 50, 100, 1000, 10000 };

Console.WriteLine("=== Sorted Data ===");
foreach (var n in testCases)
{
    var stats = new StatisticsContext();
    var sorted = Enumerable.Range(0, n).ToArray();
    TimSort.Sort(sorted.AsSpan(), stats);
    Console.WriteLine($"n={n,5}: Compares={stats.CompareCount,6}, Writes={stats.IndexWriteCount,6}, Swaps={stats.SwapCount,4}");
}

Console.WriteLine("\n=== Reversed Data ===");
foreach (var n in testCases)
{
    var stats = new StatisticsContext();
    var reversed = Enumerable.Range(0, n).Reverse().ToArray();
    TimSort.Sort(reversed.AsSpan(), stats);
    Console.WriteLine($"n={n,5}: Compares={stats.CompareCount,6}, Writes={stats.IndexWriteCount,6}, Swaps={stats.SwapCount,4}");
}

Console.WriteLine("\n=== Random Data (10 trials) ===");
foreach (var n in testCases)
{
    var compareSum = 0UL;
    var writeSum = 0UL;
    var swapSum = 0UL;
    var trials = 10;
    
    for (int trial = 0; trial < trials; trial++)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        TimSort.Sort(random.AsSpan(), stats);
        compareSum += stats.CompareCount;
        writeSum += stats.IndexWriteCount;
        swapSum += stats.SwapCount;
    }
    
    Console.WriteLine($"n={n,5}: Avg Compares={compareSum/(ulong)trials,6}, Avg Writes={writeSum/(ulong)trials,6}, Avg Swaps={swapSum/(ulong)trials,4}");
}

Console.WriteLine("\n=== Partially Sorted Data (sorted + 10% random at end) ===");
foreach (var n in testCases)
{
    var stats = new StatisticsContext();
    var data = Enumerable.Range(0, n).ToArray();
    // Randomize last 10%
    var randomCount = n / 10;
    var rnd = new Random(42);
    for (int i = n - randomCount; i < n; i++)
    {
        data[i] = rnd.Next(n);
    }
    TimSort.Sort(data.AsSpan(), stats);
    Console.WriteLine($"n={n,5}: Compares={stats.CompareCount,6}, Writes={stats.IndexWriteCount,6}, Swaps={stats.SwapCount,4}");
}

Console.WriteLine("\n=== Many Duplicates (only 4 distinct values) ===");
foreach (var n in testCases)
{
    var stats = new StatisticsContext();
    var data = new int[n];
    var rnd = new Random(42);
    for (int i = 0; i < n; i++)
    {
        data[i] = rnd.Next(4); // Only 0, 1, 2, 3
    }
    TimSort.Sort(data.AsSpan(), stats);
    Console.WriteLine($"n={n,5}: Compares={stats.CompareCount,6}, Writes={stats.IndexWriteCount,6}, Swaps={stats.SwapCount,4}");
}

