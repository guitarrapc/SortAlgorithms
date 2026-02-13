#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

// Test case: Pivot is maximum (critical bug case)
// [9, 1, 2, 3, 4] - pivot=9 is maximum, should end up at the end
var pivotIsMax = new[] { 9, 1, 2, 3, 4 };
var stats1 = new StatisticsContext();
PDQSort.Sort(pivotIsMax.AsSpan(), stats1);
Console.WriteLine("Test 1 - Pivot is maximum:");
Console.WriteLine($"  Input:  [9, 1, 2, 3, 4]");
Console.WriteLine($"  Result: [{string.Join(", ", pivotIsMax)}]");
Console.WriteLine($"  Expected: [1, 2, 3, 4, 9]");
Console.WriteLine($"  Status: {(string.Join(",", pivotIsMax) == "1,2,3,4,9" ? "✓ PASS" : "✗ FAIL")}");

// Test case: Pivot is minimum
// [0, 5, 6, 7, 8] - pivot=0 is minimum, should stay at the start
var pivotIsMin = new[] { 0, 5, 6, 7, 8 };
var stats2 = new StatisticsContext();
PDQSort.Sort(pivotIsMin.AsSpan(), stats2);
Console.WriteLine("\nTest 2 - Pivot is minimum:");
Console.WriteLine($"  Input:  [0, 5, 6, 7, 8]");
Console.WriteLine($"  Result: [{string.Join(", ", pivotIsMin)}]");
Console.WriteLine($"  Expected: [0, 5, 6, 7, 8]");
Console.WriteLine($"  Status: {(string.Join(",", pivotIsMin) == "0,5,6,7,8" ? "✓ PASS" : "✗ FAIL")}");

// Test case: Large array with pivot as maximum
var largeMax = new int[100];
largeMax[0] = 999;  // pivot is maximum
for (int i = 1; i < 100; i++) largeMax[i] = i;
var stats3 = new StatisticsContext();
PDQSort.Sort(largeMax.AsSpan(), stats3);
var isSorted = true;
for (int i = 1; i < largeMax.Length; i++)
{
    if (largeMax[i] < largeMax[i - 1]) { isSorted = false; break; }
}
Console.WriteLine("\nTest 3 - Large array (100 elements) with pivot as maximum:");
Console.WriteLine($"  Status: {(isSorted && largeMax[99] == 999 ? "✓ PASS - Sorted correctly, 999 at end" : "✗ FAIL")}");

// Test case: All elements less than pivot
var allLessThanPivot = new[] { 10, 1, 2, 3, 4, 5 }; // pivot=10, all others < pivot
var stats4 = new StatisticsContext();
PDQSort.Sort(allLessThanPivot.AsSpan(), stats4);
Console.WriteLine("\nTest 4 - All elements less than pivot:");
Console.WriteLine($"  Input:  [10, 1, 2, 3, 4, 5]");
Console.WriteLine($"  Result: [{string.Join(", ", allLessThanPivot)}]");
Console.WriteLine($"  Expected: [1, 2, 3, 4, 5, 10]");
Console.WriteLine($"  Status: {(string.Join(",", allLessThanPivot) == "1,2,3,4,5,10" ? "✓ PASS" : "✗ FAIL")}");

// Test case: All elements greater than pivot
var allGreaterThanPivot = new[] { 0, 5, 6, 7, 8, 9 }; // pivot=0, all others > pivot
var stats5 = new StatisticsContext();
PDQSort.Sort(allGreaterThanPivot.AsSpan(), stats5);
Console.WriteLine("\nTest 5 - All elements greater than pivot:");
Console.WriteLine($"  Input:  [0, 5, 6, 7, 8, 9]");
Console.WriteLine($"  Result: [{string.Join(", ", allGreaterThanPivot)}]");
Console.WriteLine($"  Expected: [0, 5, 6, 7, 8, 9]");
Console.WriteLine($"  Status: {(string.Join(",", allGreaterThanPivot) == "0,5,6,7,8,9" ? "✓ PASS" : "✗ FAIL")}");

// Test case: Two elements - pivot is max
var twoElemsMax = new[] { 2, 1 };
var stats6 = new StatisticsContext();
PDQSort.Sort(twoElemsMax.AsSpan(), stats6);
Console.WriteLine("\nTest 6 - Two elements, pivot is max:");
Console.WriteLine($"  Input:  [2, 1]");
Console.WriteLine($"  Result: [{string.Join(", ", twoElemsMax)}]");
Console.WriteLine($"  Expected: [1, 2]");
Console.WriteLine($"  Status: {(string.Join(",", twoElemsMax) == "1,2" ? "✓ PASS" : "✗ FAIL")}");

Console.WriteLine("\n=== All pivot boundary tests completed ===");
Console.WriteLine("These tests verify that first can reach end and last can reach begin.");
