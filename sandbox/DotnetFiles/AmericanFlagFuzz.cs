#:project ../../src/SortAlgorithm

using SortAlgorithm.Algorithms;

// Debug-config fuzz: exercises the Debug.Assert bucket-boundary invariants in
// PermuteInPlace plus the SortSpan bounds-checked DEBUG element accessors.
var rnd = new Random(12345);
var shapes = new (string Name, Func<Random, int, int, int> Gen)[]
{
    ("uniform",      (r, _, _) => r.Next(int.MinValue, int.MaxValue)),
    ("narrow",       (r, _, _) => r.Next(0, 8)),
    ("one digit",    (r, _, _) => r.Next(0, 16)),
    ("all equal",    (_, _, _) => -7),
    ("sorted",       (_, i, _) => i),
    ("reversed",     (_, i, n) => n - i),
    ("sign split",   (r, _, _) => r.Next(0, 2) == 0 ? int.MinValue + r.Next(0, 100) : int.MaxValue - r.Next(0, 100)),
    ("power of two", (r, _, _) => 1 << r.Next(0, 31)),
    ("dup heavy",    (r, n, _) => r.Next(0, Math.Max(1, n / 20))),
};

var cases = 0;
foreach (var (name, gen) in shapes)
{
    for (var n = 0; n <= 400; n++)
    {
        for (var trial = 0; trial < 3; trial++)
        {
            var a = new int[n];
            for (var i = 0; i < n; i++) a[i] = gen(rnd, i, n);
            var expected = a.ToArray();
            Array.Sort(expected);

            AmericanFlagSort.Sort(a.AsSpan());

            for (var i = 0; i < n; i++)
            {
                if (a[i] != expected[i])
                    throw new Exception($"mismatch: shape={name} n={n} trial={trial} at {i}: {a[i]} != {expected[i]}");
            }
            cases++;
        }
    }
}

// long / double / float key widths
for (var n = 17; n <= 300; n += 7)
{
    var l = new long[n];
    var d = new double[n];
    var f = new float[n];
    for (var i = 0; i < n; i++)
    {
        l[i] = (long)rnd.Next(int.MinValue, int.MaxValue) * rnd.Next(1, 1000);
        d[i] = rnd.NextDouble() * (rnd.Next(0, 2) == 0 ? -1e12 : 1e12);
        f[i] = (float)(rnd.NextDouble() * (rnd.Next(0, 2) == 0 ? -1e6 : 1e6));
    }
    var el = l.ToArray(); Array.Sort(el);
    var ed = d.ToArray(); Array.Sort(ed);
    var ef = f.ToArray(); Array.Sort(ef);
    AmericanFlagSort.Sort(l.AsSpan());
    AmericanFlagSort.Sort(d.AsSpan());
    AmericanFlagSort.Sort(f.AsSpan());
    if (!l.SequenceEqual(el)) throw new Exception($"long mismatch n={n}");
    if (!d.SequenceEqual(ed)) throw new Exception($"double mismatch n={n}");
    if (!f.SequenceEqual(ef)) throw new Exception($"float mismatch n={n}");
    cases += 3;
}

Console.WriteLine($"OK: {cases} cases, no assert fired, no mismatch.");
