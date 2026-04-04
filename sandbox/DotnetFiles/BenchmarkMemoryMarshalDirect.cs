#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:property AllowUnsafeBlocks=true
#:property Optimize=true
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// Direct micro-benchmark: Span[i] indexer (with bounds check) vs Unsafe.Add (no bounds check)
// Simulates the hot patterns inside SortSpan:
//   - sequential read (scan loop like InsertionSort)
//   - read-compare (like IsLessAt inner loop)
//   - insertion sort inner loop (backwards scan + shift)

const int WarmupRounds = 5;
const int BenchRounds = 20;
const int N = 4096;

var rng = new Random(42);
int[] seed = new int[N];
for (int i = 0; i < N; i++) seed[i] = rng.Next();

Console.WriteLine($"N={N}  Rounds={BenchRounds}  (warmup={WarmupRounds})");
Console.WriteLine($"{"Method",-40}  {"avg µs",10}  {"ns/elem",8}");
Console.WriteLine(new string('-', 64));

// ------- Sequential scan sum (read only) -------
RunBench("SpanIndexer: sequential scan sum", () =>
{
    var span = seed.AsSpan();
    long sum = 0;
    for (int i = 0; i < span.Length; i++)
        sum += span[i];
    _ = sum;
});

RunBench("Unsafe.Add:  sequential scan sum", () =>
{
    var span = seed.AsSpan();
    ref int r = ref MemoryMarshal.GetReference(span);
    long sum = 0;
    for (int i = 0; i < span.Length; i++)
        sum += Unsafe.Add(ref r, i);
    _ = sum;
});

Console.WriteLine();

// ------- Compare loop (read two, compare) -------
RunBench("SpanIndexer: compare loop (i vs i+1)", () =>
{
    var span = seed.AsSpan();
    int count = 0;
    for (int i = 0; i < span.Length - 1; i++)
        if (span[i] > span[i + 1]) count++;
    _ = count;
});

RunBench("Unsafe.Add:  compare loop (i vs i+1)", () =>
{
    var span = seed.AsSpan();
    ref int r = ref MemoryMarshal.GetReference(span);
    int count = 0;
    for (int i = 0; i < span.Length - 1; i++)
        if (Unsafe.Add(ref r, i) > Unsafe.Add(ref r, i + 1)) count++;
    _ = count;
});

Console.WriteLine();

// ------- Insertion sort inner loop (backwards scan + shift) -------
RunBench("SpanIndexer: insertion sort", () =>
{
    var arr = (int[])seed.Clone();
    var span = arr.AsSpan();
    for (int i = 1; i < span.Length; i++)
    {
        int key = span[i];
        int j = i - 1;
        while (j >= 0 && span[j] > key)
        {
            span[j + 1] = span[j];
            j--;
        }
        span[j + 1] = key;
    }
});

RunBench("Unsafe.Add:  insertion sort", () =>
{
    var arr = (int[])seed.Clone();
    var span = arr.AsSpan();
    ref int r = ref MemoryMarshal.GetReference(span);
    for (int i = 1; i < span.Length; i++)
    {
        int key = Unsafe.Add(ref r, i);
        int j = i - 1;
        while (j >= 0 && Unsafe.Add(ref r, j) > key)
        {
            Unsafe.Add(ref r, j + 1) = Unsafe.Add(ref r, j);
            j--;
        }
        Unsafe.Add(ref r, j + 1) = key;
    }
});

Console.WriteLine();
Console.WriteLine("Note: Unsafe.Add bypasses span bounds checks per access.");
Console.WriteLine("      JIT often eliminates bounds checks in simple forward loops via range analysis,");
Console.WriteLine("      but the irregular backwards scan in InsertionSort benefits visibly.");

void RunBench(string label, Action action)
{
    for (int i = 0; i < WarmupRounds; i++) action();

    var sw = Stopwatch.StartNew();
    for (int i = 0; i < BenchRounds; i++) action();
    sw.Stop();

    double avgUs = sw.Elapsed.TotalMicroseconds / BenchRounds;
    double nsPerElem = avgUs * 1000.0 / N;
    Console.WriteLine($"  {label,-40}  {avgUs,10:F2}  {nsPerElem,8:F2}");
}
