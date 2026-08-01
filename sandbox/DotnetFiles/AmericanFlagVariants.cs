// Standalone (no SortSpan) variants of American Flag Sort to isolate the cost of
// three design choices in the current implementation:
//   (a) RadixBits = 4 (16 buckets) vs the canonical byte-wise 8 (256 buckets)
//   (b) no top-level key-range scan (LSD256 has one; MSD/AFS do not)
//   (c) swap-per-step permutation vs the register-hold cycle from McIlroy et al.
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

// (a) current shape: radix 16, swap-per-step
static void Afs16(Span<int> a, int start, int len, int digit)
{
    if (len <= Cutoff) { Insertion(a, start, start + len); return; }
    if (digit < 0) return;
    var shift = digit * 4;
    Span<int> cnt = stackalloc int[17];
    Span<int> next = stackalloc int[16];
    cnt.Clear();
    for (var i = 0; i < len; i++) cnt[(int)((Key(a[start + i]) >> shift) & 0xF) + 1]++;
    var ne = 0;
    for (var i = 0; i < 16; i++) if (cnt[i + 1] > 0 && ++ne > 1) break;
    if (ne <= 1) { if (digit > 0) Afs16(a, start, len, digit - 1); return; }
    for (var i = 1; i <= 16; i++) cnt[i] += cnt[i - 1];
    for (var i = 0; i < 16; i++) next[i] = cnt[i];
    for (var b = 0; b < 16; b++)
    {
        var end = cnt[b + 1];
        while (next[b] < end)
        {
            var p = start + next[b];
            var d = (int)((Key(a[p]) >> shift) & 0xF);
            if (d == b) { next[b]++; continue; }
            var q = start + next[d];
            (a[p], a[q]) = (a[q], a[p]);
            next[d]++;
        }
    }
    for (var i = 0; i < 16; i++)
    {
        var bl = cnt[i + 1] - cnt[i];
        if (bl > 1) Afs16(a, start + cnt[i], bl, digit - 1);
    }
}

// (c) radix 16 + register-hold cycle permutation
static void Afs16Cycle(Span<int> a, int start, int len, int digit)
{
    if (len <= Cutoff) { Insertion(a, start, start + len); return; }
    if (digit < 0) return;
    var shift = digit * 4;
    Span<int> cnt = stackalloc int[17];
    Span<int> next = stackalloc int[16];
    cnt.Clear();
    for (var i = 0; i < len; i++) cnt[(int)((Key(a[start + i]) >> shift) & 0xF) + 1]++;
    var ne = 0;
    for (var i = 0; i < 16; i++) if (cnt[i + 1] > 0 && ++ne > 1) break;
    if (ne <= 1) { if (digit > 0) Afs16Cycle(a, start, len, digit - 1); return; }
    for (var i = 1; i <= 16; i++) cnt[i] += cnt[i - 1];
    for (var i = 0; i < 16; i++) next[i] = cnt[i];
    for (var b = 0; b < 16; b++)
    {
        var end = cnt[b + 1];
        while (next[b] < end)
        {
            var home = start + next[b];
            var tmp = a[home];
            int d;
            // Follow the cycle keeping the in-flight element in a register.
            while ((d = (int)((Key(tmp) >> shift) & 0xF)) != b)
            {
                var q = start + next[d]++;
                (tmp, a[q]) = (a[q], tmp);
            }
            a[home] = tmp;
            next[b]++;
        }
    }
    for (var i = 0; i < 16; i++)
    {
        var bl = cnt[i + 1] - cnt[i];
        if (bl > 1) Afs16Cycle(a, start + cnt[i], bl, digit - 1);
    }
}

// (b) radix 256 + register-hold cycle permutation (canonical American flag sort)
static void Afs256(Span<int> a, int start, int len, int digit)
{
    if (len <= Cutoff) { Insertion(a, start, start + len); return; }
    if (digit < 0) return;
    var shift = digit * 8;
    Span<int> cnt = stackalloc int[257];
    Span<int> next = stackalloc int[256];
    cnt.Clear();
    for (var i = 0; i < len; i++) cnt[(int)((Key(a[start + i]) >> shift) & 0xFF) + 1]++;
    var ne = 0;
    for (var i = 0; i < 256; i++) if (cnt[i + 1] > 0 && ++ne > 1) break;
    if (ne <= 1) { if (digit > 0) Afs256(a, start, len, digit - 1); return; }
    for (var i = 1; i <= 256; i++) cnt[i] += cnt[i - 1];
    for (var i = 0; i < 256; i++) next[i] = cnt[i];
    for (var b = 0; b < 256; b++)
    {
        var end = cnt[b + 1];
        while (next[b] < end)
        {
            var home = start + next[b];
            var tmp = a[home];
            int d;
            while ((d = (int)((Key(tmp) >> shift) & 0xFF)) != b)
            {
                var q = start + next[d]++;
                (tmp, a[q]) = (a[q], tmp);
            }
            a[home] = tmp;
            next[b]++;
        }
    }
    for (var i = 0; i < 256; i++)
    {
        var bl = cnt[i + 1] - cnt[i];
        if (bl > 1) Afs256(a, start + cnt[i], bl, digit - 1);
    }
}

// top-level entry with / without the LSD-style range scan
static int TopDigit(ReadOnlySpan<int> a, int radixBits, bool scan)
{
    var full = (32 + radixBits - 1) / radixBits - 1;
    if (!scan) return full;
    uint min = uint.MaxValue, max = 0;
    foreach (var v in a) { var k = Key(v); if (k < min) min = k; if (k > max) max = k; }
    var range = max ^ min;
    if (range == 0) return -1;
    var bits = 32 - BitOperations.LeadingZeroCount(range);
    return (bits + radixBits - 1) / radixBits - 1;
}

static double Bench(string name, int[] src, Action<int[]> sort)
{
    var warm = src.ToArray(); sort(warm);
    for (var i = 1; i < warm.Length; i++) if (warm[i - 1] > warm[i]) throw new Exception($"{name} not sorted");
    var best = double.MaxValue;
    for (var r = 0; r < 7; r++)
    {
        var a = src.ToArray();
        var sw = Stopwatch.StartNew();
        sort(a);
        sw.Stop();
        best = Math.Min(best, sw.Elapsed.TotalMilliseconds);
    }
    Console.WriteLine($"   {name,-34} {best,8:F3} ms");
    return best;
}

void Suite(string label, Func<Random, int> gen, int n)
{
    var rnd = new Random(42);
    var src = new int[n];
    for (var i = 0; i < n; i++) src[i] = gen(rnd);

    Console.WriteLine($"== {label} (n={n})");
    Bench("radix16 swap      (current)", src, a => Afs16(a, 0, a.Length, TopDigit(a, 4, false)));
    Bench("radix16 cycle", src, a => Afs16Cycle(a, 0, a.Length, TopDigit(a, 4, false)));
    Bench("radix16 cycle + range scan", src, a => Afs16Cycle(a, 0, a.Length, TopDigit(a, 4, true)));
    Bench("radix256 cycle", src, a => Afs256(a, 0, a.Length, TopDigit(a, 8, false)));
    Bench("radix256 cycle + range scan", src, a => Afs256(a, 0, a.Length, TopDigit(a, 8, true)));
    Bench("Array.Sort (baseline)", src, Array.Sort);
    Console.WriteLine();
}

foreach (var n in new[] { 4096, 100_000, 1_000_000 })
{
    Suite("full int range", r => r.Next(int.MinValue, int.MaxValue), n);
    Suite("0..999", r => r.Next(0, 1000), n);
}
