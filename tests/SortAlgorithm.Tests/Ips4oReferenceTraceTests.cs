using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

/// <summary>
/// Ips4oSort is a port of the sequential ips4o, so its C++ source defines what it must do. Each row holds the comparison count and a hash
/// of the compared pairs, in order, that the C++ source produces for one element type and size across all shapes, with its generator
/// patched to start from the same seed and to map draws the same way. Each element carries its position below its key, so the hash
/// also tells equal keys apart. A change to which elements are compared, or in what order, changes the hash even when the output stays
/// sorted. The two element types have blocks of 256 and of 128 elements. sandbox/Ips4oReferenceTrace/trace.cpp prints the rows.
/// </summary>
public class Ips4oReferenceTraceTests
{
    const ulong FnvOffset = 14695981039346656037UL;
    const ulong FnvPrime = 0x100000001b3UL;
    const int KeyShift = 24;

    public enum Element { Int64, Pair }

    static readonly (int Shape, int P)[] Shapes = [(0, 0), (1, 1), (1, 16), (2, 0), (3, 0), (4, 0), (5, 50), (6, 200), (7, 0), (8, 1 << 20)];

    static readonly (Element Element, int Size, ulong Count, ulong Hash)[] Rows =
    [
        (Element.Int64, 2, 20UL, 0xDB0D727C47BA2955UL),
        (Element.Int64, 3, 38UL, 0x4F2A61358D43D3E0UL),
        (Element.Int64, 32, 1422UL, 0xB663245AAD10015CUL),
        (Element.Int64, 33, 1583UL, 0xB77DA7F137BCAC0CUL),
        (Element.Int64, 100, 12439UL, 0x370E7CAC8177C7C8UL),
        (Element.Int64, 256, 91553UL, 0x6DE88295CE961AB3UL),
        (Element.Int64, 257, 18464UL, 0x51C1AEF67BA2E37FUL),
        (Element.Int64, 300, 24611UL, 0x0370C3A2A10C3817UL),
        (Element.Int64, 1000, 110023UL, 0x4A3F9344061D9EF1UL),
        (Element.Int64, 3000, 310559UL, 0x0C0AE970DF7470CFUL),
        (Element.Int64, 4096, 445421UL, 0x3E1C4F136E960F6AUL),
        (Element.Int64, 4097, 430001UL, 0x1B491F2F0F8AF55DUL),
        (Element.Int64, 10007, 1092394UL, 0x1AE5AC31A78F7B97UL),
        (Element.Int64, 65536, 8039248UL, 0x3FA02488B1B4489FUL),
        (Element.Int64, 100000, 12537375UL, 0xA941940266CD9440UL),
        (Element.Int64, 1100000, 156459753UL, 0xD571CB5899C04D37UL),
        (Element.Pair, 2, 20UL, 0xDB0D727C47BA2955UL),
        (Element.Pair, 3, 38UL, 0x4F2A61358D43D3E0UL),
        (Element.Pair, 32, 1422UL, 0xB663245AAD10015CUL),
        (Element.Pair, 33, 1583UL, 0xB77DA7F137BCAC0CUL),
        (Element.Pair, 100, 12439UL, 0x370E7CAC8177C7C8UL),
        (Element.Pair, 256, 91553UL, 0x6DE88295CE961AB3UL),
        (Element.Pair, 257, 18464UL, 0x51C1AEF67BA2E37FUL),
        (Element.Pair, 300, 24611UL, 0x0370C3A2A10C3817UL),
        (Element.Pair, 1000, 110513UL, 0xE35E16ED9EB5710EUL),
        (Element.Pair, 3000, 310655UL, 0x447612B2086C10D6UL),
        (Element.Pair, 4096, 445862UL, 0xC15FF2D6F268F3B7UL),
        (Element.Pair, 4097, 437940UL, 0x0E7ADD554E5A6910UL),
        (Element.Pair, 10007, 1103020UL, 0x2CFA9FBC8C64ACE1UL),
        (Element.Pair, 65536, 8016340UL, 0x633254545353D418UL),
        (Element.Pair, 100000, 12581671UL, 0x273F94088CBAC92DUL),
        (Element.Pair, 1100000, 157819622UL, 0x32E58291C649FBB8UL),
    ];

    sealed class Trace
    {
        public ulong Count;
        public ulong Hash = FnvOffset;

        /// <summary>Folds the smaller and then the larger identity: asking a &lt; b or b &gt; a is the same question.</summary>
        public void Add(long a, long b)
        {
            Count++;
            var lo = Math.Min(a, b);
            var hi = Math.Max(a, b);
            Hash = (Hash ^ (ulong)lo) * FnvPrime;
            Hash = (Hash ^ (ulong)hi) * FnvPrime;
        }
    }

    readonly struct IdComparer(Trace trace) : IComparer<long>
    {
        public int Compare(long a, long b)
        {
            trace.Add(a, b);
            return (a >> KeyShift).CompareTo(b >> KeyShift);
        }
    }

    /// <summary>A 16-byte element, as the C++ <c>Pair</c>.</summary>
    readonly struct Pair(long key, long id)
    {
        public readonly long Key = key;
        public readonly long Id = id;
    }

    readonly struct PairComparer(Trace trace) : IComparer<Pair>
    {
        public int Compare(Pair a, Pair b)
        {
            trace.Add(a.Id, b.Id);
            return a.Key.CompareTo(b.Key);
        }
    }

    /// <summary>The generator of trace.cpp. Keys stay below 2^39 in magnitude.</summary>
    static long[] Generate(int shape, int p, int n)
    {
        var rng = 1000003UL + (ulong)shape * 257UL + (ulong)p * 65537UL + (ulong)n;
        ulong NextRandom()
        {
            var z = rng += 0x9E3779B97F4A7C15UL;
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
            return z ^ (z >> 31);
        }
        long Below(long m) => (long)(NextRandom() % (ulong)m);

        var a = new long[n];
        switch (shape)
        {
            case 0: for (var i = 0; i < n; i++) a[i] = (long)(NextRandom() >> 26); break;
            case 1: for (var i = 0; i < n; i++) a[i] = Below(p); break;
            case 2: for (var i = 0; i < n; i++) a[i] = i; break;
            case 3: for (var i = 0; i < n; i++) a[i] = n - i; break;
            case 4: for (var i = 0; i < n; i++) a[i] = i < n - i ? i : n - i; break;
            case 5: for (var i = 0; i < n; i++) a[i] = i % p; break;
            case 6:
                for (int i = 0, run = 0; i < n; run++)
                {
                    var len = 1 + (int)Below(p);
                    var start = Below(1 << 30);
                    for (var k = 0; k < len && i < n; k++, i++) a[i] = (run & 1) != 0 ? start - 3 * k : start + 3 * k;
                }
                break;
            case 7:
                for (var i = 0; i < n; i++) a[i] = i;
                for (var k = 0; k < n / 100 + 1; k++)
                {
                    int x = (int)Below(n), y = (int)Below(n);
                    (a[x], a[y]) = (a[y], a[x]);
                }
                break;
            default: for (var i = 0; i < n; i++) a[i] = Below(p) - p / 2; break;
        }
        return a;
    }

    static void SortTraced(Element element, long[] keys, Trace trace)
    {
        var n = keys.Length;
        if (element == Element.Int64)
        {
            var a = new long[n];
            for (var i = 0; i < n; i++) a[i] = keys[i] * (1L << KeyShift) + i;
            Ips4oSort.Sort(a.AsSpan(), new IdComparer(trace), NullContext.Default);
            for (var i = 1; i < n; i++)
                if (a[i] >> KeyShift < a[i - 1] >> KeyShift) throw new InvalidOperationException($"Int64 n={n}: not sorted");
        }
        else
        {
            var a = new Pair[n];
            for (var i = 0; i < n; i++) a[i] = new Pair(keys[i], keys[i] * (1L << KeyShift) + i);
            Ips4oSort.Sort(a.AsSpan(), new PairComparer(trace), NullContext.Default);
            for (var i = 1; i < n; i++)
                if (a[i].Key < a[i - 1].Key) throw new InvalidOperationException($"Pair n={n}: not sorted");
        }
    }

    public static IEnumerable<Func<Element>> Elements() => Enum.GetValues<Element>().Select(e => (Func<Element>)(() => e));

    [Test]
    [MethodDataSource(nameof(Elements))]
    public async Task ComparisonsMatchTheReference(Element element)
    {
        var mismatches = new List<string>();
        foreach (var row in Rows.Where(r => r.Element == element))
        {
            var trace = new Trace();
            foreach (var (shape, p) in Shapes)
                SortTraced(element, Generate(shape, p, row.Size), trace);

            if (trace.Count != row.Count || trace.Hash != row.Hash)
                mismatches.Add($"n={row.Size}: comparisons {trace.Count} (expected {row.Count}), hash 0x{trace.Hash:X16} (expected 0x{row.Hash:X16})");
        }

        await Assert.That(mismatches).IsEmpty();
    }
}
