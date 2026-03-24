#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

Console.WriteLine("RadixLSD4Sort vs RadixMSD4Sort Comparison");
Console.WriteLine("==========================================");
Console.WriteLine();

void TestAndCompare(string testName, int[] originalArray)
{
    Console.WriteLine($"Test: {testName}");
    Console.WriteLine($"Array: [{string.Join(", ", originalArray.Take(10))}{(originalArray.Length > 10 ? "..." : "")}] ({originalArray.Length} elements)");
    Console.WriteLine();

    // Test LSD
    var lsdStats = new StatisticsContext();
    var lsdArray = (int[])originalArray.Clone();
    RadixLSD4Sort.Sort(lsdArray.AsSpan(), lsdStats);

    // Test MSD
    var msdStats = new StatisticsContext();
    var msdArray = (int[])originalArray.Clone();
    RadixMSD4Sort.Sort(msdArray.AsSpan(), msdStats);

    // Verify both produce the same result
    bool resultsMatch = lsdArray.SequenceEqual(msdArray);

    Console.WriteLine($"  RadixLSD4Sort:");
    Console.WriteLine($"    IndexReads:  {lsdStats.IndexReadCount,8}");
    Console.WriteLine($"    IndexWrites: {lsdStats.IndexWriteCount,8}");
    Console.WriteLine($"    Compares:    {lsdStats.CompareCount,8}");
    Console.WriteLine($"    Swaps:       {lsdStats.SwapCount,8}");
    Console.WriteLine();

    Console.WriteLine($"  RadixMSD4Sort:");
    Console.WriteLine($"    IndexReads:  {msdStats.IndexReadCount,8}");
    Console.WriteLine($"    IndexWrites: {msdStats.IndexWriteCount,8}");
    Console.WriteLine($"    Compares:    {msdStats.CompareCount,8}");
    Console.WriteLine($"    Swaps:       {msdStats.SwapCount,8}");
    Console.WriteLine();

    Console.WriteLine($"  Results Match: {(resultsMatch ? "✓ PASS" : "✗ FAIL")}");
    Console.WriteLine($"  Read Difference:  {(long)msdStats.IndexReadCount - (long)lsdStats.IndexReadCount,8} ({(msdStats.IndexReadCount < lsdStats.IndexReadCount ? "MSD better" : "LSD better")})");
    Console.WriteLine($"  Write Difference: {(long)msdStats.IndexWriteCount - (long)lsdStats.IndexWriteCount,8} ({(msdStats.IndexWriteCount < lsdStats.IndexWriteCount ? "MSD better" : "LSD better")})");
    Console.WriteLine();
    Console.WriteLine(new string('-', 70));
    Console.WriteLine();
}

// Test 1: Small random array
TestAndCompare("Small Random Array", new[] { 5, 2, 8, 1, 9, 3, 7, 4, 6 });

// Test 2: Sorted array
TestAndCompare("Sorted Array (10 elements)", Enumerable.Range(0, 10).ToArray());

// Test 3: Reversed array
TestAndCompare("Reversed Array (10 elements)", Enumerable.Range(0, 10).Reverse().ToArray());

// Test 4: Array with negative numbers
TestAndCompare("Mixed Negative/Positive", new[] { -5, 3, -1, 0, 2, -3, 1, -10, 15, 20 });

// Test 5: Larger sorted array
TestAndCompare("Sorted Array (100 elements)", Enumerable.Range(0, 100).ToArray());

// Test 6: Larger reversed array
TestAndCompare("Reversed Array (100 elements)", Enumerable.Range(0, 100).Reverse().ToArray());

// Test 7: Random array
var random = new Random(42);
TestAndCompare("Random Array (100 elements)", Enumerable.Range(0, 100).OrderBy(_ => random.Next()).ToArray());

// Test 8: Array with many duplicates
TestAndCompare("Many Duplicates", Enumerable.Range(0, 100).Select(x => x / 10).ToArray());

Console.WriteLine("Summary:");
Console.WriteLine("--------");
Console.WriteLine("LSD (Least Significant Digit):");
Console.WriteLine("  - Processes digits from right to left (least to most significant)");
Console.WriteLine("  - Iterative approach with fixed number of passes");
Console.WriteLine("  - Stable sort (preserves relative order)");
Console.WriteLine("  - No comparisons needed (pure distribution sort)");
Console.WriteLine();
Console.WriteLine("MSD (Most Significant Digit):");
Console.WriteLine("  - Processes digits from left to right (most to least significant)");
Console.WriteLine("  - Recursive approach with early termination");
Console.WriteLine("  - Not stable (but can be made stable with extra work)");
Console.WriteLine("  - Uses insertion sort for small buckets (includes comparisons)");
Console.WriteLine("  - Can be faster on partially sorted or small-range data");
