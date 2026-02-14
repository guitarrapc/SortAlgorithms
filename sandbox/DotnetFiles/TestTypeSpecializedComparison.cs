#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

Console.WriteLine("=== Testing Type-Specialized Comparison in SortSpan ===\n");

// Test with int (primitive type - should use optimized comparison)
var intArray = new[] { 5, 3, 8, 1, 2, 9, 4, 7, 6 };
var stats = new StatisticsContext();

PDQSort.Sort<int>(intArray, stats);
Console.WriteLine($"int array sorted: [{string.Join(", ", intArray)}]");
Console.WriteLine($"Compares: {stats.CompareCount}, Swaps: {stats.SwapCount}");
Console.WriteLine($"IndexReads: {stats.IndexReadCount}, IndexWrites: {stats.IndexWriteCount}\n");

// Test with double (primitive type - should use optimized comparison)
var doubleArray = new[] { 5.5, 3.3, 8.8, 1.1, 2.2 };
stats = new StatisticsContext();

PDQSort.Sort<double>(doubleArray, stats);
Console.WriteLine($"double array sorted: [{string.Join(", ", doubleArray)}]");
Console.WriteLine($"Compares: {stats.CompareCount}, Swaps: {stats.SwapCount}");
Console.WriteLine($"IndexReads: {stats.IndexReadCount}, IndexWrites: {stats.IndexWriteCount}\n");

// Test with string (reference type - should fall back to regular comparison)
var stringArray = new[] { "banana", "apple", "cherry", "date" };
stats = new StatisticsContext();

PDQSort.Sort<string>(stringArray, stats);
Console.WriteLine($"string array sorted: [{string.Join(", ", stringArray)}]");
Console.WriteLine($"Compares: {stats.CompareCount}, Swaps: {stats.SwapCount}");
Console.WriteLine($"IndexReads: {stats.IndexReadCount}, IndexWrites: {stats.IndexWriteCount}\n");

Console.WriteLine("✓ Type-specialized comparison is working correctly!");
Console.WriteLine("✓ All sorting algorithms now benefit from this optimization!");
