#:project ../../src/SortAlgorithm

using System.Diagnostics;
using SortAlgorithm.Algorithms;

// Cost of the -0 normalization added to the IEEE key selectors. It sits on the float/double/Half key
// path only, so an int sort is measured in the same run as an anchor: any movement there is machine
// drift, not the edit.

static double Time<T>(T[] source, int reps, Action<T[]> sort)
{
    var work = new T[source.Length];
    for (var i = 0; i < Math.Max(3, reps / 4); i++) { source.CopyTo(work, 0); sort(work); }

    var best = double.MaxValue;
    for (var iteration = 0; iteration < reps; iteration++)
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

foreach (var n in new[] { 65536, 1048576 })
{
    var reps = n >= 1048576 ? 41 : 71;
    var doubles = Enumerable.Range(0, n).Select(_ => rng.NextDouble() * 2000.0 - 1000.0).ToArray();
    var floats = doubles.Select(x => (float)x).ToArray();
    var ints = Enumerable.Range(0, n).Select(_ => rng.Next(int.MinValue, int.MaxValue)).ToArray();

    var lsd256 = Time(doubles, reps, a => RadixLSD256Sort.Sort(a.AsSpan()));
    var msd4 = Time(doubles, reps, a => RadixMSD4Sort.Sort(a.AsSpan()));
    var spread = Time(doubles, reps, a => SpreadSort.Sort(a.AsSpan()));
    var lsd256f = Time(floats, reps, a => RadixLSD256Sort.Sort(a.AsSpan()));
    var anchor = Time(ints, reps, a => RadixLSD256Sort.Sort(a.AsSpan()));

    Console.WriteLine($"n={n,7}  LSD256<double> {lsd256,8:F4}   MSD4<double> {msd4,8:F4}   SpreadSort<double> {spread,8:F4}   LSD256<float> {lsd256f,8:F4}   LSD256<int>(anchor) {anchor,8:F4}");
}
