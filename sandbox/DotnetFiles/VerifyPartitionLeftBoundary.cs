#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

// Test case: All elements greater than pivot (worst case for PartitionLeft)
// This tests if last goes below begin
var allGreaterThanPivot = new[] { 1, 5, 6, 7, 8, 9 }; // pivot = 1, all others > pivot
var stats1 = new StatisticsContext();
PDQSort.Sort(allGreaterThanPivot.AsSpan(), stats1);
Console.WriteLine("Test 1 - All elements > pivot:");
Console.WriteLine($"  Input:  [1, 5, 6, 7, 8, 9]");
Console.WriteLine($"  Result: [{string.Join(", ", allGreaterThanPivot)}]");
Console.WriteLine($"  Expected: [1, 5, 6, 7, 8, 9]");
Console.WriteLine($"  Status: {(string.Join(",", allGreaterThanPivot) == "1,5,6,7,8,9" ? "✓ PASS" : "✗ FAIL")}");

// Test case: All elements equal to pivot
var allEqualToPivot = new[] { 5, 5, 5, 5, 5 };
var stats2 = new StatisticsContext();
PDQSort.Sort(allEqualToPivot.AsSpan(), stats2);
Console.WriteLine("\nTest 2 - All elements equal to pivot:");
Console.WriteLine($"  Input:  [5, 5, 5, 5, 5]");
Console.WriteLine($"  Result: [{string.Join(", ", allEqualToPivot)}]");
Console.WriteLine($"  Expected: [5, 5, 5, 5, 5]");
Console.WriteLine($"  Status: {(string.Join(",", allEqualToPivot) == "5,5,5,5,5" ? "✓ PASS" : "✗ FAIL")}");

// Test case: Minimum element at start, all others greater
var minAtStart = new[] { 0, 10, 20, 30, 40 };
var stats3 = new StatisticsContext();
PDQSort.Sort(minAtStart.AsSpan(), stats3);
Console.WriteLine("\nTest 3 - Minimum at start:");
Console.WriteLine($"  Input:  [0, 10, 20, 30, 40]");
Console.WriteLine($"  Result: [{string.Join(", ", minAtStart)}]");
Console.WriteLine($"  Expected: [0, 10, 20, 30, 40]");
Console.WriteLine($"  Status: {(string.Join(",", minAtStart) == "0,10,20,30,40" ? "✓ PASS" : "✗ FAIL")}");

// Test case: Large array with minimum at start
var largeMin = new int[100];
largeMin[0] = 0;
for (int i = 1; i < 100; i++) largeMin[i] = i + 100;
var stats4 = new StatisticsContext();
PDQSort.Sort(largeMin.AsSpan(), stats4);
var isSorted = true;
for (int i = 1; i < largeMin.Length; i++)
{
    if (largeMin[i] < largeMin[i - 1]) { isSorted = false; break; }
}
Console.WriteLine("\nTest 4 - Large array (100 elements) with min at start:");
Console.WriteLine($"  Status: {(isSorted ? "✓ PASS - Sorted correctly" : "✗ FAIL - Not sorted")}");

Console.WriteLine("\n=== All PartitionLeft boundary tests completed ===");
