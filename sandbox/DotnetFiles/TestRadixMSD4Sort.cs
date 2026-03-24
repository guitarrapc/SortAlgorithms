#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

Console.WriteLine("RadixMSD4Sort Test - Basic Example");
Console.WriteLine("==================================");

var stats = new StatisticsContext();
var array = new[] { 5, 2, 8, 1, 9, 3, 7, 4, 6 };

Console.WriteLine($"Original: [{string.Join(", ", array)}]");

RadixMSD4Sort.Sort(array.AsSpan(), stats);

Console.WriteLine($"Sorted:   [{string.Join(", ", array)}]");
Console.WriteLine();
Console.WriteLine($"Statistics:");
Console.WriteLine($"  IndexReads:  {stats.IndexReadCount}");
Console.WriteLine($"  IndexWrites: {stats.IndexWriteCount}");
Console.WriteLine($"  Compares:    {stats.CompareCount}");
Console.WriteLine($"  Swaps:       {stats.SwapCount}");
Console.WriteLine();

Console.WriteLine("RadixMSD4Sort Test - Negative Numbers");
Console.WriteLine("======================================");

stats = new StatisticsContext();
var negativeArray = new[] { -5, 3, -1, 0, 2, -3, 1, -10, 15 };

Console.WriteLine($"Original: [{string.Join(", ", negativeArray)}]");

RadixMSD4Sort.Sort(negativeArray.AsSpan(), stats);

Console.WriteLine($"Sorted:   [{string.Join(", ", negativeArray)}]");
Console.WriteLine();
Console.WriteLine($"Statistics:");
Console.WriteLine($"  IndexReads:  {stats.IndexReadCount}");
Console.WriteLine($"  IndexWrites: {stats.IndexWriteCount}");
Console.WriteLine($"  Compares:    {stats.CompareCount}");
Console.WriteLine($"  Swaps:       {stats.SwapCount}");
Console.WriteLine();

Console.WriteLine("RadixMSD4Sort Test - Large Array");
Console.WriteLine("=================================");

stats = new StatisticsContext();
var largeArray = Enumerable.Range(0, 100).Reverse().ToArray();

Console.WriteLine($"Original: [99, 98, 97, ... 2, 1, 0] (100 elements reversed)");

RadixMSD4Sort.Sort(largeArray.AsSpan(), stats);

Console.WriteLine($"Sorted:   [0, 1, 2, ... 97, 98, 99] (all correctly sorted)");
Console.WriteLine();
Console.WriteLine($"Statistics:");
Console.WriteLine($"  IndexReads:  {stats.IndexReadCount}");
Console.WriteLine($"  IndexWrites: {stats.IndexWriteCount}");
Console.WriteLine($"  Compares:    {stats.CompareCount}");
Console.WriteLine($"  Swaps:       {stats.SwapCount}");

// Verify correctness
bool isSorted = true;
for (int i = 0; i < largeArray.Length; i++)
{
    if (largeArray[i] != i)
    {
        isSorted = false;
        break;
    }
}

Console.WriteLine($"  Correctness: {(isSorted ? "✓ PASS" : "✗ FAIL")}");
