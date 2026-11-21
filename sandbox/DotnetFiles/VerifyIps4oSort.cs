#:project ../../src/SortAlgorithm

// Measures Ips4oSort (IS⁴o):
//   1. operation counts on random input, per n and per n log2 n, to fit the Performance Characteristics rows,
//   2. the same on few distinct keys, where equality buckets end the recursion,
//   3. the McIlroy killer adversary built against Ips4oSort itself (fixed default seed), to see how far the worst case grows,
//   4. that killer input replayed on other seeds.
//
// Run from the repository root: dotnet run -c Release sandbox/DotnetFiles/VerifyIps4oSort.cs

using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using SortAlgorithm.Utils;

Console.WriteLine("--- random distinct keys: counts / (n log2 n), and swaps / n ---");
Console.WriteLine($"{"n",9} {"cmp",8} {"read",8} {"write",8} {"swap/n",8}");
foreach (var n in new[] { 1_000, 10_000, 100_000, 1_000_000, 4_000_000 })
{
    var a = Shuffled(n, 7);
    var st = new StatisticsContext();
    Ips4oSort.Sort(a.AsSpan(), st);
    Check(a);
    var nl = n * Math.Log2(n);
    Console.WriteLine($"{n,9} {st.CompareCount / nl,8:F3} {st.IndexReadCount / nl,8:F3} {st.IndexWriteCount / nl,8:F3} {st.SwapCount / (double)n,8:F4}");
}

Console.WriteLine();
Console.WriteLine("--- 16 distinct keys: counts / n ---");
Console.WriteLine($"{"n",9} {"cmp",8} {"read",8} {"write",8} {"swap",8}");
foreach (var n in new[] { 10_000, 100_000, 1_000_000, 4_000_000 })
{
    var random = new Random(3);
    var a = Enumerable.Range(0, n).Select(_ => random.Next(16)).ToArray();
    var st = new StatisticsContext();
    Ips4oSort.Sort(a.AsSpan(), st);
    Check(a);
    Console.WriteLine($"{n,9} {st.CompareCount / (double)n,8:F3} {st.IndexReadCount / (double)n,8:F3} {st.IndexWriteCount / (double)n,8:F3} {st.SwapCount / (double)n,8:F4}");
}

// McIlroy's adversary freezes items in the order the initial check scans them, which makes the input sorted and ends the sort after
// n comparisons. Two comparisons made before the sort freeze a[n-1] below a[1] below the still undecided a[0]: the check then sees a
// last element less than the first and an increase at once, and the adversary gets the partitioning steps.
static void Killable(Span<int> span, IComparer<int> comparer)
{
    comparer.Compare(span[1], span[^1]);
    comparer.Compare(span[1], span[0]);
    Ips4oSort.Sort(span, comparer, NullContext.Default);
}

Console.WriteLine();
Console.WriteLine("--- killer adversary built against Ips4oSort (default seed) ---");
Console.WriteLine($"{"n",8} {"cmp/(n log2 n)",15} {"cmp/(n^2/log2 n)",17} {"cmp/n^2",10} {"writes/n^2",11} {"partition steps",16}");
foreach (var n in new[] { 1_000, 4_000, 16_000, 64_000, 256_000 })
{
    var killer = KillerAdversaryGenerator.Generate(n, Killable);
    var steps = 0;
    var phases = new VisualizationContext(onPhase: (phase, _, _, _) => { if (phase == SortPhase.Ips4oClassification) steps++; });
    Ips4oSort.Sort(killer.ToArray().AsSpan(), phases);
    var st = new StatisticsContext();
    var a = killer.ToArray();
    Ips4oSort.Sort(a.AsSpan(), st);
    Check(a);
    var c = (double)st.CompareCount;
    var l = Math.Log2(n);
    Console.WriteLine($"{n,8} {c / (n * l),15:F3} {c / ((double)n * n / l),17:F4} {c / ((double)n * n),10:F5} {st.IndexWriteCount / ((double)n * n),11:F5} {steps,16}");
}

Console.WriteLine();
Console.WriteLine("--- killer against Ips4oSort replayed on other seeds (cmp / (n log2 n)), n = 64,000 ---");
{
    var killer = KillerAdversaryGenerator.Generate(64_000, Killable);
    foreach (var seed in new ulong[] { 1, 2, 12345 })
    {
        var a = killer.ToArray();
        var st = new StatisticsContext();
        Ips4oSort.Sort(a.AsSpan(), Comparer<int>.Default, st, seed);
        Check(a);
        Console.WriteLine($"seed {seed,6}: {st.CompareCount / (64_000 * Math.Log2(64_000)),8:F3}");
    }
}

static int[] Shuffled(int n, int seed)
{
    var a = Enumerable.Range(0, n).ToArray();
    new Random(seed).Shuffle(a);
    return a;
}

static void Check(int[] a)
{
    for (var i = 1; i < a.Length; i++)
        if (a[i] < a[i - 1]) throw new InvalidOperationException($"not sorted at {i}");
}
