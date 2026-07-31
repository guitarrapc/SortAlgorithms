#:project ../../src/SortAlgorithm

// Prints the observed operation counts of BucketSortInteger, used to re-derive the
// expectations in BucketSortIntegerTests after the per-bucket sort was routed through
// SortSpan (it previously ran on a raw Span and reported nothing).

using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

Console.WriteLine($"{"input",-18} {"n",4} {"reads",8} {"writes",8} {"compares",9} {"swaps",6} {"buckets",8} {"nonEmpty",9}");

foreach (var n in new[] { 10, 20, 50, 100 })
{
    Report("sorted", n, Enumerable.Range(0, n).ToArray());
    Report("reversed", n, Enumerable.Range(0, n).Reverse().ToArray());
    foreach (var seed in new[] { 42, 1234 })
    {
        Report($"random(seed={seed})", n, Shuffled(n, seed));
    }
}

static void Report(string label, int n, int[] data)
{
    var stats = new StatisticsContext();
    var copy = data.ToArray();
    BucketSortInteger.Sort(copy.AsSpan(), stats);

    // Mirror the algorithm's bucket geometry so the derivation is visible.
    var bucketCount = Math.Max(2, Math.Min(1000, (int)Math.Sqrt(n)));
    var min = data.Min();
    var max = data.Max();
    ulong range = (ulong)((long)max - min) + 1;
    if (range < (ulong)bucketCount) bucketCount = (int)range;
    var bucketSize = Math.Max(1UL, range / (ulong)bucketCount + (range % (ulong)bucketCount != 0 ? 1UL : 0UL));
    var counts = new int[bucketCount];
    foreach (var v in data)
    {
        var b = (int)((ulong)((long)v - min) / bucketSize);
        if (b >= bucketCount) b = bucketCount - 1;
        counts[b]++;
    }
    var nonEmpty = counts.Count(c => c > 0);

    Console.WriteLine($"{label,-18} {n,4} {stats.IndexReadCount,8} {stats.IndexWriteCount,8} {stats.CompareCount,9} {stats.SwapCount,6} {bucketCount,8} {nonEmpty,9}");
}

static int[] Shuffled(int n, int seed)
{
    var a = Enumerable.Range(0, n).ToArray();
    var rng = new Random(seed);
    for (var i = a.Length - 1; i > 0; i--)
    {
        var j = rng.Next(i + 1);
        (a[i], a[j]) = (a[j], a[i]);
    }
    return a;
}
