#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

var stats = new StatisticsContext();
var array = new[] { 25, 13, 28, 11, 22, 29, 14, 27, 16, 30, 15, 26, 17, 31, 18, 24, 19, 23, 20, 21 };

Console.WriteLine("Before sort:");
Console.WriteLine(string.Join(", ", array));

AmericanFlagSort.Sort(array.AsSpan(), stats);

Console.WriteLine("\nAfter sort:");
Console.WriteLine(string.Join(", ", array));

Console.WriteLine($"\nStatistics:");
Console.WriteLine($"  CompareCount: {stats.CompareCount}");
Console.WriteLine($"  SwapCount: {stats.SwapCount}");
Console.WriteLine($"  IndexReadCount: {stats.IndexReadCount}");
Console.WriteLine($"  IndexWriteCount: {stats.IndexWriteCount}");

// Check if sorted
bool isSorted = true;
for (int i = 1; i < array.Length; i++)
{
    if (array[i - 1] > array[i])
    {
        isSorted = false;
        break;
    }
}

Console.WriteLine($"\nIs sorted: {isSorted}");
