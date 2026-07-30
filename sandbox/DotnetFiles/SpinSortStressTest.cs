#:project ../../src/SortAlgorithm

using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

// Randomized correctness + stability stress for SpinSort, covering the input shapes that reach the
// merge MIN_CHECK short-circuits and the upper_bound based insert_partial_sort.
static int[] Build(string pattern, int n, Random rng) => pattern switch
{
    "random" => Enumerable.Range(0, n).Select(_ => rng.Next()).ToArray(),
    "fewUnique" => Enumerable.Range(0, n).Select(_ => rng.Next(0, 4)).ToArray(),
    "allEqual" => Enumerable.Repeat(7, n).ToArray(),
    "sorted" => Enumerable.Range(0, n).ToArray(),
    "reversed" => Enumerable.Range(0, n).Reverse().ToArray(),
    "ascBlocks" => Enumerable.Range(0, n).Select(i => (i / 2048) * 2048 + rng.Next(2048)).ToArray(),
    "descBlocks" => Enumerable.Range(0, n).Select(i => (Math.Max(1, n / 2048) - 1 - i / 2048) * 2048 + rng.Next(2048)).ToArray(),
    "sortedLowTail" => Enumerable.Range(0, n).Select(i => i < n - Math.Min(n, 32) ? i : rng.Next(0, Math.Max(1, n / 4))).ToArray(),
    "sortedHighTail" => Enumerable.Range(0, n).Select(i => i < n - Math.Min(n, 32) ? i : n - rng.Next(0, 32)).ToArray(),
    "reversedTail" => Enumerable.Range(0, n).Select(i => i < n - Math.Min(n, 32) ? n - i : rng.Next()).ToArray(),
    "organ" => Enumerable.Range(0, n).Select(i => i < n / 2 ? i : n - i).ToArray(),
    "sawtooth" => Enumerable.Range(0, n).Select(i => i % 128).ToArray(),
    _ => throw new ArgumentOutOfRangeException(nameof(pattern)),
};

string[] patterns =
[
    "random", "fewUnique", "allEqual", "sorted", "reversed", "ascBlocks", "descBlocks",
    "sortedLowTail", "sortedHighTail", "reversedTail", "organ", "sawtooth",
];

int[] sizes =
[
    0, 1, 2, 3, 31, 32, 33, 35, 36, 37, 71, 72, 73, 100, 255, 256, 257, 1023, 1024, 1025,
    2047, 2048, 2049, 4095, 4096, 5000, 8192, 20000, 65536, 100000, 131072,
];

var failures = 0;
var comparer = new KeyComparer();

foreach (var n in sizes)
{
    foreach (var pattern in patterns)
    {
        for (var seed = 0; seed < 3; seed++)
        {
            var rng = new Random(seed * 7919 + n);
            var keys = Build(pattern, n, rng);

            // order + stability in one pass: sort (key, originalIndex) by key only
            var items = keys.Select((k, i) => new Item(k, i)).ToArray();
            SpinSort.Sort(items.AsSpan(), comparer, NullContext.Default);

            var expected = keys.Select((k, i) => new Item(k, i)).ToArray();
            Array.Sort(expected, (a, b) => a.Key != b.Key ? a.Key.CompareTo(b.Key) : a.Seq.CompareTo(b.Seq));

            if (!items.AsSpan().SequenceEqual(expected))
            {
                var at = 0;
                while (at < n && items[at] == expected[at]) at++;
                Console.WriteLine($"FAIL n={n} pattern={pattern} seed={seed} at={at} got={items[at]} want={expected[at]}");
                failures++;
            }
        }
    }
}

Console.WriteLine(failures == 0
    ? $"OK: {sizes.Length * patterns.Length * 3} cases passed (order + stability)"
    : $"{failures} failures");

record struct Item(int Key, int Seq);

sealed class KeyComparer : IComparer<Item>
{
    public int Compare(Item x, Item y) => x.Key.CompareTo(y.Key);
}
