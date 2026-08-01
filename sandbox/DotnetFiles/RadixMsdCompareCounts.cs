#:project ../../src/SortAlgorithm

using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

// Comparison counts for the MSD radix sorts, so the class docs can state what the insertion-sort cutoff
// actually costs instead of claiming zero comparisons. Deterministic: no timing involved.

const int N = 100_000;
var rng = new Random(42);

var patterns = new (string Label, int[] Data)[]
{
    ("uniform random int", Enumerable.Range(0, N).Select(_ => rng.Next(int.MinValue, int.MaxValue)).ToArray()),
    ("already sorted 0..n", Enumerable.Range(0, N).ToArray()),
    ("keys 0..999",         Enumerable.Range(0, N).Select(_ => rng.Next(0, 1000)).ToArray()),
    ("all equal",           Enumerable.Repeat(7, N).ToArray()),
};

foreach (var (label, data) in patterns)
{
    var a = data.ToArray();
    var b = data.ToArray();
    var s4 = new StatisticsContext();
    var s10 = new StatisticsContext();
    RadixMSD4Sort.Sort(a.AsSpan(), s4);
    RadixMSD10Sort.Sort(b.AsSpan(), s10);
    Console.WriteLine($"  {label,-20} MSD4 compares={s4.CompareCount,10:N0}   MSD10 compares={s10.CompareCount,10:N0}");
}
