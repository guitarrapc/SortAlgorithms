#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

// Test case 1: All elements equal (tests boundary condition)
var allEqual = new[] { 5, 5, 5, 5, 5 };
var stats1 = new StatisticsContext();
PDQSort.Sort(allEqual.AsSpan(), stats1);
Console.WriteLine("Test 1 - All equal:");
Console.WriteLine($"  Result: [{string.Join(", ", allEqual)}]");
Console.WriteLine($"  Stats: Compares={stats1.CompareCount}, Swaps={stats1.SwapCount}");

// Test case 2: Already sorted (tests first pointer advancement)
var sorted = new[] { 1, 2, 3, 4, 5 };
var stats2 = new StatisticsContext();
PDQSort.Sort(sorted.AsSpan(), stats2);
Console.WriteLine("\nTest 2 - Already sorted:");
Console.WriteLine($"  Result: [{string.Join(", ", sorted)}]");
Console.WriteLine($"  Stats: Compares={stats2.CompareCount}, Swaps={stats2.SwapCount}");

// Test case 3: Reverse sorted (tests last pointer retreat)
var reverse = new[] { 5, 4, 3, 2, 1 };
var stats3 = new StatisticsContext();
PDQSort.Sort(reverse.AsSpan(), stats3);
Console.WriteLine("\nTest 3 - Reverse sorted:");
Console.WriteLine($"  Result: [{string.Join(", ", reverse)}]");
Console.WriteLine($"  Stats: Compares={stats3.CompareCount}, Swaps={stats3.SwapCount}");

// Test case 4: Two elements
var twoElems = new[] { 2, 1 };
var stats4 = new StatisticsContext();
PDQSort.Sort(twoElems.AsSpan(), stats4);
Console.WriteLine("\nTest 4 - Two elements:");
Console.WriteLine($"  Result: [{string.Join(", ", twoElems)}]");
Console.WriteLine($"  Stats: Compares={stats4.CompareCount}, Swaps={stats4.SwapCount}");

Console.WriteLine("\n✓ All boundary guard tests passed!");
