#:project ../../src/SortAlgorithm

// Verifies that GenerateTimsortAdversary costs TimSort more than random input, across sizes
// that straddle minRun, run counts that are and are not powers of two, and the non-multiple case.
//
// TimSort has no quadratic case, so the number to watch is the ratio against random - not a
// growing c/(n log n). The pattern this replaced measured ~0.7x, i.e. easier than random.

using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using SortAlgorithm.Utils;

Console.WriteLine("--- TimSort: adversary vs random ---");
Console.WriteLine($"{"n",8} {"minRun",7} {"runs",6} {"compares",12} {"writes",12} {"total",13} {"vs random",10}");

foreach (var n in new[] { 64, 128, 256, 1024, 4096, 5000, 8192, 16384, 32768, 50000, 65536, 100000 })
{
    var adversary = ArrayPatterns.GenerateTimsortAdversary(n);
    var stats = Run(adversary);
    var baseline = Total(Shuffled(n));
    var total = stats.CompareCount + stats.SwapCount + stats.IndexReadCount + stats.IndexWriteCount;

    var minRun = MinRun(n);
    Console.WriteLine($"{n,8} {minRun,7} {(n + minRun - 1) / minRun,6} {stats.CompareCount,12:N0} {stats.IndexWriteCount,12:N0} {total,13:N0} {(double)total / baseline,9:F2}x");
}

Console.WriteLine();
Console.WriteLine("--- the construction is a permutation and every run is exactly minRun ---");
foreach (var n in new[] { 1, 2, 3, 33, 64, 65, 1000, 4096 })
{
    var a = ArrayPatterns.GenerateTimsortAdversary(n);
    var sorted = (int[])a.Clone();
    Array.Sort(sorted);
    var isPermutation = sorted.SequenceEqual(Enumerable.Range(0, n));
    Console.WriteLine($"n={n,6} length={a.Length,6} permutation={isPermutation,5} longestNaturalRun={LongestRun(a),4} (minRun={MinRun(n)})");
}

Console.WriteLine();
Console.WriteLine("--- what it costs the other run-adaptive merge sorts (total ops, n = 32768) ---");
Console.WriteLine($"{"algorithm",-20} {"random",13} {"timSortAdv",13} {"ratio",8}");
Other("timsort", (s, c) => TimSort.Sort(s, c));
Other("powersort", (s, c) => PowerSort.Sort(s, c));
Other("shiftsort", (s, c) => ShiftSort.Sort(s, c));
Other("naturalmergesort", (s, c) => NaturalMergeSort.Sort(s, c));
Other("mergesort", (s, c) => MergeSort.Sort(s, c));
Other("glidesort", (s, c) => Glidesort.Sort(s, c));
Other("driftsort", (s, c) => Driftsort.Sort(s, c));
Other("flatstablesort", (s, c) => FlatStableSort.Sort(s, c));
Other("blocksortwikisort", (s, c) => BlockMergeSort.Sort(s, c));
Other("bubblesort", (s, c) => BubbleSort.Sort(s, c));
Other("pdqsort", (s, c) => PDQSort.Sort(s, c));

static void Other(string label, Action<Span<int>, StatisticsContext> sort)
{
    const int N = 32768;
    var random = TotalWith(Shuffled(N), sort);
    var adversary = TotalWith(ArrayPatterns.GenerateTimsortAdversary(N), sort);
    Console.WriteLine($"{label,-20} {random,13:N0} {adversary,13:N0} {(double)adversary / random,7:F2}x");
}

/// <summary>Length of the longest ascending or strictly descending prefix run, as TimSort detects it.</summary>
static int LongestRun(int[] a)
{
    if (a.Length < 2) return a.Length;

    var best = 1;
    var i = 0;
    while (i < a.Length - 1)
    {
        var j = i + 1;
        if (a[j] < a[i]) while (j < a.Length - 1 && a[j + 1] < a[j]) j++;
        else while (j < a.Length - 1 && a[j + 1] >= a[j]) j++;

        best = Math.Max(best, j - i + 1);
        i = j + 1;
    }
    return best;
}

static ulong Total(int[] data) => TotalWith(data, (s, c) => TimSort.Sort(s, c));

static ulong TotalWith(int[] data, Action<Span<int>, StatisticsContext> sort)
{
    var work = (int[])data.Clone();
    var stats = new StatisticsContext();
    sort(work.AsSpan(), stats);
    return stats.CompareCount + stats.SwapCount + stats.IndexReadCount + stats.IndexWriteCount;
}

static StatisticsContext Run(int[] data)
{
    var work = (int[])data.Clone();
    var stats = new StatisticsContext();
    TimSort.Sort(work.AsSpan(), stats);

    for (var i = 1; i < work.Length; i++)
    {
        if (work[i - 1] > work[i]) throw new InvalidOperationException("not sorted");
    }
    return stats;
}

/// <summary>Mirrors TimSort.ComputeMinRun, which is internal to the library.</summary>
static int MinRun(int n)
{
    var r = 0;
    while (n >= 64)
    {
        r |= n & 1;
        n >>= 1;
    }
    return n + r;
}

static int[] Shuffled(int size)
{
    var random = new Random(20260802);
    var array = Enumerable.Range(0, size).ToArray();
    for (var i = array.Length - 1; i > 0; i--)
    {
        var j = random.Next(i + 1);
        (array[i], array[j]) = (array[j], array[i]);
    }
    return array;
}
