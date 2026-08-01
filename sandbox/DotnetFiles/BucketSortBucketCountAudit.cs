#:project ../../src/SortAlgorithm

using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using SortAlgorithm.Utils;

// Comparison counts are exact and machine-independent, so they show the growth rate of the
// per-bucket insertion sort without any benchmark noise.
//
// If bucket sort achieves its textbook O(n) average case, comparisons must grow ~4x when n grows 4x.
// With k = sqrt(n) buckets each bucket holds sqrt(n) elements, so insertion sort inside it costs
// O(n) per bucket and O(n^1.5) overall - an 8x growth per 4x of n.

Console.WriteLine("=== current implementation: k = sqrt(n) buckets ===");
Console.WriteLine($"{"n",8} {"k",5} {"n/k",6} | {"Sorted",12} {"Random",12} {"Reversed",12} | Reversed/n");
foreach (var n in new[] { 1024, 4096, 16384, 65536 })
{
    var k = Math.Max(2, Math.Min(512, (int)Math.Sqrt(n)));
    Console.WriteLine($"{n,8} {k,5} {n / k,6} | {Cmp(ArrayPatterns.GenerateSorted(n)),12:N0} {Cmp(ArrayPatterns.GenerateRandom(n, new Random(42))),12:N0} {Cmp(ArrayPatterns.GenerateReversed(n)),12:N0} | {Cmp(ArrayPatterns.GenerateReversed(n)) / (double)n,10:N1}");
}

Console.WriteLine();
Console.WriteLine("=== growth per 4x of n (4.0 = O(n), 8.0 = O(n^1.5)) ===");
foreach (var pattern in new[] { "Sorted", "Random", "Reversed" })
{
    var counts = new[] { 1024, 4096, 16384, 65536 }.Select(n => (double)Cmp(Gen(pattern, n))).ToArray();
    Console.WriteLine($"{pattern,10}: " + string.Join("  ", counts.Zip(counts.Skip(1), (a, b) => $"{b / a,5:N2}x")));
}

// Sweep the bucket count at a fixed n to separate the heuristic from the algorithm.
// ManyDuplicates keeps buckets non-trivial even at k = n, so k = n is not a degenerate
// one-element-per-bucket case there (that degenerate case is pigeonhole sort, not bucket sort).
Console.WriteLine();
Console.WriteLine("=== same algorithm, sweeping the bucket count (n = 65536) ===");
Console.WriteLine($"{"k",8} {"n/k",8} | {"Random",14} {"Reversed",14} {"ManyDuplicates",16}");
const int N = 65536;
var random = ArrayPatterns.GenerateRandom(N, new Random(42));
var reversed = ArrayPatterns.GenerateReversed(N);
var duplicates = ArrayPatterns.GenerateManyDuplicates(N, new Random(42));
foreach (var k in new[] { 256, 1024, 4096, 16384, N })
{
    Console.WriteLine($"{k,8} {N / k,8} | {RefCmp(random, k),14:N0} {RefCmp(reversed, k),14:N0} {RefCmp(duplicates, k),16:N0}");
}

Console.WriteLine();
Console.WriteLine("=== growth per 4x of n when k scales with n (k = n/8) ===");
foreach (var pattern in new[] { "Sorted", "Random", "Reversed" })
{
    var counts = new[] { 1024, 4096, 16384, 65536 }.Select(n => (double)RefCmp(Gen(pattern, n), n / 8)).ToArray();
    Console.WriteLine($"{pattern,10}: " + string.Join("  ", counts.Zip(counts.Skip(1), (a, b) => $"{b / a,5:N2}x")) + $"   (absolute: {string.Join(", ", counts.Select(c => c.ToString("N0")))})");
}

static int[] Gen(string pattern, int n) => pattern switch
{
    "Sorted" => ArrayPatterns.GenerateSorted(n),
    "Random" => ArrayPatterns.GenerateRandom(n, new Random(42)),
    _ => ArrayPatterns.GenerateReversed(n),
};

static ulong Cmp(int[] source)
{
    var stats = new StatisticsContext();
    var array = source.ToArray();
    BucketSortInteger.Sort(array.AsSpan(), stats);
    if (!array.SequenceEqual(source.OrderBy(x => x))) throw new InvalidOperationException("not sorted");
    return stats.CompareCount;
}

// Reference bucket sort with a configurable bucket count: distribute by value range, insertion sort
// each bucket, concatenate. Same algorithm, only the bucket count differs.
static long RefCmp(int[] source, int bucketCount)
{
    var n = source.Length;
    var min = source.Min();
    var max = source.Max();
    long range = (long)max - min + 1;
    var bucketSize = Math.Max(1, (range + bucketCount - 1) / bucketCount);

    var buckets = new List<int>[bucketCount];
    for (var i = 0; i < bucketCount; i++) buckets[i] = [];
    foreach (var v in source)
    {
        var b = (int)((v - (long)min) / bucketSize);
        buckets[Math.Min(b, bucketCount - 1)].Add(v);
    }

    long compares = 0;
    var result = new List<int>(n);
    foreach (var bucket in buckets)
    {
        for (var i = 1; i < bucket.Count; i++)
        {
            var key = bucket[i];
            var j = i - 1;
            while (j >= 0) { compares++; if (bucket[j] <= key) break; bucket[j + 1] = bucket[j]; j--; }
            bucket[j + 1] = key;
        }
        result.AddRange(bucket);
    }
    if (!result.SequenceEqual(source.OrderBy(x => x))) throw new InvalidOperationException("reference not sorted");
    return compares;
}
