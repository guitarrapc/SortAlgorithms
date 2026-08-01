#:project ../../src/SortAlgorithm

using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

// Check two claims made in the AmericanFlagSort class summary:
//   "Comparisons : 0 (Non-comparison sort, uses bitwise operations only)"
//   "Best case   : Theta(n) - When all elements fall into one bucket early"
const int N = 100_000;

void Report(string label, Func<Random, int, int> gen)
{
    var rnd = new Random(42);
    var a = new int[N];
    for (var i = 0; i < N; i++) a[i] = gen(rnd, i);

    var stats = new StatisticsContext();
    AmericanFlagSort.Sort(a.AsSpan(), stats);

    var sorted = true;
    for (var i = 1; i < N; i++) if (a[i - 1] > a[i]) { sorted = false; break; }

    Console.WriteLine($"{label,-22} sorted={sorted}  reads={stats.IndexReadCount,10} ({(double)stats.IndexReadCount / N,6:F2} n)  compares={stats.CompareCount,10}  swaps={stats.SwapCount,9}");
}

Report("all equal (42)", (_, _) => 42);
Report("0..999", (r, _) => r.Next(0, 1000));
Report("full int range", (r, _) => r.Next(int.MinValue, int.MaxValue));
Report("already sorted 0..n", (_, i) => i);
