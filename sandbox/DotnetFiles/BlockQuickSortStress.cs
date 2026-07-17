#:project ../../src/SortAlgorithm

using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

// Differential stress test for BlockQuickSort vs Array.Sort.
// Covers pivot-strategy thresholds (100 / 800 / 20000), block boundaries (2*128),
// duplicate-heavy inputs, adversarial patterns, range sort, and custom comparer.

var failures = 0;
var cases = 0;

int[] sizes = [0, 1, 2, 3, 4, 5, 10, 20, 21, 50, 99, 100, 101, 127, 128, 129, 255, 256, 257, 258, 300, 511, 512, 513, 799, 800, 801, 1000, 2000, 5000, 10000, 19999, 20000, 20001, 25000, 40000, 65536];

foreach (var n in sizes)
{
    foreach (var (name, gen) in Patterns(n))
    {
        Check(name, n, gen);
    }
}

// Range sort: sort interior slice, verify outside untouched
{
    var rng = new Random(1234);
    for (var trial = 0; trial < 50; trial++)
    {
        var n = rng.Next(3, 3000);
        var arr = new int[n];
        for (var i = 0; i < n; i++) arr[i] = rng.Next(0, 100);
        var first = rng.Next(0, n);
        var last = rng.Next(first, n + 1);
        var expected = arr.ToArray();
        Array.Sort(expected, first, last - first);

        var actual = arr.ToArray();
        BlockQuickSort.Sort(actual.AsSpan(), first, last, new StatisticsContext());

        cases++;
        if (!actual.AsSpan().SequenceEqual(expected))
        {
            failures++;
            Console.WriteLine($"FAIL range-sort n={n} [{first},{last})");
        }
    }
}

// Custom comparer: descending
{
    var rng = new Random(99);
    foreach (var n in new[] { 0, 1, 50, 500, 5000, 30000 })
    {
        var arr = new int[n];
        for (var i = 0; i < n; i++) arr[i] = rng.Next(0, 1000);
        var expected = arr.ToArray();
        Array.Sort(expected, (a, b) => b.CompareTo(a));

        var actual = arr.ToArray();
        BlockQuickSort.Sort<int, Comparer<int>, NullContext>(actual.AsSpan(), Comparer<int>.Create((a, b) => b.CompareTo(a)), NullContext.Default);

        cases++;
        if (!actual.AsSpan().SequenceEqual(expected))
        {
            failures++;
            Console.WriteLine($"FAIL descending n={n}");
        }
    }
}

// Random fuzz across arbitrary sizes
{
    var rng = new Random(7);
    for (var trial = 0; trial < 300; trial++)
    {
        var n = rng.Next(0, 4000);
        var distinct = rng.Next(1, 4) switch { 1 => 2, 2 => 16, _ => int.MaxValue };
        var arr = new int[n];
        for (var i = 0; i < n; i++) arr[i] = distinct == int.MaxValue ? rng.Next() : rng.Next(0, distinct);
        Check($"fuzz(distinct={distinct})", n, () => arr.ToArray());
    }
}

Console.WriteLine($"cases={cases} failures={failures}");
return failures == 0 ? 0 : 1;

void Check(string name, int n, Func<int[]> gen)
{
    cases++;
    var input = gen();
    var expected = input.ToArray();
    Array.Sort(expected);

    // NullContext fast path
    var a1 = input.ToArray();
    BlockQuickSort.Sort(a1.AsSpan());
    if (!a1.AsSpan().SequenceEqual(expected))
    {
        failures++;
        Console.WriteLine($"FAIL {name} n={n} (NullContext)");
        return;
    }

    // StatisticsContext observed path
    var a2 = input.ToArray();
    BlockQuickSort.Sort(a2.AsSpan(), new StatisticsContext());
    if (!a2.AsSpan().SequenceEqual(expected))
    {
        failures++;
        Console.WriteLine($"FAIL {name} n={n} (StatisticsContext)");
    }
}

IEnumerable<(string, Func<int[]>)> Patterns(int n)
{
    yield return ("sorted", () => Enumerable.Range(0, n).ToArray());
    yield return ("reversed", () => Enumerable.Range(0, n).Reverse().ToArray());
    yield return ("all-equal", () => Enumerable.Repeat(42, n).ToArray());
    yield return ("two-values", () => { var r = new Random(n); return Enumerable.Range(0, n).Select(_ => r.Next(2)).ToArray(); });
    yield return ("few-unique", () => { var r = new Random(n + 1); return Enumerable.Range(0, n).Select(_ => r.Next(8)).ToArray(); });
    yield return ("random", () => { var r = new Random(n + 2); return Enumerable.Range(0, n).Select(_ => r.Next()).ToArray(); });
    yield return ("random-dup25", () => { var r = new Random(n + 3); return Enumerable.Range(0, n).Select(_ => r.Next(Math.Max(1, n / 4))).ToArray(); });
    yield return ("organ-pipe", () => Enumerable.Range(0, (n + 1) / 2).Concat(Enumerable.Range(0, n / 2).Reverse()).ToArray());
    yield return ("sawtooth", () => Enumerable.Range(0, n).Select(i => i % 32).ToArray());
    yield return ("mostly-sorted", () => { var a = Enumerable.Range(0, n).ToArray(); var r = new Random(n + 4); for (var i = 0; i < n / 20; i++) { var x = r.Next(n); var y = r.Next(n); (a[x], a[y]) = (a[y], a[x]); } return a; });
    yield return ("neg-pos", () => { var r = new Random(n + 5); return Enumerable.Range(0, n).Select(_ => r.Next(-1000, 1000)).ToArray(); });
}
