#:project ../../src/SortAlgorithm

// Large-scale verification for BlockMergeSort.
// Fuzz tests cap at n=4096, but buffer2 / MergeInternal / MergeInPlace / rotate-fallback
// paths only activate when blockSize = sqrt(iterator.Length()) exceeds CacheSize (512),
// i.e. merge levels >= 513^2 = 263169 elements (arrays of ~526k+).
// This harness verifies correctness AND stability against LINQ OrderBy (stable).

using System.Diagnostics;
using SortAlgorithm.Algorithms;

var failures = 0;
var sw = new Stopwatch();

void Verify(string name, int[] keys)
{
    var n = keys.Length;
    var items = new KeyIdx[n];
    for (var i = 0; i < n; i++) items[i] = new KeyIdx(keys[i], i);

    var expected = items.OrderBy(x => x.Key).ToArray(); // stable reference

    sw.Restart();
    BlockMergeSort.Sort(items.AsSpan());
    sw.Stop();

    for (var i = 0; i < n; i++)
    {
        if (items[i].Key != expected[i].Key || items[i].Idx != expected[i].Idx)
        {
            failures++;
            var kind = items[i].Key != expected[i].Key ? "ORDER" : "STABILITY";
            Console.WriteLine($"FAIL [{kind}] {name}: at {i} got (key={items[i].Key}, idx={items[i].Idx}) expected (key={expected[i].Key}, idx={expected[i].Idx})");
            return;
        }
    }
    Console.WriteLine($"OK   {name} (n={n}, {sw.ElapsedMilliseconds}ms)");
}

// ---- large sizes: activate buffer2 / MergeInternal / MergeInPlace paths ----
foreach (var seed in new[] { 1, 42, 20260717 })
{
    var rng = new Random(seed);
    Verify($"random-full n=1048576 seed={seed}", Gen(1048576, _ => rng.Next()));
    Verify($"random-full n=1000003 seed={seed}", Gen(1000003, _ => rng.Next()));
    Verify($"distinct-1000 n=1048576 seed={seed}", Gen(1048576, _ => rng.Next(1000)));
    Verify($"distinct-300 n=1048576 seed={seed}", Gen(1048576, _ => rng.Next(300)));   // buffer1 partial -> MergeInPlace + rotate fallback
    Verify($"distinct-50 n=1048576 seed={seed}", Gen(1048576, _ => rng.Next(50)));
    Verify($"distinct-2 n=1048576 seed={seed}", Gen(1048576, _ => rng.Next(2)));
}
Verify("all-same n=1048576", Gen(1048576, _ => 42));
Verify("reversed n=1048576", Gen(1048576, i => 1048576 - i));
Verify("sawtooth-10000 n=786433", Gen(786433, i => i % 10000));
Verify("organ-pipe n=600000", Gen(600000, i => i < 300000 ? i : 600000 - i));
{
    var rng = new Random(7);
    var nearly = Gen(1048576, i => i);
    for (var i = 0; i < 1048576 / 20; i++)
    {
        var x = rng.Next(1048576);
        var y = rng.Next(1048576);
        (nearly[x], nearly[y]) = (nearly[y], nearly[x]);
    }
    Verify("nearly-sorted n=1048576", nearly);
}

// ---- medium sweep: stability with duplicates across level boundaries ----
foreach (var size in new[] { 520, 1025, 3000, 9999, 65536, 100003, 262145, 526339 })
{
    foreach (var seed in new[] { 11, 12, 13 })
    {
        var rng = new Random(seed);
        Verify($"random-full n={size} seed={seed}", Gen(size, _ => rng.Next()));
        Verify($"distinct-32 n={size} seed={seed}", Gen(size, _ => rng.Next(32)));
        Verify($"distinct-2 n={size} seed={seed}", Gen(size, _ => rng.Next(2)));
    }
}

Console.WriteLine(failures == 0 ? "ALL PASSED" : $"{failures} FAILURES");
return failures == 0 ? 0 : 1;

static int[] Gen(int n, Func<int, int> f)
{
    var a = new int[n];
    for (var i = 0; i < n; i++) a[i] = f(i);
    return a;
}

readonly struct KeyIdx(int key, int idx) : IComparable<KeyIdx>
{
    public readonly int Key = key;
    public readonly int Idx = idx;
    public int CompareTo(KeyIdx other) => Key.CompareTo(other.Key); // key only: stability observable via Idx
}
