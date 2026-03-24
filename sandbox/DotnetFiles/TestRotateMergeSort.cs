#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

var stats = new StatisticsContext();
var testData = new[] { 5, 3, 8, 1, 2, 9, 4, 7, 6 };

Console.WriteLine("Original array: " + string.Join(", ", testData));

RotateMergeSort.Sort(testData.AsSpan(), stats);

Console.WriteLine("Sorted array:   " + string.Join(", ", testData));
Console.WriteLine($"\nStatistics:");
Console.WriteLine($"  Compares:      {stats.CompareCount}");
Console.WriteLine($"  Swaps:         {stats.SwapCount}");
Console.WriteLine($"  IndexReads:    {stats.IndexReadCount}");
Console.WriteLine($"  IndexWrites:   {stats.IndexWriteCount}");

// Test with another dataset
var stats2 = new StatisticsContext();
var reversedData = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
Console.WriteLine("\n\nReversed array test:");
Console.WriteLine("Original: " + string.Join(", ", reversedData));

RotateMergeSort.Sort(reversedData.AsSpan(), stats2);

Console.WriteLine("Sorted:   " + string.Join(", ", reversedData));
Console.WriteLine($"\nStatistics:");
Console.WriteLine($"  Compares:      {stats2.CompareCount}");
Console.WriteLine($"  Swaps:         {stats2.SwapCount}");
Console.WriteLine($"  IndexReads:    {stats2.IndexReadCount}");
Console.WriteLine($"  IndexWrites:   {stats2.IndexWriteCount}");

Console.WriteLine("\nRotateMergeSort implementation completed successfully!");
