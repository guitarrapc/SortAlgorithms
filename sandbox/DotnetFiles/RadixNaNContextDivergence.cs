#:project ../../src/SortAlgorithm

using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

// SortSpan only specializes ComparableComparer to the raw IEEE operators when TContext is NullContext.
// If that specialization is what breaks NaN at the insertion-sort cutoff, then the same input sorted
// with an observation context attached must come out differently.

static string Run(Action<double[]> sort)
{
    var rng = new Random(42);
    var a = new double[40];
    for (var i = 0; i < a.Length; i++) a[i] = i % 3 == 0 ? double.NaN : rng.NextDouble() * 200.0 - 100.0;
    sort(a);
    return string.Concat(a.Take(12).Select(x => double.IsNaN(x) ? "N" : x < 0 ? "-" : "+"));
}

var expected = Run(a => Array.Sort(a));
Console.WriteLine($"Array.Sort                              {expected}");
Console.WriteLine($"RadixMSD4Sort.Sort(span)                {Run(a => RadixMSD4Sort.Sort(a.AsSpan()))}");
Console.WriteLine($"RadixMSD4Sort.Sort(span, statistics)    {Run(a => RadixMSD4Sort.Sort(a.AsSpan(), new StatisticsContext()))}");
Console.WriteLine($"AmericanFlagSort.Sort(span)             {Run(a => AmericanFlagSort.Sort(a.AsSpan()))}");
Console.WriteLine($"AmericanFlagSort.Sort(span, statistics) {Run(a => AmericanFlagSort.Sort(a.AsSpan(), new StatisticsContext()))}");
