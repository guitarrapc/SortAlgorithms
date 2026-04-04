#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:property AllowUnsafeBlocks=true
#:property Optimize=true
#:project ../../src/SortAlgorithm
using System.Diagnostics;
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

// Quick throughput benchmark to measure the impact of MemoryMarshal.GetReference + Unsafe.Add
// Applied to: Read, Write, Compare, IsLessAt, IsLessOrEqualAt, IsGreaterAt, IsGreaterOrEqualAt, Swap
//
// Algorithms exercised (cover the most-called NullContext paths):
//   InsertionSort  → O(n²) comparisons + swaps → stress-tests Compare/Swap hot path
//   PDQSort        → O(n log n) → stress-tests partition (Compare) + InsertionSort fallback
//   PowerSort      → O(n log n) adaptive → stress-tests IsLessAt / merge paths

const int WarmupRounds = 3;
const int BenchRounds = 10;
const int N = 4096;

var rng = new Random(42);
int[] seed = new int[N];
for (int i = 0; i < N; i++) seed[i] = rng.Next();

Console.WriteLine($"Array size: {N}   Rounds: {BenchRounds}   (after {WarmupRounds} warmups)");
Console.WriteLine();

RunBench("InsertionSort  (Random)  ", () =>
{
    var arr = (int[])seed.Clone();
    InsertionSort.Sort<int>(arr.AsSpan());
});

RunBench("InsertionSort  (Reversed)", () =>
{
    var arr = new int[N];
    for (int i = 0; i < N; i++) arr[i] = N - i;
    InsertionSort.Sort<int>(arr.AsSpan());
});

RunBench("PDQSort        (Random)  ", () =>
{
    var arr = (int[])seed.Clone();
    PDQSort.Sort<int>(arr.AsSpan());
});

RunBench("PDQSort        (Reversed)", () =>
{
    var arr = new int[N];
    for (int i = 0; i < N; i++) arr[i] = N - i;
    PDQSort.Sort<int>(arr.AsSpan());
});

RunBench("PowerSort      (Random)  ", () =>
{
    var arr = (int[])seed.Clone();
    PowerSort.Sort<int>(arr.AsSpan());
});

RunBench("PowerSort      (Reversed)", () =>
{
    var arr = new int[N];
    for (int i = 0; i < N; i++) arr[i] = N - i;
    PowerSort.Sort<int>(arr.AsSpan());
});

Console.WriteLine();
Console.WriteLine("Note: Run Release build for meaningful numbers.");
Console.WriteLine("      Compare with BenchmarkDotNet for micro-benchmark precision.");

static void RunBench(string label, Action action)
{
    // Warmup
    for (int i = 0; i < WarmupRounds; i++) action();

    // Measure
    var sw = Stopwatch.StartNew();
    for (int i = 0; i < BenchRounds; i++) action();
    sw.Stop();

    double avgUs = sw.Elapsed.TotalMicroseconds / BenchRounds;
    Console.WriteLine($"  {label}  avg = {avgUs,9:F1} µs");
}
