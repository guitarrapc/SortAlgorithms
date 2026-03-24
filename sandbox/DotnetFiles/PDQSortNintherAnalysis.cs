#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

// Test 1: Sorted array (size > NintherThreshold)
Console.WriteLine("=== Test 1: Sorted Array (200 elements) ===");
var sortedArray = Enumerable.Range(1, 200).ToArray();
var stats1 = new StatisticsContext();
PDQSort.Sort(sortedArray.AsSpan(), stats1);

Console.WriteLine($"Compares: {stats1.CompareCount}");
Console.WriteLine($"Swaps: {stats1.SwapCount}");
Console.WriteLine($"IndexWrites: {stats1.IndexWriteCount}");
Console.WriteLine($"Is sorted correctly: {IsSorted(sortedArray)}");
Console.WriteLine();

// Test 2: Mountain array (size > NintherThreshold)
Console.WriteLine("=== Test 2: Mountain Array (200 elements) ===");
var mountainArray = new int[200];
for (int i = 0; i < 100; i++) mountainArray[i] = i + 1;
for (int i = 100; i < 200; i++) mountainArray[i] = 200 - i;
var stats2 = new StatisticsContext();
PDQSort.Sort(mountainArray.AsSpan(), stats2);

Console.WriteLine($"Compares: {stats2.CompareCount}");
Console.WriteLine($"Swaps: {stats2.SwapCount}");
Console.WriteLine($"IndexWrites: {stats2.IndexWriteCount}");
Console.WriteLine($"Is sorted correctly: {IsSorted(mountainArray)}");
Console.WriteLine();

// Test 3: Random array for comparison (size > NintherThreshold)
Console.WriteLine("=== Test 3: Random Array (200 elements) ===");
var random = new Random(42);
var randomArray = Enumerable.Range(1, 200).OrderBy(_ => random.Next()).ToArray();
var stats3 = new StatisticsContext();
PDQSort.Sort(randomArray.AsSpan(), stats3);

Console.WriteLine($"Compares: {stats3.CompareCount}");
Console.WriteLine($"Swaps: {stats3.SwapCount}");
Console.WriteLine($"IndexWrites: {stats3.IndexWriteCount}");
Console.WriteLine($"Is sorted correctly: {IsSorted(randomArray)}");
Console.WriteLine();

// Test 4: Small test to manually trace what happens
Console.WriteLine("=== Test 4: Manual Trace (150 elements, first iteration) ===");
var traceArray = Enumerable.Range(1, 150).ToArray();
Console.WriteLine($"Initial state (showing first and middle elements):");
Console.WriteLine($"  array[0] = {traceArray[0]}, array[75] = {traceArray[75]}, array[149] = {traceArray[149]}");
Console.WriteLine("  After ninther + swap(begin, begin+s2), begin should have median value.");
Console.WriteLine("  But in sorted array: array[0]=1 is smallest, array[75]=76 is middle.");
Console.WriteLine("  Swapping them puts 76 at position 0, which is WRONG for sorted input!");
Console.WriteLine();

static bool IsSorted(int[] array)
{
    for (int i = 1; i < array.Length; i++)
    {
        if (array[i] < array[i - 1]) return false;
    }
    return true;
}
