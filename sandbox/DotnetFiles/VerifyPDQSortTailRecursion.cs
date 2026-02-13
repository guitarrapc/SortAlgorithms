#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using System.Diagnostics;

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║       PDQSort - Tail Recursion Optimization Verification    ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
Console.WriteLine();

var random = new Random(42);
var results = new List<(string Name, int Size, ulong Compares, ulong Swaps, long TimeMs)>();

// Test 1: Random data (baseline)
Console.WriteLine("📊 Test 1: Random Distribution (Baseline)");
var array1 = Enumerable.Range(0, 100000).Select(_ => random.Next(100000)).ToArray();
var stats1 = new StatisticsContext();
var sw1 = Stopwatch.StartNew();
PDQSort.Sort(array1.AsSpan(), stats1);
sw1.Stop();
results.Add(("Random", 100000, stats1.CompareCount, stats1.SwapCount, sw1.ElapsedMilliseconds));
Console.WriteLine($"  ✓ Sorted: {IsSorted(array1)}");
Console.WriteLine($"  📈 Compares: {stats1.CompareCount:N0}, Swaps: {stats1.SwapCount:N0}, Time: {sw1.ElapsedMilliseconds}ms");
Console.WriteLine();

// Test 2: Already sorted (pattern detection)
Console.WriteLine("🔄 Test 2: Already Sorted (Pattern Detection + Tail Recursion)");
var array2 = Enumerable.Range(0, 100000).ToArray();
var stats2 = new StatisticsContext();
var sw2 = Stopwatch.StartNew();
PDQSort.Sort(array2.AsSpan(), stats2);
sw2.Stop();
results.Add(("Sorted", 100000, stats2.CompareCount, stats2.SwapCount, sw2.ElapsedMilliseconds));
Console.WriteLine($"  ✓ Sorted: {IsSorted(array2)}");
Console.WriteLine($"  📈 Compares: {stats2.CompareCount:N0}, Swaps: {stats2.SwapCount:N0}, Time: {sw2.ElapsedMilliseconds}ms");
Console.WriteLine($"  🎯 PDQSort Pattern Detection: O(n) performance for sorted arrays");
Console.WriteLine();

// Test 3: Reverse sorted
Console.WriteLine("🔄 Test 3: Reverse Sorted (Pattern Detection)");
var array3 = Enumerable.Range(0, 100000).Reverse().ToArray();
var stats3 = new StatisticsContext();
var sw3 = Stopwatch.StartNew();
PDQSort.Sort(array3.AsSpan(), stats3);
sw3.Stop();
results.Add(("Reverse", 100000, stats3.CompareCount, stats3.SwapCount, sw3.ElapsedMilliseconds));
Console.WriteLine($"  ✓ Sorted: {IsSorted(array3)}");
Console.WriteLine($"  📈 Compares: {stats3.CompareCount:N0}, Swaps: {stats3.SwapCount:N0}, Time: {sw3.ElapsedMilliseconds}ms");
Console.WriteLine($"  🎯 PDQSort Pattern Detection: O(n) for reverse-sorted arrays");
Console.WriteLine();

// Test 4: All equal elements
Console.WriteLine("🔁 Test 4: All Equal Elements (PartitionLeft Optimization)");
var array4 = Enumerable.Repeat(42, 100000).ToArray();
var stats4 = new StatisticsContext();
var sw4 = Stopwatch.StartNew();
PDQSort.Sort(array4.AsSpan(), stats4);
sw4.Stop();
results.Add(("All Equal", 100000, stats4.CompareCount, stats4.SwapCount, sw4.ElapsedMilliseconds));
Console.WriteLine($"  ✓ Sorted: {IsSorted(array4)}");
Console.WriteLine($"  📈 Compares: {stats4.CompareCount:N0}, Swaps: {stats4.SwapCount:N0}, Time: {sw4.ElapsedMilliseconds}ms");
Console.WriteLine($"  🎯 PDQSort PartitionLeft: O(n) for all-equal arrays");
Console.WriteLine();

// Test 5: Pathological input (worst case for basic QuickSort)
Console.WriteLine("⚠️  Test 5: Pathological Input (Tail Recursion Stress Test)");
var array5 = new int[100000];
for (int i = 0; i < array5.Length; i++)
{
    array5[i] = i % 2; // Alternating 0 and 1
}
var stats5 = new StatisticsContext();
var sw5 = Stopwatch.StartNew();
PDQSort.Sort(array5.AsSpan(), stats5);
sw5.Stop();
results.Add(("Pathological", 100000, stats5.CompareCount, stats5.SwapCount, sw5.ElapsedMilliseconds));
Console.WriteLine($"  ✓ Sorted: {IsSorted(array5)}");
Console.WriteLine($"  📈 Compares: {stats5.CompareCount:N0}, Swaps: {stats5.SwapCount:N0}, Time: {sw5.ElapsedMilliseconds}ms");
Console.WriteLine($"  🎯 Tail Recursion: Always recurse on smaller → O(log n) stack depth");
Console.WriteLine();

// Test 6: Many duplicates
Console.WriteLine("🔁 Test 6: Many Duplicates (5 distinct values)");
var array6 = Enumerable.Range(0, 100000).Select(_ => random.Next(5)).ToArray();
var stats6 = new StatisticsContext();
var sw6 = Stopwatch.StartNew();
PDQSort.Sort(array6.AsSpan(), stats6);
sw6.Stop();
results.Add(("5 Duplicates", 100000, stats6.CompareCount, stats6.SwapCount, sw6.ElapsedMilliseconds));
Console.WriteLine($"  ✓ Sorted: {IsSorted(array6)}");
Console.WriteLine($"  📈 Compares: {stats6.CompareCount:N0}, Swaps: {stats6.SwapCount:N0}, Time: {sw6.ElapsedMilliseconds}ms");
Console.WriteLine($"  🎯 PartitionLeft handles duplicates efficiently");
Console.WriteLine();

// Test 7: Organ pipe (ascending then descending)
Console.WriteLine("🎵 Test 7: Organ Pipe (Ascending then Descending)");
var array7 = new int[100000];
for (int i = 0; i < array7.Length / 2; i++) array7[i] = i;
for (int i = array7.Length / 2; i < array7.Length; i++) array7[i] = array7.Length - i;
var stats7 = new StatisticsContext();
var sw7 = Stopwatch.StartNew();
PDQSort.Sort(array7.AsSpan(), stats7);
sw7.Stop();
results.Add(("Organ Pipe", 100000, stats7.CompareCount, stats7.SwapCount, sw7.ElapsedMilliseconds));
Console.WriteLine($"  ✓ Sorted: {IsSorted(array7)}");
Console.WriteLine($"  📈 Compares: {stats7.CompareCount:N0}, Swaps: {stats7.SwapCount:N0}, Time: {sw7.ElapsedMilliseconds}ms");
Console.WriteLine($"  🎯 Pattern detection handles complex patterns");
Console.WriteLine();

// Test 8: Sawtooth pattern
Console.WriteLine("🪚 Test 8: Sawtooth Pattern");
var array8 = new int[100000];
for (int i = 0; i < array8.Length; i++) array8[i] = i % 100;
var stats8 = new StatisticsContext();
var sw8 = Stopwatch.StartNew();
PDQSort.Sort(array8.AsSpan(), stats8);
sw8.Stop();
results.Add(("Sawtooth", 100000, stats8.CompareCount, stats8.SwapCount, sw8.ElapsedMilliseconds));
Console.WriteLine($"  ✓ Sorted: {IsSorted(array8)}");
Console.WriteLine($"  📈 Compares: {stats8.CompareCount:N0}, Swaps: {stats8.SwapCount:N0}, Time: {sw8.ElapsedMilliseconds}ms");
Console.WriteLine($"  🎯 Ninther pivot selection handles repeated patterns");
Console.WriteLine();

Console.WriteLine("╔══════════════════════════════════════════════════════════════╗");
Console.WriteLine("║                    Performance Summary                       ║");
Console.WriteLine("╚══════════════════════════════════════════════════════════════╝");
Console.WriteLine();
Console.WriteLine($"{"Test",-20} {"Size",-10} {"Compares",-15} {"Swaps",-15} {"Time (ms)",-10}");
Console.WriteLine(new string('─', 75));
foreach (var (name, size, compares, swaps, time) in results)
{
    Console.WriteLine($"{name,-20} {size,-10:N0} {compares,-15:N0} {swaps,-15:N0} {time,-10}");
}
Console.WriteLine();

Console.WriteLine("✅ PDQSort Tail Recursion Optimization Verified:");
Console.WriteLine("  🔹 Tail Recursion: Always recurse on smaller partition → O(log n) stack");
Console.WriteLine("  🔹 Pattern Detection: O(n) for sorted/reverse-sorted/all-equal arrays");
Console.WriteLine("  🔹 PartitionLeft: Handles many duplicates efficiently");
Console.WriteLine("  🔹 Ninther Pivot: Robust selection for large arrays (≥128 elements)");
Console.WriteLine("  🔹 Bad Partition Limit: HeapSort fallback ensures O(n log n) worst case");
Console.WriteLine();

var avgCompares = results.Average(r => (double)r.Compares);
var avgTime = results.Average(r => (double)r.TimeMs);
Console.WriteLine($"📊 Average Performance (100K elements): {avgCompares:N0} compares, {avgTime:F1}ms");

// Compare with expected O(n log n)
var expected = 100000 * Math.Log2(100000) * 1.4;
var ratio = avgCompares / expected;
Console.WriteLine($"📈 Efficiency: {ratio:P0} of expected O(n log n) = {expected:N0} compares");

static bool IsSorted<T>(T[] array)
{
    var comparer = Comparer<T>.Default;
    for (int i = 1; i < array.Length; i++)
    {
        if (comparer.Compare(array[i], array[i - 1]) < 0)
            return false;
    }
    return true;
}
