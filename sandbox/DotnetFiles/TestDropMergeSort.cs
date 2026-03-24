#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

Console.WriteLine("Testing DropMergeSort theoretical values...\n");

// Test reversed data for different sizes
int[] sizes = [10, 20, 50, 100];
foreach (var n in sizes)
{
    var stats = new StatisticsContext();
    var reversed = Enumerable.Range(0, n).Reverse().ToArray();
    DropMergeSort.Sort(reversed.AsSpan(), stats);
    
    var logN = Math.Log2(n);
    var ratio = stats.CompareCount / (n * logN);
    
    Console.WriteLine($"Reversed n={n}:");
    Console.WriteLine($"  Compares: {stats.CompareCount}");
    Console.WriteLine($"  n*log2(n): {n * logN:F2}");
    Console.WriteLine($"  Ratio: {ratio:F3}");
    Console.WriteLine($"  Writes: {stats.IndexWriteCount}");
    Console.WriteLine($"  Swaps: {stats.SwapCount}");
    Console.WriteLine();
}

Console.WriteLine("Testing random data...\n");
foreach (var n in sizes)
{
    var stats = new StatisticsContext();
    var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
    DropMergeSort.Sort(random.AsSpan(), stats);
    
    var logN = Math.Log2(n);
    var ratio = stats.CompareCount / (n * logN);
    
    Console.WriteLine($"Random n={n}:");
    Console.WriteLine($"  Compares: {stats.CompareCount}");
    Console.WriteLine($"  n*log2(n): {n * logN:F2}");
    Console.WriteLine($"  Ratio: {ratio:F3}");
    Console.WriteLine($"  Writes: {stats.IndexWriteCount}");
    Console.WriteLine($"  Swaps: {stats.SwapCount}");
    Console.WriteLine();
}
