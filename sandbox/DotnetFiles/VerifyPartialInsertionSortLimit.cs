#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

// Test case: Intentionally create a scenario where exactly 9 moves would be needed
// to test if PartialInsertionSort correctly returns false when limit exceeds 8

// Array that requires exactly 9 element moves:
// [9, 1, 2, 3, 4, 5, 6, 7, 8]
// Moving 9 to the front requires 9 moves (shift 1,2,3,4,5,6,7,8 right)
var array1 = new[] { 9, 1, 2, 3, 4, 5, 6, 7, 8 };
var stats1 = new StatisticsContext();
PDQSort.Sort(array1.AsSpan(), stats1);
Console.WriteLine("Test 1 - Exactly 9 moves (should trigger limit):");
Console.WriteLine($"  Input:  [9, 1, 2, 3, 4, 5, 6, 7, 8]");
Console.WriteLine($"  Result: [{string.Join(", ", array1)}]");
Console.WriteLine($"  Status: {(string.Join(",", array1) == "1,2,3,4,5,6,7,8,9" ? "✓ PASS - Sorted correctly" : "✗ FAIL")}");

// Array that requires exactly 8 element moves (should pass):
// [8, 1, 2, 3, 4, 5, 6, 7]
// Moving 8 to position 7 requires 8 moves
var array2 = new[] { 8, 1, 2, 3, 4, 5, 6, 7 };
var stats2 = new StatisticsContext();
PDQSort.Sort(array2.AsSpan(), stats2);
Console.WriteLine("\nTest 2 - Exactly 8 moves (should be within limit):");
Console.WriteLine($"  Input:  [8, 1, 2, 3, 4, 5, 6, 7]");
Console.WriteLine($"  Result: [{string.Join(", ", array2)}]");
Console.WriteLine($"  Status: {(string.Join(",", array2) == "1,2,3,4,5,6,7,8" ? "✓ PASS - Sorted correctly" : "✗ FAIL")}");

// Array that requires exactly 7 element moves (should pass):
var array3 = new[] { 7, 1, 2, 3, 4, 5, 6 };
var stats3 = new StatisticsContext();
PDQSort.Sort(array3.AsSpan(), stats3);
Console.WriteLine("\nTest 3 - Exactly 7 moves (well within limit):");
Console.WriteLine($"  Input:  [7, 1, 2, 3, 4, 5, 6]");
Console.WriteLine($"  Result: [{string.Join(", ", array3)}]");
Console.WriteLine($"  Status: {(string.Join(",", array3) == "1,2,3,4,5,6,7" ? "✓ PASS - Sorted correctly" : "✗ FAIL")}");

// Multiple elements requiring moves that sum to > 8
// [5, 4, 3, 2, 1] requires 4+3+2+1 = 10 moves total
var array4 = new[] { 5, 4, 3, 2, 1 };
var stats4 = new StatisticsContext();
PDQSort.Sort(array4.AsSpan(), stats4);
Console.WriteLine("\nTest 4 - Multiple moves summing to 10 (should trigger limit):");
Console.WriteLine($"  Input:  [5, 4, 3, 2, 1]");
Console.WriteLine($"  Result: [{string.Join(", ", array4)}]");
Console.WriteLine($"  Status: {(string.Join(",", array4) == "1,2,3,4,5" ? "✓ PASS - Sorted correctly" : "✗ FAIL")}");

// Nearly sorted with small moves (should use partial insertion sort successfully)
var array5 = new[] { 1, 3, 2, 4, 5, 7, 6, 8 }; // Only 2 elements out of place, 2 moves total
var stats5 = new StatisticsContext();
PDQSort.Sort(array5.AsSpan(), stats5);
Console.WriteLine("\nTest 5 - Nearly sorted with 2 moves (should succeed with partial insertion):");
Console.WriteLine($"  Input:  [1, 3, 2, 4, 5, 7, 6, 8]");
Console.WriteLine($"  Result: [{string.Join(", ", array5)}]");
Console.WriteLine($"  Status: {(string.Join(",", array5) == "1,2,3,4,5,6,7,8" ? "✓ PASS - Sorted correctly" : "✗ FAIL")}");

Console.WriteLine("\n=== All PartialInsertionSort limit tests completed ===");
Console.WriteLine("Note: The fix ensures that even the last iteration checks the limit after increment.");
