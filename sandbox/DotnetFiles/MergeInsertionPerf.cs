#:project ../../src/SortAlgorithm

using System.Diagnostics;
using SortAlgorithm.Algorithms;

// Same-run anchor (BinaryInsertionSort) so drift between runs can be normalized away.
static double Bench(Action<int[]> sort, int n, int rounds, int seed)
{
    var rng = new Random(seed);
    var src = Enumerable.Range(0, n).ToArray();
    for (var i = n - 1; i > 0; i--) { var j = rng.Next(i + 1); (src[i], src[j]) = (src[j], src[i]); }

    var work = new int[n];
    // warmup
    for (var t = 0; t < 3; t++) { src.CopyTo(work, 0); sort(work); }

    var best = double.MaxValue;
    for (var r = 0; r < 5; r++)
    {
        var sw = Stopwatch.StartNew();
        for (var t = 0; t < rounds; t++) { src.CopyTo(work, 0); sort(work); }
        sw.Stop();
        best = Math.Min(best, sw.Elapsed.TotalMilliseconds / rounds * 1000.0); // us/op
    }
    for (var i = 1; i < work.Length; i++) if (work[i - 1] > work[i]) throw new Exception("not sorted");
    return best;
}

Console.WriteLine($"{"n",6} {"MergeInsertion us",20} {"anchor(BinaryIns) us",22} {"ratio",8}");
foreach (var n in new[] { 256, 1024, 2048, 4096 })
{
    var rounds = Math.Max(20, 400_000 / n);
    var mi = Bench(a => MergeInsertionSort.Sort(a.AsSpan()), n, rounds, 99);
    var bi = Bench(a => BinaryInsertionSort.Sort(a.AsSpan()), n, rounds, 99);
    Console.WriteLine($"{n,6} {mi,20:F2} {bi,22:F2} {mi / bi,8:F2}");
}
