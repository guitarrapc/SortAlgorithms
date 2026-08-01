// Why is radix-256 slower than radix-16 on full-int-range keys at n=4096..8192?
// Hypothesis: level 0 splits n into 256 buckets averaging n/256 elements; with n=4096 that is ~16,
// so roughly half the buckets land just above the insertion cutoff and each pays the full 256-bucket
// fixed cost (clear 1028B + prefix 256 + init 256 + bucket walk 256) to split ~20 elements.
// If that is the cause, raising the cutoff so those nodes never do a 256-way split should remove it.
using System.Diagnostics;
using System.Numerics;

static void Insertion(Span<int> a, int first, int last)
{
    for (var i = first + 1; i < last; i++)
    {
        var t = a[i];
        var j = i - 1;
        while (j >= first && a[j] > t) { a[j + 1] = a[j]; j--; }
        a[j + 1] = t;
    }
}

static uint Key(int v) => (uint)v ^ 0x8000_0000u;

long nodes = 0;

void Afs(Span<int> a, uint minKey, int start, int len, int digit, int radixBits, int cutoff)
{
    var radixSize = 1 << radixBits;
    var mask = (uint)(radixSize - 1);
    if (len <= cutoff) { Insertion(a, start, start + len); return; }
    if (digit < 0) return;
    nodes++;
    var shift = digit * radixBits;
    Span<int> cnt = stackalloc int[257];
    Span<int> next = stackalloc int[256];
    cnt = cnt[..(radixSize + 1)];
    next = next[..radixSize];
    cnt.Clear();
    for (var i = 0; i < len; i++) cnt[(int)(((Key(a[start + i]) - minKey) >> shift) & mask) + 1]++;
    var ne = 0;
    for (var i = 0; i < radixSize; i++) if (cnt[i + 1] > 0 && ++ne > 1) break;
    if (ne <= 1) { if (digit > 0) Afs(a, minKey, start, len, digit - 1, radixBits, cutoff); return; }
    for (var i = 1; i <= radixSize; i++) cnt[i] += cnt[i - 1];
    for (var i = 0; i < radixSize; i++) next[i] = cnt[i];
    for (var b = 0; b < radixSize; b++)
    {
        var end = cnt[b + 1];
        while (next[b] < end)
        {
            var p = start + next[b];
            var d = (int)(((Key(a[p]) - minKey) >> shift) & mask);
            if (d == b) { next[b]++; continue; }
            var q = start + next[d];
            (a[p], a[q]) = (a[q], a[p]);
            next[d]++;
        }
    }
    for (var i = 0; i < radixSize; i++)
    {
        var bl = cnt[i + 1] - cnt[i];
        if (bl > 1) Afs(a, minKey, start + cnt[i], bl, digit - 1, radixBits, cutoff);
    }
}

void Entry(int[] a, int radixBits, int cutoff)
{
    if (a.Length <= 1) return;
    uint mn = uint.MaxValue, mx = 0;
    foreach (var v in a) { var k = Key(v); if (k < mn) mn = k; if (k > mx) mx = k; }
    var range = mx - mn;
    if (range == 0) return;
    var bits = 32 - BitOperations.LeadingZeroCount(range);
    Afs(a, mn, 0, a.Length, (bits + radixBits - 1) / radixBits - 1, radixBits, cutoff);
}

(double Ms, long Nodes) Bench(int[] src, int radixBits, int cutoff)
{
    var warm = src.ToArray(); Entry(warm, radixBits, cutoff);
    for (var i = 1; i < warm.Length; i++) if (warm[i - 1] > warm[i]) throw new Exception("not sorted");
    nodes = 0;
    var a0 = src.ToArray(); Entry(a0, radixBits, cutoff);
    var nodeCount = nodes;
    var best = double.MaxValue;
    for (var r = 0; r < 9; r++)
    {
        var a = src.ToArray();
        var sw = Stopwatch.StartNew();
        Entry(a, radixBits, cutoff);
        best = Math.Min(best, sw.Elapsed.TotalMilliseconds);
    }
    return (best, nodeCount);
}

void Suite(string label, Func<Random, int, int> gen, int n)
{
    var rnd = new Random(42);
    var src = new int[n];
    for (var i = 0; i < n; i++) src[i] = gen(rnd, i);

    var b16 = Bench(src, 4, 16);
    Console.Write($"{label,-12} n={n,8}  r16/c16={b16.Ms,9:F4} ms (nodes={b16.Nodes,7})");
    foreach (var c in new[] { 16, 32, 48, 64, 96, 128 })
    {
        var b = Bench(src, 8, c);
        Console.Write($"   r256/c{c,-3}={b.Ms,9:F4} ({b.Ms / b16.Ms,5:F2}x, n={b.Nodes})");
    }
    Console.WriteLine();
}

foreach (var n in new[] { 4096, 8192, 65536, 1_048_576 })
{
    Suite("wide", (r, _) => r.Next(int.MinValue, int.MaxValue), n);
    Suite("narrow 1..n", (_, i) => i + 1, n); // shuffled below
}

// narrow needs shuffling; redo properly
Console.WriteLine("\n-- narrow (shuffled 1..n) --");
foreach (var n in new[] { 4096, 8192, 65536, 1_048_576 })
{
    var rnd = new Random(42);
    var src = Enumerable.Range(1, n).OrderBy(_ => rnd.Next()).ToArray();
    var b16 = Bench(src, 4, 16);
    Console.Write($"{"narrow",-12} n={n,8}  r16/c16={b16.Ms,9:F4} ms (nodes={b16.Nodes,7})");
    foreach (var c in new[] { 16, 32, 48, 64, 96, 128 })
    {
        var b = Bench(src, 8, c);
        Console.Write($"   r256/c{c,-3}={b.Ms,9:F4} ({b.Ms / b16.Ms,5:F2}x, n={b.Nodes})");
    }
    Console.WriteLine();
}
