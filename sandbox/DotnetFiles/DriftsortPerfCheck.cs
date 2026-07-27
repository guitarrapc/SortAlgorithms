#:project ../../src/SortAlgorithm

using System.Diagnostics;
using SortAlgorithm.Algorithms;

// Quick Release-mode sanity check: Driftsort vs Glidesort vs Array.Sort on common patterns.
// Not a benchmark (use SortAlgorithm.Benchmark for that) - just verifies the port is in a
// plausible performance range and produces correct output on large inputs.

const int N = 1_000_000;
const int Iterations = 5;

var rng = new Random(42);
var patterns = new Dictionary<string, int[]>
{
    ["Random"] = Enumerable.Range(0, N).Select(_ => rng.Next()).ToArray(),
    ["Sorted"] = Enumerable.Range(0, N).ToArray(),
    ["Reversed"] = Enumerable.Range(0, N).Reverse().ToArray(),
    ["LowCardinality"] = Enumerable.Range(0, N).Select(_ => rng.Next(16)).ToArray(),
    ["PipeOrgan"] = Enumerable.Range(0, N).Select(i => i < N / 2 ? i : N - i).ToArray(),
};

foreach (var (name, source) in patterns)
{
    var expected = source.ToArray();
    Array.Sort(expected);

    Measure($"{name,-15} Driftsort ", source, expected, s => Driftsort.Sort(s));
    Measure($"{name,-15} Glidesort ", source, expected, s => Glidesort.Sort(s));
    Measure($"{name,-15} Array.Sort", source, expected, s => s.Sort());
}

static void Measure(string label, int[] source, int[] expected, SpanAction sort)
{
    var buffer = new int[source.Length];
    // Warmup + correctness
    source.CopyTo(buffer, 0);
    sort(buffer.AsSpan());
    if (!buffer.AsSpan().SequenceEqual(expected)) throw new Exception($"{label}: INCORRECT RESULT");

    var best = double.MaxValue;
    for (var i = 0; i < Iterations; i++)
    {
        source.CopyTo(buffer, 0);
        var sw = Stopwatch.StartNew();
        sort(buffer.AsSpan());
        sw.Stop();
        best = Math.Min(best, sw.Elapsed.TotalMilliseconds);
    }
    Console.WriteLine($"{label}: {best,8:F2} ms (best of {Iterations})");
}

delegate void SpanAction(Span<int> span);
