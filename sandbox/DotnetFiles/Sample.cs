#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

var stats = new StatisticsContext();
PowerSort.Sort([ 5, 3, 8, 1, 2 ], stats);

Console.WriteLine("Sorted array with PowerSort.");
Console.WriteLine($"Compares: {stats.CompareCount}, Swaps: {stats.SwapCount}, IndexReads: {stats.IndexReadCount}, IndexWrites: {stats.IndexWriteCount}");
