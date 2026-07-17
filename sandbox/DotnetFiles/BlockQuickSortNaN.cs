#:project ../../src/SortAlgorithm

using SortAlgorithm.Algorithms;

// Verify BlockQuickSort with NaN under the NullContext fast path (Release).
// IntroSort (which has a NaN pre-pass) is the control.

var failures = 0;

foreach (var seed in Enumerable.Range(0, 20))
{
    var rng = new Random(seed);
    foreach (var n in new[] { 10, 30, 100, 500, 3000, 25000 })
    {
        var input = new double[n];
        for (var i = 0; i < n; i++)
        {
            var roll = rng.Next(10);
            input[i] = roll == 0 ? double.NaN : rng.NextDouble() * 1000 - 500;
        }

        var expected = input.ToArray();
        Array.Sort(expected);

        var block = input.ToArray();
        BlockQuickSort.Sort(block.AsSpan());

        var intro = input.ToArray();
        IntroSort.Sort(intro.AsSpan());

        if (!SequenceEqualNaN(block, expected))
        {
            failures++;
            Console.WriteLine($"FAIL BlockQuickSort NaN seed={seed} n={n}");
        }
        if (!SequenceEqualNaN(intro, expected))
        {
            failures++;
            Console.WriteLine($"FAIL IntroSort NaN seed={seed} n={n} (control!)");
        }
    }
}

Console.WriteLine(failures == 0 ? "all passed" : $"failures={failures}");
return failures == 0 ? 0 : 1;

static bool SequenceEqualNaN(double[] a, double[] b)
{
    if (a.Length != b.Length) return false;
    for (var i = 0; i < a.Length; i++)
    {
        if (double.IsNaN(a[i]) != double.IsNaN(b[i])) return false;
        if (!double.IsNaN(a[i]) && a[i] != b[i]) return false;
    }
    return true;
}
