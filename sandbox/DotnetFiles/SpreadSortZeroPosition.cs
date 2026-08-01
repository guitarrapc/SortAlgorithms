#:project ../../src/SortAlgorithm

using SortAlgorithm.Algorithms;

// Boost's float_sort documentation claims -0.0 and +0.0 get "definitive ordered positions". This checks
// whether THIS SpreadSort ever had that property, at sizes above min_sort_size = 1000 where the
// distribution path actually runs (below it the sort is just PDQSort).

foreach (var n in new[] { 1024, 4096, 65536 })
{
    // -0.0 and +0.0 alternating, embedded in a spread of real values so the bins are not degenerate.
    var rng = new Random(42);
    var a = new double[n];
    for (var i = 0; i < n; i++)
        a[i] = i % 3 == 0 ? +0.0 : i % 3 == 1 ? -0.0 : rng.NextDouble() * 2000.0 - 1000.0;

    var zerosBefore = a.Count(x => x == 0.0);
    SpreadSort.Sort(a.AsSpan());

    var zeroRun = a.Where(x => x == 0.0).ToArray();
    var allNegativeFirst = zeroRun.TakeWhile(double.IsNegative).Count() == zeroRun.Count(double.IsNegative);
    var inputOrder = string.Concat(zeroRun.Take(8).Select(x => double.IsNegative(x) ? '-' : '+'));

    Console.WriteLine($"n={n,6}  zeros={zeroRun.Length}/{zerosBefore}  first8OfZeroRun={inputOrder}  allNegativeGroupedFirst={allNegativeFirst}");
}
