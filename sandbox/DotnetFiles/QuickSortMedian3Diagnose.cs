#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:property Configuration=Release
#:project ../../src/SortAlgorithm

// Diagnose QuickSortMedian3 pathology on nearly-sorted data.
// Hypothesis: the Dijkstra DNF 3-way partition combined with "swap pivot to right"
// scrambles the > region into a rotated pattern, so the next level's median-of-3
// picks a near-minimum pivot -> unbalanced partitions + O(k) swaps per level.

using System.Diagnostics;
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

static int[] Gen(string pattern, int n, int seed = 42)
{
    var rng = new Random(seed);
    return pattern switch
    {
        "sorted" => Enumerable.Range(0, n).ToArray(),
        "disorder0.1%" => Enumerable.Range(0, n).Select(i => rng.NextDouble() < 0.001 ? rng.Next(n) : i).ToArray(),
        "disorder5%" => Enumerable.Range(0, n).Select(i => rng.NextDouble() < 0.05 ? rng.Next(n) : i).ToArray(),
        "reversed" => Enumerable.Range(0, n).Reverse().ToArray(),
        "random" => Enumerable.Range(0, n).Select(_ => rng.Next()).ToArray(),
        "dup10" => Enumerable.Range(0, n).Select(_ => rng.Next(10)).ToArray(),
        "allsame" => Enumerable.Repeat(7, n).ToArray(),
        _ => throw new ArgumentOutOfRangeException(nameof(pattern)),
    };
}

static double TimeMs(int[] template, Action<int[]> sort)
{
    // Warmup: let the JIT promote the NullContext instantiation to tier1 before measuring
    for (var i = 0; i < 3; i++)
    {
        var w = (int[])template.Clone();
        sort(w);
    }
    var times = new List<double>();
    for (var i = 0; i < 5; i++)
    {
        var c = (int[])template.Clone();
        var sw = Stopwatch.StartNew();
        sort(c);
        sw.Stop();
        if (!IsSorted(c)) Console.WriteLine("  !! NOT SORTED !!");
        times.Add(sw.Elapsed.TotalMilliseconds);
    }
    times.Sort();
    return times[2];
}

static bool IsSorted(int[] a)
{
    for (var i = 1; i < a.Length; i++) if (a[i - 1] > a[i]) return false;
    return true;
}

static (ulong compares, ulong swaps, ulong writes) Counts(int[] template, Action<int[], StatisticsContext> sort)
{
    var stats = new StatisticsContext();
    var c = (int[])template.Clone();
    sort(c, stats);
    return (stats.CompareCount, stats.SwapCount, stats.IndexWriteCount);
}

const int N = 1_000_000;
string[] patterns = ["sorted", "disorder0.1%", "disorder5%", "reversed", "random", "dup10", "allsame"];

Console.WriteLine($"n = {N}, counts are per element (compares/n, swaps/n)");
Console.WriteLine();
Console.WriteLine($"{"pattern",13} | {"QS(mid,Hoare)",16} | {"QSMedian3(DNF)",16} | {"QSMedian9(DNF)",16} | {"QS3way(DNF)",16}");
foreach (var p in patterns)
{
    var data = Gen(p, N);
    var qs = Counts(data, (a, st) => QuickSort.Sort(a.AsSpan(), st));
    var m3 = Counts(data, (a, st) => QuickSortMedian3.Sort(a.AsSpan(), st));
    var m9 = Counts(data, (a, st) => QuickSortMedian9.Sort(a.AsSpan(), st));
    var w3 = Counts(data, (a, st) => QuickSort3way.Sort(a.AsSpan(), st));
    static string F((ulong c, ulong s, ulong w) x) => $"c={(double)x.c / N,6:F1} s={(double)x.s / N,6:F1}";
    Console.WriteLine($"{p,13} | {F(qs)} | {F(m3)} | {F(m9)} | {F(w3)}");
}

Console.WriteLine();
Console.WriteLine($"{"pattern",13} | {"QS(mid,Hoare)",13} | {"QSMedian3",13} | {"QSMedian9",13} | {"QS3way",13}  (median of 5, ms)");
foreach (var p in patterns)
{
    var data = Gen(p, N);
    var qs = TimeMs(data, a => QuickSort.Sort(a.AsSpan()));
    var m3 = TimeMs(data, a => QuickSortMedian3.Sort(a.AsSpan()));
    var m9 = TimeMs(data, a => QuickSortMedian9.Sort(a.AsSpan()));
    var w3 = TimeMs(data, a => QuickSort3way.Sort(a.AsSpan()));
    Console.WriteLine($"{p,13} | {qs,13:F1} | {m3,13:F1} | {m9,13:F1} | {w3,13:F1}");
}
