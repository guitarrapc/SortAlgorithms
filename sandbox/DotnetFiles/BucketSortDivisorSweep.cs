#:project ../../src/SortAlgorithm

using SortAlgorithm.Utils;
using System.Diagnostics;

// Picks the bucket-count divisor c in k = n/c. Replicates the real inner shape
// (count -> prefix sum -> scatter into temp -> insertion sort each bucket -> copy back)
// so the trade-off measured is the real one: larger c means fewer bucket-array slots to
// scan but more elements per insertion sort.
//
// Reports the minimum of several runs, which is the statistic least disturbed by
// scheduling noise on this machine.

int[] sizes = [4096, 16384, 65536, 262144];
int[] divisors = [1, 2, 4, 8, 16, 32];

foreach (var pattern in new[] { "Random", "Reversed", "Sorted", "ManyDuplicates", "SparseRandom", "Clustered" })
{
    Console.WriteLine($"=== {pattern} — microseconds (min of 7), k = n/c ===");
    Console.Write($"{"n",8} |");
    foreach (var c in divisors) Console.Write($" {"c=" + c,10}");
    Console.WriteLine($" | {"sqrt(n)",10}  (current)");

    foreach (var n in sizes)
    {
        var source = Gen(pattern, n);
        Console.Write($"{n,8} |");
        foreach (var c in divisors) Console.Write($" {Time(source, Math.Max(2, n / c)),10:N1}");
        var sqrtK = Math.Max(2, Math.Min(512, (int)Math.Sqrt(n)));
        Console.WriteLine($" | {Time(source, sqrtK),10:N1}");
    }
    Console.WriteLine();
}

static int[] Gen(string pattern, int n) => pattern switch
{
    "Sorted" => ArrayPatterns.GenerateSorted(n),
    "Random" => ArrayPatterns.GenerateRandom(n, new Random(42)),
    "ManyDuplicates" => ArrayPatterns.GenerateManyDuplicates(n, new Random(42)),
    // Range >> n: the regime bucket sort exists for, where counting/pigeonhole refuse the input.
    // Buckets span many values each, so k = n is not the degenerate one-value-per-bucket case.
    "SparseRandom" => SparseRandom(n, 1000, new Random(42)),
    // Same wide range, but every value falls inside one narrow band: the adversarial case where
    // the distribution assumption fails and most buckets stay empty.
    "Clustered" => SparseRandom(n, 1, new Random(42)).Concat([0, n * 1000]).ToArray(),
    _ => ArrayPatterns.GenerateReversed(n),
};

static int[] SparseRandom(int n, int spread, Random random)
    => Enumerable.Range(0, n).Select(_ => random.Next(0, Math.Max(2, n * spread))).ToArray();

static double Time(int[] source, int bucketCount)
{
    var n = source.Length;
    var work = new int[n];
    var temp = new int[n];
    var indices = new int[n];
    var counts = new int[bucketCount];
    var positions = new int[bucketCount];

    var best = double.MaxValue;
    for (var run = 0; run < 7; run++)
    {
        Array.Copy(source, work, n);
        Array.Clear(counts);
        var sw = Stopwatch.StartNew();
        Run(work, temp, indices, counts, positions, bucketCount);
        sw.Stop();
        best = Math.Min(best, sw.Elapsed.TotalMicroseconds);
        if (run == 0 && !work.SequenceEqual(source.OrderBy(x => x))) throw new InvalidOperationException("not sorted");
    }
    return best;
}

static void Run(int[] a, int[] temp, int[] indices, int[] counts, int[] positions, int bucketCount)
{
    var n = a.Length;
    int min = a[0], max = a[0];
    for (var i = 1; i < n; i++) { if (a[i] < min) min = a[i]; if (a[i] > max) max = a[i]; }
    if (min == max) return;

    long range = (long)max - min + 1;
    if (range < bucketCount) bucketCount = (int)range;
    var bucketSize = Math.Max(1, (range + bucketCount - 1) / bucketCount);

    for (var i = 0; i < n; i++)
    {
        var b = (int)((a[i] - (long)min) / bucketSize);
        if (b >= bucketCount) b = bucketCount - 1;
        indices[i] = b;
        counts[b]++;
    }

    var offset = 0;
    for (var i = 0; i < bucketCount; i++) { positions[i] = offset; offset += counts[i]; }
    for (var i = 0; i < n; i++) temp[positions[indices[i]]++] = a[i];

    for (var i = 0; i < bucketCount; i++)
    {
        var count = counts[i];
        if (count <= 1) continue;
        var start = positions[i] - count;
        for (var j = start + 1; j < start + count; j++)
        {
            var key = temp[j];
            var m = j - 1;
            while (m >= start && temp[m] > key) { temp[m + 1] = temp[m]; m--; }
            temp[m + 1] = key;
        }
    }

    Array.Copy(temp, a, n);
}
