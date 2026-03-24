#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

Console.WriteLine("=== RadixMSD10Sort Test ===\n");

// Test 1: Basic positive numbers
var test1 = new[] { 170, 45, 75, 90, 2, 802, 24, 66 };
Console.WriteLine("Test 1: Basic positive numbers");
Console.WriteLine($"Original: [{string.Join(", ", test1)}]");
var stats1 = new StatisticsContext();
RadixMSD10Sort.Sort(test1.AsSpan(), stats1);
Console.WriteLine($"Sorted:   [{string.Join(", ", test1)}]");
Console.WriteLine($"Stats - Compares: {stats1.CompareCount}, Swaps: {stats1.SwapCount}, Reads: {stats1.IndexReadCount}, Writes: {stats1.IndexWriteCount}\n");

// Test 2: With negative numbers
var test2 = new[] { -5, 3, -1, 0, 2, -3, 1 };
Console.WriteLine("Test 2: With negative numbers");
Console.WriteLine($"Original: [{string.Join(", ", test2)}]");
var stats2 = new StatisticsContext();
RadixMSD10Sort.Sort(test2.AsSpan(), stats2);
Console.WriteLine($"Sorted:   [{string.Join(", ", test2)}]");
Console.WriteLine($"Stats - Compares: {stats2.CompareCount}, Swaps: {stats2.SwapCount}, Reads: {stats2.IndexReadCount}, Writes: {stats2.IndexWriteCount}\n");

// Test 3: Edge cases with int.MinValue and int.MaxValue
var test3 = new[] { int.MinValue, -1, 0, 1, int.MaxValue };
Console.WriteLine("Test 3: Edge cases (int.MinValue to int.MaxValue)");
Console.WriteLine($"Original: [{string.Join(", ", test3)}]");
var stats3 = new StatisticsContext();
RadixMSD10Sort.Sort(test3.AsSpan(), stats3);
Console.WriteLine($"Sorted:   [{string.Join(", ", test3)}]");
Console.WriteLine($"Stats - Compares: {stats3.CompareCount}, Swaps: {stats3.SwapCount}, Reads: {stats3.IndexReadCount}, Writes: {stats3.IndexWriteCount}\n");

// Test 4: Decimal digit boundaries
var test4 = new[] { 100, 9, 99, 10, 1, 999, 1000 };
Console.WriteLine("Test 4: Decimal digit boundaries");
Console.WriteLine($"Original: [{string.Join(", ", test4)}]");
var stats4 = new StatisticsContext();
RadixMSD10Sort.Sort(test4.AsSpan(), stats4);
Console.WriteLine($"Sorted:   [{string.Join(", ", test4)}]");
Console.WriteLine($"Stats - Compares: {stats4.CompareCount}, Swaps: {stats4.SwapCount}, Reads: {stats4.IndexReadCount}, Writes: {stats4.IndexWriteCount}\n");

// Test 5: Compare MSD10 vs MSD4
var test5a = new[] { 123, 456, 789, 12, 45, 78, 1, 4, 7 };
var test5b = test5a.ToArray();
Console.WriteLine("Test 5: Compare RadixMSD10Sort vs RadixMSD4Sort");
Console.WriteLine($"Original: [{string.Join(", ", test5a)}]");

var statsMSD10 = new StatisticsContext();
RadixMSD10Sort.Sort(test5a.AsSpan(), statsMSD10);
Console.WriteLine($"MSD10 Sorted: [{string.Join(", ", test5a)}]");
Console.WriteLine($"MSD10 Stats - Compares: {statsMSD10.CompareCount}, Swaps: {statsMSD10.SwapCount}, Reads: {statsMSD10.IndexReadCount}, Writes: {statsMSD10.IndexWriteCount}");

var statsMSD4 = new StatisticsContext();
RadixMSD4Sort.Sort(test5b.AsSpan(), statsMSD4);
Console.WriteLine($"MSD4 Sorted:  [{string.Join(", ", test5b)}]");
Console.WriteLine($"MSD4 Stats - Compares: {statsMSD4.CompareCount}, Swaps: {statsMSD4.SwapCount}, Reads: {statsMSD4.IndexReadCount}, Writes: {statsMSD4.IndexWriteCount}");

Console.WriteLine($"\nResults match: {test5a.SequenceEqual(test5b)}");
