#:project ../../src/SortAlgorithm

using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

// Backs the figures quoted in the AmericanFlagSort class documentation.
const int N = 100_000;

static (ulong Reads, ulong Writes, ulong Compares, ulong Swaps, bool Sorted) Measure(int[] source, Action<int[], StatisticsContext> sort)
{
    var a = source.ToArray();
    var stats = new StatisticsContext();
    sort(a, stats);
    var sorted = true;
    for (var i = 1; i < a.Length; i++) if (a[i - 1] > a[i]) { sorted = false; break; }
    return (stats.IndexReadCount, stats.IndexWriteCount, stats.CompareCount, stats.SwapCount, sorted);
}

static int[] Generate(Func<Random, int, int> gen)
{
    var rnd = new Random(42);
    var a = new int[N];
    for (var i = 0; i < N; i++) a[i] = gen(rnd, i);
    return a;
}

Console.WriteLine($"== AmericanFlagSort per input shape (n={N})");
foreach (var (label, gen) in new (string, Func<Random, int, int>)[]
{
    ("all equal (42)", (_, _) => 42),
    ("0..999", (r, _) => r.Next(0, 1000)),
    ("-500..500", (r, _) => r.Next(-500, 501)),
    ("full int range", (r, _) => r.Next(int.MinValue, int.MaxValue)),
    ("already sorted 0..n", (_, i) => i),
})
{
    var m = Measure(Generate(gen), static (a, c) => AmericanFlagSort.Sort(a.AsSpan(), c));
    Console.WriteLine($"  {label,-22} sorted={m.Sorted}  reads={m.Reads,9} ({(double)m.Reads / N,6:F2} n)  writes={m.Writes,9}  compares={m.Compares,8}  swaps={m.Swaps,9}");
}

Console.WriteLine();
Console.WriteLine($"== Family comparison, uniform random int (n={N})");
var src = Generate(static (r, _) => r.Next(int.MinValue, int.MaxValue));
foreach (var (label, sort) in new (string, Action<int[], StatisticsContext>)[]
{
    ("AmericanFlagSort", static (a, c) => AmericanFlagSort.Sort(a.AsSpan(), c)),
    ("RadixMSD4Sort", static (a, c) => RadixMSD4Sort.Sort(a.AsSpan(), c)),
    ("RadixLSD256Sort", static (a, c) => RadixLSD256Sort.Sort(a.AsSpan(), c)),
})
{
    var m = Measure(src, sort);
    Console.WriteLine($"  {label,-22} sorted={m.Sorted}  reads={m.Reads,9}  writes={m.Writes,9}  compares={m.Compares,8}  swaps={m.Swaps,9}");
}
