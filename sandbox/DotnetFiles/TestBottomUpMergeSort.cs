#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

var stats = new StatisticsContext();
var array = new[] { 5, 2, 8, 1, 9, 3, 7, 4, 6 };
Console.WriteLine($"Original: [{string.Join(", ", array)}]");

BottomUpMergeSort.Sort(array.AsSpan(), stats);

Console.WriteLine($"Sorted:   [{string.Join(", ", array)}]");
Console.WriteLine($"Statistics:");
Console.WriteLine($"  Compares:     {stats.CompareCount}");
Console.WriteLine($"  Swaps:        {stats.SwapCount}");
Console.WriteLine($"  IndexReads:   {stats.IndexReadCount}");
Console.WriteLine($"  IndexWrites:  {stats.IndexWriteCount}");
