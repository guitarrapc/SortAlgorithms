#:project ../../src/SortAlgorithm

using SortAlgorithm.Algorithms;

// The digit passes order NaN correctly (its key is 0, below every non-NaN key). The insertion-sort
// cutoff does not use the key — it uses TComparer, which for the element overloads is
// ComparableComparer, and SortSpan specializes that to the raw IEEE operators for float/double, under
// which NaN is unordered. So a range that reaches the cutoff while still holding NaN alongside ordered
// values is sorted by a relation that cannot place NaN. This checks whether that actually happens.

static bool SortsLikeArraySort(int n, Action<double[]> sort, out string got)
{
    var rng = new Random(42);
    var a = new double[n];
    for (var i = 0; i < n; i++) a[i] = i % 3 == 0 ? double.NaN : rng.NextDouble() * 200.0 - 100.0;
    var expected = a.ToArray();
    Array.Sort(expected);

    sort(a);
    got = string.Concat(a.Take(Math.Min(n, 12)).Select(x => double.IsNaN(x) ? "N" : x < 0 ? "-" : "+"));
    var want = string.Concat(expected.Take(Math.Min(n, 12)).Select(x => double.IsNaN(x) ? "N" : x < 0 ? "-" : "+"));

    // Array.Sort puts every NaN first. Compare the full sequences bit-for-bit (NaN == NaN fails under ==,
    // so compare NaN-ness positionally and values elsewhere).
    for (var i = 0; i < n; i++)
    {
        if (double.IsNaN(expected[i]) != double.IsNaN(a[i])) return false;
        if (!double.IsNaN(expected[i]) && expected[i] != a[i]) return false;
    }
    return true;
}

var cases = new (string Name, Action<double[]> Sort)[]
{
    ("RadixMSD4Sort", a => RadixMSD4Sort.Sort(a.AsSpan())),
    ("RadixMSD10Sort", a => RadixMSD10Sort.Sort(a.AsSpan())),
    ("AmericanFlagSort", a => AmericanFlagSort.Sort(a.AsSpan())),
    ("RadixLSD4Sort", a => RadixLSD4Sort.Sort(a.AsSpan())),
    ("RadixLSD256Sort", a => RadixLSD256Sort.Sort(a.AsSpan())),
    ("SpreadSort", a => SpreadSort.Sort(a.AsSpan())),
};

// Sizes straddling every cutoff in the library: MSD 48, AmericanFlag 64, SpreadSort's min_sort_size 1000.
foreach (var n in new[] { 12, 40, 60, 200, 4096 })
{
    foreach (var (name, sort) in cases)
    {
        var ok = SortsLikeArraySort(n, sort, out var got);
        Console.WriteLine($"  n={n,5} {name,-18} matchesArraySort={ok,-6} first12={got}");
    }
    Console.WriteLine();
}
