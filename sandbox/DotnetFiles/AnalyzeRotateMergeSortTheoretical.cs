#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

Console.WriteLine("=== RotateMergeSort 理論値確認 ===\n");

void TestAndReport(string label, int[] data, int n)
{
    var stats = new StatisticsContext();
    RotateMergeSort.Sort(data.AsSpan(), stats);
    
    var logN = Math.Log2(n);
    var nlogn = n * logN;
    var nlog2n = n * logN * logN;
    
    Console.WriteLine($"{label} (n={n}):");
    Console.WriteLine($"  Compares:     {stats.CompareCount,6} (理論: O(n log n)={nlogn,6:F1}, O(n log²n)={nlog2n,6:F1})");
    Console.WriteLine($"  Swaps:        {stats.SwapCount,6} (Reverseで発生)");
    Console.WriteLine($"  IndexReads:   {stats.IndexReadCount,6}");
    Console.WriteLine($"  IndexWrites:  {stats.IndexWriteCount,6}");
    Console.WriteLine();
}

// Sorted data test
foreach (var n in new[] { 10, 20, 50, 100 })
{
    var sorted = Enumerable.Range(0, n).ToArray();
    TestAndReport($"Sorted", sorted, n);
}

Console.WriteLine("\n" + new string('=', 60) + "\n");

// Reversed data test
foreach (var n in new[] { 10, 20, 50, 100 })
{
    var reversed = Enumerable.Range(0, n).Reverse().ToArray();
    TestAndReport($"Reversed", reversed, n);
}

Console.WriteLine("\n" + new string('=', 60) + "\n");

// Random data test
foreach (var n in new[] { 10, 20, 50, 100 })
{
    var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
    TestAndReport($"Random", random, n);
}

Console.WriteLine("\n✅ この結果を元にテストの理論値を調整します");
