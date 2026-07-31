#:project ../../src/SortAlgorithm

using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

// F(n) = sum_{k=1}^{n} ceil(log2(3k/4))  (Ford-Johnson worst-case comparison count, OEIS A001768)
static int F(int n)
{
    var total = 0;
    for (var k = 1; k <= n; k++)
    {
        var t = 0;
        // smallest t with 2^t >= 3k/4  <=>  4*2^t >= 3k
        while ((4L << t) < 3L * k) t++;
        total += t;
    }
    return total;
}

static ulong Compares(int[] a)
{
    var stats = new StatisticsContext();
    var copy = (int[])a.Clone();
    MergeInsertionSort.Sort(copy.AsSpan(), stats);
    for (var i = 1; i < copy.Length; i++)
        if (copy[i - 1] > copy[i]) throw new Exception($"NOT SORTED n={copy.Length}");
    return stats.CompareCount;
}

// exhaustive for n <= 8
static void Permute(int[] a, int k, Action<int[]> f)
{
    if (k == a.Length) { f(a); return; }
    for (var i = k; i < a.Length; i++)
    {
        (a[k], a[i]) = (a[i], a[k]);
        Permute(a, k + 1, f);
        (a[k], a[i]) = (a[i], a[k]);
    }
}

Console.WriteLine($"{"n",4} {"F(n)",6} {"maxObserved",12} {"delta",6}  {"ceil(lg n!)",12}");
for (var n = 2; n <= 9; n++)
{
    ulong max = 0;
    Permute(Enumerable.Range(0, n).ToArray(), 0, p => { var c = Compares(p); if (c > max) max = c; });
    var f = F(n);
    Console.WriteLine($"{n,4} {f,6} {max,12} {(long)max - f,6}  (exhaustive)");
}

var rng = new Random(12345);
for (var n = 10; n <= 200; n += (n < 40 ? 1 : 10))
{
    ulong max = 0;
    for (var t = 0; t < 3000; t++)
    {
        var p = Enumerable.Range(0, n).ToArray();
        for (var i = n - 1; i > 0; i--) { var j = rng.Next(i + 1); (p[i], p[j]) = (p[j], p[i]); }
        var c = Compares(p);
        if (c > max) max = c;
    }
    var f = F(n);
    Console.WriteLine($"{n,4} {f,6} {max,12} {(long)max - f,6}  (random x3000)");
}
