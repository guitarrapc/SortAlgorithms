#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm

using System.Diagnostics;
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

Console.WriteLine("=== Pair Insertion Sort vs Standard Insertion Sort ===\n");

// Test different array sizes
int[] sizes = [10, 20, 50, 100];

foreach (var size in sizes)
{
    Console.WriteLine($"Array size: {size}");
    Console.WriteLine(new string('-', 60));

    // Create test data
    var random = new Random(42);
    var originalData = Enumerable.Range(0, size).Select(_ => random.Next(1000)).ToArray();

    // Test Standard Insertion Sort
    var data1 = originalData.ToArray();
    var stats1 = new StatisticsContext();
    var sw1 = Stopwatch.StartNew();
    InsertionSort.Sort(data1.AsSpan(), stats1);
    sw1.Stop();

    // Test Pair Insertion Sort
    var data2 = originalData.ToArray();
    var stats2 = new StatisticsContext();
    var sw2 = Stopwatch.StartNew();
    PairInsertionSort.Sort(data2.AsSpan(), stats2);
    sw2.Stop();

    // Verify both produce same result
    bool same = data1.SequenceEqual(data2);

    Console.WriteLine($"Standard Insertion Sort:");
    Console.WriteLine($"  Time: {sw1.Elapsed.TotalMicroseconds:F2} μs");
    Console.WriteLine($"  Compares: {stats1.CompareCount,8}");
    Console.WriteLine($"  Reads:    {stats1.IndexReadCount,8}");
    Console.WriteLine($"  Writes:   {stats1.IndexWriteCount,8}");

    Console.WriteLine($"\nPair Insertion Sort:");
    Console.WriteLine($"  Time: {sw2.Elapsed.TotalMicroseconds:F2} μs");
    Console.WriteLine($"  Compares: {stats2.CompareCount,8}");
    Console.WriteLine($"  Reads:    {stats2.IndexReadCount,8}");
    Console.WriteLine($"  Writes:   {stats2.IndexWriteCount,8}");

    Console.WriteLine($"\nComparison reduction: {(1.0 - (double)stats2.CompareCount / stats1.CompareCount) * 100:F1}%");
    Console.WriteLine($"Results match: {same}");
    Console.WriteLine();
}

Console.WriteLine("\n=== Edge Cases ===\n");

// Test edge cases
var testCases = new[]
{
    ("Empty array", Array.Empty<int>()),
    ("Single element", new[] { 42 }),
    ("Two elements (sorted)", new[] { 1, 2 }),
    ("Two elements (reversed)", new[] { 2, 1 }),
    ("Already sorted", new[] { 1, 2, 3, 4, 5 }),
    ("Reverse sorted", new[] { 5, 4, 3, 2, 1 }),
    ("All same", new[] { 5, 5, 5, 5, 5 }),
    ("Odd length", new[] { 5, 3, 8, 1, 2 }),
    ("Even length", new[] { 5, 3, 8, 1, 2, 4 }),
};

foreach (var (name, data) in testCases)
{
    var array = data.ToArray();
    var stats = new StatisticsContext();
    PairInsertionSort.Sort(array.AsSpan(), stats);
    var sorted = array.OrderBy(x => x).ToArray();
    var match = array.SequenceEqual(sorted);
    
    Console.WriteLine($"{name,-25} -> [{string.Join(", ", array)}] {(match ? "✓" : "✗")}");
}

Console.WriteLine("\n=== Demo Complete ===");
