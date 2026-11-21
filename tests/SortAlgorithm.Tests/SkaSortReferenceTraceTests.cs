using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

/// <summary>
/// SkaSort is a port of ska_sort.hpp. Each row holds what that source does for one key type and size across all shapes: how many
/// exchanges its radix levels make, how many ranges it hands to std::sort, and the hash of both, in order, with the positions they
/// touch. A change to where the levels exchange, in what order, or which ranges reach std::sort changes the trace even when the output
/// stays sorted. What std::sort does inside a range is not traced. sandbox/SkaSortReferenceTrace/trace.cpp prints the rows.
/// </summary>
public class SkaSortReferenceTraceTests
{
    const ulong FnvOffset = 14695981039346656037UL;
    const ulong FnvPrime = 0x100000001b3UL;

    public enum Key { SByte, Int16, Int32, Int64 }

    static readonly (int Shape, int P)[] Shapes = [(0, 0), (1, 16), (2, 0), (3, 0), (4, 0), (5, 50), (6, 200), (7, 0), (8, 1 << 20)];

    static readonly (Key Key, int Size, ulong Swaps, ulong StdSorts, ulong Hash)[] Rows =
    [
        (Key.SByte, 2, 0UL, 9UL, 0xAB85A956EC629AC7UL),
        (Key.SByte, 5, 0UL, 9UL, 0x7B8BF6DEF0E86226UL),
        (Key.SByte, 6, 0UL, 9UL, 0x473F93D4B54CB2DBUL),
        (Key.SByte, 23, 0UL, 9UL, 0x860C8F158DE779F0UL),
        (Key.SByte, 24, 0UL, 9UL, 0xB2C158F2C7852E45UL),
        (Key.SByte, 100, 0UL, 9UL, 0x760E45EB0B943981UL),
        (Key.SByte, 127, 0UL, 9UL, 0xB263D633A0E4C718UL),
        (Key.SByte, 128, 795UL, 0UL, 0x8F1C7154C69C48D1UL),
        (Key.SByte, 129, 1056UL, 0UL, 0x675ACE05B620061AUL),
        (Key.SByte, 200, 1634UL, 0UL, 0x0F893BE3BEC8A0A7UL),
        (Key.SByte, 1000, 8807UL, 0UL, 0x91F8A66EEDCB94F4UL),
        (Key.SByte, 1023, 8960UL, 0UL, 0xD06EC8ABE2C963CFUL),
        (Key.SByte, 1024, 9213UL, 0UL, 0x61F556CD7892DA00UL),
        (Key.SByte, 1025, 9222UL, 0UL, 0x0CB642944636F38CUL),
        (Key.SByte, 3000, 26994UL, 0UL, 0x8D87B2271CF851C1UL),
        (Key.SByte, 10000, 89994UL, 0UL, 0x36FE1B39F4BDE942UL),
        (Key.SByte, 100000, 899996UL, 0UL, 0xDE59D7D304195EC4UL),
        (Key.Int16, 2, 0UL, 9UL, 0xAB85A956EC629AC7UL),
        (Key.Int16, 5, 0UL, 9UL, 0x7B8BF6DEF0E86226UL),
        (Key.Int16, 6, 0UL, 9UL, 0x473F93D4B54CB2DBUL),
        (Key.Int16, 23, 0UL, 9UL, 0x860C8F158DE779F0UL),
        (Key.Int16, 24, 0UL, 9UL, 0xB2C158F2C7852E45UL),
        (Key.Int16, 100, 0UL, 9UL, 0x760E45EB0B943981UL),
        (Key.Int16, 127, 0UL, 9UL, 0xB263D633A0E4C718UL),
        (Key.Int16, 128, 669UL, 48UL, 0xEA494BC5709FAA78UL),
        (Key.Int16, 129, 675UL, 46UL, 0x2BD3A09DB5373174UL),
        (Key.Int16, 200, 1080UL, 101UL, 0xF90BFB63732CBE27UL),
        (Key.Int16, 1000, 8260UL, 480UL, 0xF69FDB28FE4EEC04UL),
        (Key.Int16, 1023, 8567UL, 482UL, 0x728D4CAF226A7799UL),
        (Key.Int16, 1024, 11416UL, 494UL, 0x810449AC8FADBEBEUL),
        (Key.Int16, 1025, 11311UL, 484UL, 0x5AFABE84B2C492DBUL),
        (Key.Int16, 3000, 34293UL, 569UL, 0x60A35C9AA1E19BA4UL),
        (Key.Int16, 10000, 116367UL, 637UL, 0xC01366FAB91B86D9UL),
        (Key.Int16, 100000, 1550061UL, 7UL, 0x547384DD118CFB42UL),
        (Key.Int32, 2, 0UL, 9UL, 0xAB85A956EC629AC7UL),
        (Key.Int32, 5, 0UL, 9UL, 0x7B8BF6DEF0E86226UL),
        (Key.Int32, 6, 0UL, 9UL, 0x473F93D4B54CB2DBUL),
        (Key.Int32, 23, 0UL, 9UL, 0x860C8F158DE779F0UL),
        (Key.Int32, 24, 0UL, 9UL, 0xB2C158F2C7852E45UL),
        (Key.Int32, 100, 0UL, 9UL, 0x760E45EB0B943981UL),
        (Key.Int32, 127, 0UL, 9UL, 0xB263D633A0E4C718UL),
        (Key.Int32, 128, 597UL, 25UL, 0xEBA6DC30E7D897D0UL),
        (Key.Int32, 129, 615UL, 32UL, 0xB57826FA678999DAUL),
        (Key.Int32, 200, 1086UL, 52UL, 0xE8EF7050AEC9E51CUL),
        (Key.Int32, 1000, 8841UL, 260UL, 0x8E13DA227A388F7FUL),
        (Key.Int32, 1023, 9888UL, 271UL, 0x94D93279F84C1402UL),
        (Key.Int32, 1024, 12489UL, 261UL, 0x5634853B65A8751CUL),
        (Key.Int32, 1025, 12580UL, 265UL, 0xEBFFA66C257F06B0UL),
        (Key.Int32, 3000, 41550UL, 976UL, 0x0A4EDF1BD9B18C41UL),
        (Key.Int32, 10000, 139208UL, 3278UL, 0xA59B9FE91720A46DUL),
        (Key.Int32, 100000, 1844663UL, 35452UL, 0xF4E21879A1D77344UL),
        (Key.Int64, 2, 0UL, 9UL, 0xAB85A956EC629AC7UL),
        (Key.Int64, 5, 0UL, 9UL, 0x7B8BF6DEF0E86226UL),
        (Key.Int64, 6, 0UL, 9UL, 0x473F93D4B54CB2DBUL),
        (Key.Int64, 23, 0UL, 9UL, 0x860C8F158DE779F0UL),
        (Key.Int64, 24, 0UL, 9UL, 0xB2C158F2C7852E45UL),
        (Key.Int64, 100, 0UL, 9UL, 0x760E45EB0B943981UL),
        (Key.Int64, 127, 0UL, 9UL, 0xB263D633A0E4C718UL),
        (Key.Int64, 128, 601UL, 28UL, 0x36AD92064ACBB4B0UL),
        (Key.Int64, 129, 612UL, 23UL, 0xF9C46CC7A5336DEDUL),
        (Key.Int64, 200, 1087UL, 52UL, 0xC50ECB63315B0474UL),
        (Key.Int64, 1000, 8844UL, 255UL, 0x8369B17AA6E7046DUL),
        (Key.Int64, 1023, 9886UL, 265UL, 0x8C847A17B4284F2DUL),
        (Key.Int64, 1024, 12490UL, 266UL, 0x240410F59529C432UL),
        (Key.Int64, 1025, 12580UL, 270UL, 0x11E310219FECA312UL),
        (Key.Int64, 3000, 41551UL, 976UL, 0x156AF439821FCC73UL),
        (Key.Int64, 10000, 139207UL, 3278UL, 0xC29C9EF22683BD29UL),
        (Key.Int64, 100000, 1844636UL, 35329UL, 0x9D4EE36C72E75D19UL),
    ];

    sealed class Trace
    {
        public ulong Swaps;
        public ulong StdSorts;
        public ulong Hash = FnvOffset;
        bool inStdSort;

        void Fold(ulong value) => Hash = (Hash ^ value) * FnvPrime;

        public VisualizationContext Context() => new(
            onSwap: (i, j, _) =>
            {
                if (inStdSort) return;
                Swaps++;
                Fold(1);
                Fold((ulong)i);
                Fold((ulong)j);
            },
            onPhase: (phase, p1, p2, _) =>
            {
                switch (phase)
                {
                    case SortPhase.SkaSortStdSort:
                        inStdSort = true;
                        StdSorts++;
                        Fold(2);
                        Fold((ulong)p1);
                        Fold((ulong)(p2 - p1 + 1));
                        break;
                    case SortPhase.RadixPass:
                        inStdSort = false;
                        break;
                }
            });
    }

    /// <summary>The generator of trace.cpp. Values are made as 64-bit integers and truncated to the element type.</summary>
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
            case 0: for (var i = 0; i < n; i++) a[i] = (long)NextRandom(); break;
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

    static void SortTraced(Key key, long[] values, Trace trace)
    {
        switch (key)
        {
            case Key.SByte: Check(values.Select(v => unchecked((sbyte)v)).ToArray(), trace); break;
            case Key.Int16: Check(values.Select(v => unchecked((short)v)).ToArray(), trace); break;
            case Key.Int32: Check(values.Select(v => unchecked((int)v)).ToArray(), trace); break;
            default: Check(values, trace); break;
        }
    }

    static void Check<T>(T[] array, Trace trace) where T : System.Numerics.IBinaryInteger<T>
    {
        var expected = array.Order().ToArray();
        SkaSort.Sort(array.AsSpan(), trace.Context());
        if (!array.SequenceEqual(expected)) throw new InvalidOperationException($"{typeof(T).Name} n={array.Length}: not sorted");
    }

    public static IEnumerable<Func<Key>> Keys() => Enum.GetValues<Key>().Select(k => (Func<Key>)(() => k));

    [Test]
    [MethodDataSource(nameof(Keys))]
    public async Task RadixLevelsMakeTheReferenceExchanges(Key key)
    {
        var mismatches = new List<string>();
        foreach (var row in Rows.Where(r => r.Key == key))
        {
            var trace = new Trace();
            foreach (var (shape, p) in Shapes)
                SortTraced(key, Generate(shape, p, row.Size), trace);

            if (trace.Swaps != row.Swaps || trace.StdSorts != row.StdSorts || trace.Hash != row.Hash)
                mismatches.Add($"n={row.Size}: swaps {trace.Swaps} (expected {row.Swaps}), std::sort {trace.StdSorts} (expected {row.StdSorts}), hash 0x{trace.Hash:X16} (expected 0x{row.Hash:X16})");
        }

        await Assert.That(mismatches).IsEmpty();
    }
}
