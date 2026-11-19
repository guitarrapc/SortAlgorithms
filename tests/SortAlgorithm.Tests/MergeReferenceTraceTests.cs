using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

/// <summary>
/// PowerSort, TimSort and ShiftSort have C++ sources that define what they must do: Sebastian Wild's powersort with the merge that copies
/// the smaller run, gfx::timsort with CPython's binary insertion search and a galloping threshold kept at least 1, and James Quintero's C++
/// ShiftSort. Each row holds the comparison count and the hash of every compared pair, in order, that those sources produce for one size
/// across all shapes. A change to which elements are compared, or in what order, changes that trace even when the output stays sorted.
/// sandbox/MergeReferenceTrace/trace.cpp prints the rows.
/// </summary>
public class MergeReferenceTraceTests
{
    const ulong FnvOffset = 14695981039346656037UL;
    const ulong FnvPrime = 0x100000001b3UL;

    public enum Sorter { Power, Tim, Shift }

    static readonly (int Shape, int P)[] Shapes = [(0, 0), (1, 16), (2, 0), (3, 0), (4, 0), (5, 50), (6, 200), (7, 0)];

    static readonly (Sorter Sorter, int Size, ulong Count, ulong Hash)[] Rows =
    [
        (Sorter.Power, 2, 16UL, 0xE6A476EF4B548ED9UL),
        (Sorter.Power, 5, 52UL, 0x5CDD72C6B2BCBCB5UL),
        (Sorter.Power, 20, 441UL, 0x4ED8A2A5EC53EED9UL),
        (Sorter.Power, 23, 566UL, 0x9102C51E1BABF096UL),
        (Sorter.Power, 24, 601UL, 0xA9A6259CF13BD445UL),
        (Sorter.Power, 25, 589UL, 0x75BF5792F4A95228UL),
        (Sorter.Power, 31, 682UL, 0x7D76A263C9422932UL),
        (Sorter.Power, 32, 695UL, 0x0CA839B3631878AFUL),
        (Sorter.Power, 33, 685UL, 0x068070673D1F57D1UL),
        (Sorter.Power, 63, 1631UL, 0x76F0C70165868280UL),
        (Sorter.Power, 64, 1993UL, 0x482B7AA7791C474CUL),
        (Sorter.Power, 65, 1567UL, 0xDA5E4A95E46D9C80UL),
        (Sorter.Power, 100, 2779UL, 0x129B0B0FBEBAE6BBUL),
        (Sorter.Power, 1000, 41321UL, 0x01338920339B2ABEUL),
        (Sorter.Power, 10000, 573624UL, 0x38AA04C3D8B69316UL),
        (Sorter.Power, 100000, 7350972UL, 0xCD24769FEEF19E02UL),
        (Sorter.Tim, 2, 8UL, 0x752D73666AEC3D60UL),
        (Sorter.Tim, 5, 42UL, 0x33D941DA89111F68UL),
        (Sorter.Tim, 20, 307UL, 0xDBBD833E7BDDAA0FUL),
        (Sorter.Tim, 23, 363UL, 0x6ED01AE0937FFA11UL),
        (Sorter.Tim, 24, 353UL, 0x79C611756ED2B2A8UL),
        (Sorter.Tim, 25, 382UL, 0xC3D9F59385680F31UL),
        (Sorter.Tim, 31, 553UL, 0x1C0F256D3F5D1909UL),
        (Sorter.Tim, 32, 499UL, 0x4D58C276D2465F96UL),
        (Sorter.Tim, 33, 532UL, 0xD80F56F5D4A2843BUL),
        (Sorter.Tim, 63, 1543UL, 0x7C5742EA553E6A9DUL),
        (Sorter.Tim, 64, 1441UL, 0xCC4D400E8BE89987UL),
        (Sorter.Tim, 65, 1203UL, 0x2C2E20DD9F7F2021UL),
        (Sorter.Tim, 100, 2095UL, 0x611FA49816004A2CUL),
        (Sorter.Tim, 1000, 33053UL, 0x0D2F14BAF092F539UL),
        (Sorter.Tim, 10000, 336444UL, 0xAEFECB87E5D95134UL),
        (Sorter.Tim, 100000, 3777497UL, 0xC9FDACA27C50A32BUL),
        (Sorter.Shift, 2, 11UL, 0x980D35C3EA9B13A9UL),
        (Sorter.Shift, 5, 53UL, 0x8354A76A5D7C47D8UL),
        (Sorter.Shift, 20, 367UL, 0xBAEC30BFCD7253CCUL),
        (Sorter.Shift, 23, 460UL, 0xB8E86C7F2D0096C2UL),
        (Sorter.Shift, 24, 465UL, 0x093016ABFF38C313UL),
        (Sorter.Shift, 25, 483UL, 0xAE75D5FA56E5C6DAUL),
        (Sorter.Shift, 31, 705UL, 0xE9D64FBA36DFF31BUL),
        (Sorter.Shift, 32, 658UL, 0x131072C692841EDCUL),
        (Sorter.Shift, 33, 712UL, 0xDB326CCF79D14A1CUL),
        (Sorter.Shift, 63, 1856UL, 0x76AFEB993606D479UL),
        (Sorter.Shift, 64, 1936UL, 0x6670A5B4693783D9UL),
        (Sorter.Shift, 65, 1691UL, 0x049EB06F14A57F7EUL),
        (Sorter.Shift, 100, 3018UL, 0xB7F9AAE1E811AD3AUL),
        (Sorter.Shift, 1000, 55871UL, 0x7DB89A711213A8F2UL),
        (Sorter.Shift, 10000, 827408UL, 0x6CA36CEEDB5D0644UL),
        (Sorter.Shift, 100000, 10926042UL, 0x09541E82A8EF0308UL),
    ];

    sealed class Trace
    {
        public ulong Count;
        public ulong Hash = FnvOffset;
    }

    /// <summary>Folds the smaller and then the larger value of each compared pair, so operand order does not matter but pair order does.</summary>
    readonly struct TraceComparer(Trace trace) : IComparer<int>
    {
        public int Compare(int a, int b)
        {
            trace.Count++;
            var (lo, hi) = a < b ? (a, b) : (b, a);
            trace.Hash = (trace.Hash ^ (uint)lo) * FnvPrime;
            trace.Hash = (trace.Hash ^ (uint)hi) * FnvPrime;
            return a.CompareTo(b);
        }
    }

    /// <summary>The generator of trace.cpp.</summary>
    static int[] Generate(int shape, int p, int n)
    {
        var rng = 1000003UL + (ulong)shape * 257UL + (ulong)p * 65537UL + (ulong)n;
        ulong NextRandom()
        {
            var z = rng += 0x9E3779B97F4A7C15UL;
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
            return z ^ (z >> 31);
        }
        int Below(long m) => (int)(NextRandom() % (ulong)m);

        var a = new int[n];
        switch (shape)
        {
            case 0: for (var i = 0; i < n; i++) a[i] = Below(1L << 31); break;
            case 1: for (var i = 0; i < n; i++) a[i] = Below(p); break;
            case 2: for (var i = 0; i < n; i++) a[i] = i; break;
            case 3: for (var i = 0; i < n; i++) a[i] = n - i; break;
            case 4: for (var i = 0; i < n; i++) a[i] = i < n - i ? i : n - i; break;
            case 5: for (var i = 0; i < n; i++) a[i] = i % p; break;
            case 6:
                for (int i = 0, run = 0; i < n; run++)
                {
                    var len = 1 + Below(p);
                    var start = Below(1 << 30);
                    for (var k = 0; k < len && i < n; k++, i++) a[i] = (run & 1) != 0 ? start - 3 * k : start + 3 * k;
                }
                break;
            default:
                for (var i = 0; i < n; i++) a[i] = i;
                for (var k = 0; k < n / 100 + 1; k++)
                {
                    var x = Below(n);
                    var y = Below(n);
                    (a[x], a[y]) = (a[y], a[x]);
                }
                break;
        }
        return a;
    }

    public static IEnumerable<(Sorter Sorter, int Size, ulong Count, ulong Hash)> Cases() => Rows;

    [Test]
    [MethodDataSource(nameof(Cases))]
    public async Task ComparesTheSamePairsAsTheReference((Sorter Sorter, int Size, ulong Count, ulong Hash) row)
    {
        var trace = new Trace();
        foreach (var (shape, p) in Shapes)
        {
            var data = Generate(shape, p, row.Size);
            var comparer = new TraceComparer(trace);
            if (row.Sorter == Sorter.Shift)
                ShiftSort.Sort(data.AsSpan(), comparer, NullContext.Default);
            else if (row.Sorter == Sorter.Power)
                PowerSort.Sort(data.AsSpan(), comparer, NullContext.Default);
            else
                TimSort.Sort(data.AsSpan(), comparer, NullContext.Default);

            for (var i = 1; i < data.Length; i++)
                await Assert.That(data[i - 1] <= data[i]).IsTrue().Because($"{row.Sorter} n={row.Size} shape={shape}: not sorted at {i}");
        }

        await Assert.That(trace.Count).IsEqualTo(row.Count);
        await Assert.That(trace.Hash).IsEqualTo(row.Hash);
    }
}
