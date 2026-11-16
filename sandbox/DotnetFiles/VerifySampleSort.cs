#:project ../../src/SortAlgorithm

// Measures SampleSort (Frazer–McKellar) against the quicksort family:
//   1. comparisons on random input, as a ratio to log2(n!) (the information-theoretic bound),
//   2. the McIlroy killer adversary built against SampleSort itself (fixed default seed),
//      to see how far the worst case grows.
//
// Reported per n: comparisons / log2(n!). A ratio that tends to 1 means the bound is approached;
// a ratio that grows like n / log n means quadratic.

using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using SortAlgorithm.Utils;

var variants = new (string Name, Action<Span<int>, StatisticsContext> Sort)[]
{
    ("SampleSort       ", (s, c) => SampleSort.Sort(s, c)),
    ("QuickSort        ", (s, c) => QuickSort.Sort(s, c)),
    ("QuickSortMedian3 ", (s, c) => QuickSortMedian3.Sort(s, c)),
    ("QuickSortMedian9 ", (s, c) => QuickSortMedian9.Sort(s, c)),
};

Console.WriteLine("--- random input: comparisons / log2(n!), averaged over 5 seeds ---");
Console.WriteLine($"{"n",9} " + string.Join(" ", variants.Select(v => $"{v.Name.Trim(),17}")));
foreach (var n in new[] { 1_000, 10_000, 100_000, 1_000_000 })
{
    var line = $"{n,9} ";
    foreach (var (_, sort) in variants)
    {
        var total = 0.0;
        for (var seed = 0; seed < 5; seed++)
        {
            var a = Shuffled(n, seed);
            var st = new StatisticsContext();
            sort(a, st);
            Check(a);
            total += st.CompareCount;
        }
        line += $"{total / 5 / Log2Factorial(n),17:F4} ";
    }
    Console.WriteLine(line);
}

Console.WriteLine();
Console.WriteLine("--- random input n = 1,000,000: operation counts / n ---");
foreach (var (name, sort) in variants)
{
    var a = Shuffled(1_000_000, 7);
    var st = new StatisticsContext();
    sort(a, st);
    Check(a);
    var n = (double)a.Length;
    Console.WriteLine($"{name} cmp {st.CompareCount / n,7:F2}  swap {st.SwapCount / n,6:F2}  read {st.IndexReadCount / n,7:F2}  write {st.IndexWriteCount / n,7:F2}");
}

Console.WriteLine();
Console.WriteLine("--- killer adversary built against SampleSort (default seed) ---");
Console.WriteLine($"{"n",8} {"cmp/log2(n!)",13} {"cmp/(n log2^2 n)",17} {"cmp/(n log2^3 n)",17} {"quadratic ceiling",18}");
foreach (var n in new[] { 1_000, 4_000, 16_000, 64_000, 256_000 })
{
    var killer = KillerAdversaryGenerator.Generate(n, static (span, comparer) => SampleSort.Sort(span, comparer, NullContext.Default));
    var st = new StatisticsContext();
    var a = killer.ToArray();
    SampleSort.Sort(a.AsSpan(), st);
    Check(a);
    var c = (double)st.CompareCount;
    var l = Math.Log2(n);
    Console.WriteLine($"{n,8} {c / Log2Factorial(n),13:F3} {c / (n * l * l),17:F4} {c / (n * l * l * l),17:F5} {(double)n * (n - 1) / 2 / Log2Factorial(n),18:F1}");
}

Console.WriteLine();
Console.WriteLine("--- killer against SampleSort replayed on other seeds (cmp / log2(n!)), n = 64,000 ---");
{
    var killer = KillerAdversaryGenerator.Generate(64_000, static (span, comparer) => SampleSort.Sort(span, comparer, NullContext.Default));
    foreach (var seed in new uint[] { 1, 2, 12345 })
    {
        var a = killer.ToArray();
        var st = new StatisticsContext();
        SampleSort.Sort(a.AsSpan(), Comparer<int>.Default, st, seed);
        Check(a);
        Console.WriteLine($"seed {seed,6}: {st.CompareCount / Log2Factorial(64_000):F3}");
    }
}

static int[] Shuffled(int n, int seed)
{
    var a = Enumerable.Range(0, n).ToArray();
    new Random(seed).Shuffle(a);
    return a;
}

static double Log2Factorial(int n)
{
    var sum = 0.0;
    for (var i = 2; i <= n; i++) sum += Math.Log2(i);
    return sum;
}

static void Check(int[] a)
{
    for (var i = 1; i < a.Length; i++)
        if (a[i - 1] > a[i]) throw new InvalidOperationException($"not sorted at {i}");
}
