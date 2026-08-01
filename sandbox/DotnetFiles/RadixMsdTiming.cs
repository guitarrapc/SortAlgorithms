#:project ../../src/SortAlgorithm

using System.Diagnostics;
using SortAlgorithm.Algorithms;

// Wall clock for the MSD radix sorts on the NullContext path. Run once with the uniform-digit skip in
// place and once with it stashed out; the two runs are separate processes, so treat anything inside the
// machine's ~10-15% drift as noise. AmericanFlagSort is measured in the same run as an anchor: it is not
// being changed, so a shift in its number is drift, not the edit.

// Minimum, not median: MSD10 in particular swings by ~75% run to run even on inputs this edit does not
// touch, and the minimum is the sample least contaminated by whatever else the machine was doing.
static double Time(int[] source, Action<int[]> sort)
{
    var work = new int[source.Length];
    for (var i = 0; i < 20; i++) { source.CopyTo(work, 0); sort(work); }   // warmup

    var best = double.MaxValue;
    for (var iteration = 0; iteration < 101; iteration++)
    {
        source.CopyTo(work, 0);
        var sw = Stopwatch.StartNew();
        sort(work);
        sw.Stop();
        best = Math.Min(best, sw.Elapsed.TotalMilliseconds);
    }
    return best;
}

var rng = new Random(42);

foreach (var n in new[] { 8192, 65536 })
{
    var cases = new (string Label, int[] Data)[]
    {
        ("small 0..999",         Enumerable.Range(0, n).Select(_ => rng.Next(0, 1000)).ToArray()),
        ("straddling -500..499", Enumerable.Range(0, n).Select(_ => rng.Next(-500, 500)).ToArray()),
        ("full int range",       Enumerable.Range(0, n).Select(_ => rng.Next(int.MinValue, int.MaxValue)).ToArray()),
        ("all identical",        Enumerable.Repeat(7, n).ToArray()),
    };

    foreach (var (label, data) in cases)
    {
        var msd4 = Time(data, a => RadixMSD4Sort.Sort(a.AsSpan()));
        var msd10 = Time(data, a => RadixMSD10Sort.Sort(a.AsSpan()));
        var flag = Time(data, a => AmericanFlagSort.Sort(a.AsSpan()));
        Console.WriteLine($"n={n,6} {label,-22} MSD4 {msd4,8:F3} ms   MSD10 {msd10,8:F3} ms   AmericanFlag(anchor) {flag,8:F3} ms");
    }
}
