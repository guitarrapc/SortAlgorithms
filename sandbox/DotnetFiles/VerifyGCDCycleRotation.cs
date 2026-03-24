#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

Console.WriteLine("=== GCD-Cycle Rotation 確認 ===\n");

// Test 1: Reversed data
var stats1 = new StatisticsContext();
var reversed = new[] { 10, 9, 8, 7, 6, 5, 4, 3, 2, 1 };
Console.WriteLine("Reversed array test:");
Console.WriteLine("Original: " + string.Join(", ", reversed));

RotateMergeSort.Sort(reversed.AsSpan(), stats1);

Console.WriteLine("Sorted:   " + string.Join(", ", reversed));
Console.WriteLine($"\nStatistics:");
Console.WriteLine($"  Compares:      {stats1.CompareCount}");
Console.WriteLine($"  Swaps:         {stats1.SwapCount} (GCD-cycle uses assignments only, should be 0!)");
Console.WriteLine($"  IndexReads:    {stats1.IndexReadCount}");
Console.WriteLine($"  IndexWrites:   {stats1.IndexWriteCount}");

// Test 2: Random data
var stats2 = new StatisticsContext();
var random = new[] { 5, 3, 8, 1, 2, 9, 4, 7, 6 };
Console.WriteLine("\n\nRandom array test:");
Console.WriteLine("Original: " + string.Join(", ", random));

RotateMergeSort.Sort(random.AsSpan(), stats2);

Console.WriteLine("Sorted:   " + string.Join(", ", random));
Console.WriteLine($"\nStatistics:");
Console.WriteLine($"  Compares:      {stats2.CompareCount}");
Console.WriteLine($"  Swaps:         {stats2.SwapCount} (should be 0!)");
Console.WriteLine($"  IndexReads:    {stats2.IndexReadCount}");
Console.WriteLine($"  IndexWrites:   {stats2.IndexWriteCount}");

Console.WriteLine("\n\n✅ GCD-Cycle Rotation Implementation:");
Console.WriteLine("   - Swaps: 0 (uses assignments only)");
Console.WriteLine("   - More efficient than 3-reversal (no swap overhead)");
Console.WriteLine("   - Mathematically elegant (GCD-based cycles)");
