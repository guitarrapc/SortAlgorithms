// Same-run comparison of the three top-level digit-count strategies for American Flag Sort,
// on standalone ports so all three run in one process (this machine drifts between runs).
//   none      : digitCount from the key width  (the behaviour before the range scan)
//   xor       : digitCount from bitlength(max ^ min)
//   normalize : digitCount from bitlength(max - min), digits taken from (key - min)  (what was implemented)
using System.Diagnostics;
using System.Numerics;

const int Cutoff = 16;

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

static void Afs(Span<int> a, uint minKey, int start, int len, int digit)
{
    if (len <= Cutoff) { Insertion(a, start, start + len); return; }
    if (digit < 0) return;
    var shift = digit * 4;
    Span<int> cnt = stackalloc int[17];
    Span<int> next = stackalloc int[16];
    cnt.Clear();
    for (var i = 0; i < len; i++) cnt[(int)(((Key(a[start + i]) - minKey) >> shift) & 0xF) + 1]++;
    var ne = 0;
    for (var i = 0; i < 16; i++) if (cnt[i + 1] > 0 && ++ne > 1) break;
    if (ne <= 1) { if (digit > 0) Afs(a, minKey, start, len, digit - 1); return; }
    for (var i = 1; i <= 16; i++) cnt[i] += cnt[i - 1];
    for (var i = 0; i < 16; i++) next[i] = cnt[i];
    for (var b = 0; b < 16; b++)
    {
        var end = cnt[b + 1];
        while (next[b] < end)
        {
            var p = start + next[b];
            var d = (int)(((Key(a[p]) - minKey) >> shift) & 0xF);
            if (d == b) { next[b]++; continue; }
            var q = start + next[d];
            (a[p], a[q]) = (a[q], a[p]);
            next[d]++;
        }
    }
    for (var i = 0; i < 16; i++)
    {
        var bl = cnt[i + 1] - cnt[i];
        if (bl > 1) Afs(a, minKey, start + cnt[i], bl, digit - 1);
    }
}

static void Entry(int[] a, string strategy)
{
    if (a.Length <= 1) return;
    switch (strategy)
    {
        case "none":
            Afs(a, 0u, 0, a.Length, 7); // 32 bits / 4 = 8 levels, always
            return;
        case "xor":
        {
            uint mn = uint.MaxValue, mx = 0;
            foreach (var v in a) { var k = Key(v); if (k < mn) mn = k; if (k > mx) mx = k; }
            var range = mx ^ mn;
            if (range == 0) return;
            var bits = 32 - BitOperations.LeadingZeroCount(range);
            Afs(a, 0u, 0, a.Length, (bits + 3) / 4 - 1);
            return;
        }
        default:
        {
            uint mn = uint.MaxValue, mx = 0;
            foreach (var v in a) { var k = Key(v); if (k < mn) mn = k; if (k > mx) mx = k; }
            var range = mx - mn;
            if (range == 0) return;
            var bits = 32 - BitOperations.LeadingZeroCount(range);
            Afs(a, mn, 0, a.Length, (bits + 3) / 4 - 1);
            return;
        }
    }
}

static double Bench(int[] src, string strategy)
{
    var warm = src.ToArray(); Entry(warm, strategy);
    for (var i = 1; i < warm.Length; i++) if (warm[i - 1] > warm[i]) throw new Exception($"{strategy} not sorted");
    var best = double.MaxValue;
    for (var r = 0; r < 7; r++)
    {
        var a = src.ToArray();
        var sw = Stopwatch.StartNew();
        Entry(a, strategy);
        best = Math.Min(best, sw.Elapsed.TotalMilliseconds);
    }
    return best;
}

void Suite(string label, Func<Random, int, int> gen, int n)
{
    var rnd = new Random(42);
    var src = new int[n];
    for (var i = 0; i < n; i++) src[i] = gen(rnd, i);

    var none = Bench(src, "none");
    var xor = Bench(src, "xor");
    var norm = Bench(src, "normalize");
    Console.WriteLine($"{label,-24} n={n,8}  none={none,8:F3}  xor={xor,8:F3} ({(xor / none - 1) * 100,6:+0.0;-0.0}%)  normalize={norm,8:F3} ({(norm / none - 1) * 100,6:+0.0;-0.0}%)");
}

foreach (var n in new[] { 100_000, 1_000_000 })
{
    Suite("full int range", (r, _) => r.Next(int.MinValue, int.MaxValue), n);
    Suite("0..999", (r, _) => r.Next(0, 1000), n);
    Suite("-500..500 (spans zero)", (r, _) => r.Next(-500, 501), n);
    Suite("sorted 0..n", (_, i) => i, n);
    Suite("all equal", (_, _) => 42, n);
    Console.WriteLine();
}
