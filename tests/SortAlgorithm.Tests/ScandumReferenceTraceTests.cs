using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

/// <summary>
/// QuadSort and FluxSort are ports, so the C sources of quadsort and fluxsort 1.2.1.3 define what they must do. Each row holds the
/// comparison count and the hash of every compared pair, in order, that the C sources produce for the same inputs, with fluxsort's pivot
/// sample offset replaced by FluxSort's. A change to which elements are compared, or in what order, changes that trace even when the output
/// stays sorted.
/// sandbox/ScandumReferenceTrace/trace.c prints the rows.
/// </summary>
public class ScandumReferenceTraceTests
{
    const ulong FnvOffset = 14695981039346656037UL;
    const ulong FnvPrime = 0x100000001b3UL;
    const int KeyShift = 28;
    const int CappedSawPeriod = 2_097_152;

    enum Sorter { Quad, Flux }

    enum Plan { Core, All, AllSomeQuarters, AllEveryQuarter, Capped }

    static readonly (int Shape, long P)[] AllShapes =
    [
        (0, 0), (1, 2), (1, 4), (1, 16), (1, 100), (1, 1000), (2, 0), (3, 0), (4, 2), (4, 3), (4, 16), (5, 0), (7, 10), (7, 30), (7, 50), (7, 90),
        (8, 7), (8, 32), (8, 64), (8, 128), (8, 256), (8, 1000), (9, 7), (9, 32), (9, 64), (9, 128), (9, 256), (9, 1000), (10, 0), (11, 4), (11, 16), (11, 100),
        (12, 2), (12, 4), (12, 10), (13, 2), (13, 4), (13, 10), (14, 50), (14, 90), (14, 99), (15, 25), (15, 45), (15, 50), (15, 55), (15, 75),
        (16, 25), (16, 45), (16, 50), (16, 55), (16, 75), (17, 2), (17, 6), (18, 33), (18, 100), (18, 500), (19, 0), (20, 8), (20, 64), (20, 1024),
        (21, 8), (21, 64), (21, 1024), (22, 0), (23, 0), (24, 0), (25, 1), (25, 2), (25, 3), (26, 2), (27, 4), (28, 51), (29, 60),
    ];

    static readonly (int Shape, long P)[] CoreShapes =
    [
        (0, 0), (1, 4), (1, 1000), (7, 30), (8, 1024), (11, 100), (12, 4), (14, 90), (15, 50), (15, 60), (16, 50), (20, 64),
    ];

    static readonly (int Shape, long P)[] CappedShapes = [(8, CappedSawPeriod)];

    static readonly (Sorter Sorter, int First, int Last, int Step, int Swap, Plan Plan, ulong Count, ulong Hash)[] Rows =
    [
        (Sorter.Quad, 2, 128, 1, 0, Plan.All, 2299534UL, 0x8BE4A5D52DFD6519UL),
        (Sorter.Quad, 129, 1100, 29, 0, Plan.All, 7997906UL, 0xB693D4AB1C9375A1UL),
        (Sorter.Quad, 2047, 2049, 1, 0, Plan.All, 2668162UL, 0x3C94920B7CB19B69UL),
        (Sorter.Quad, 4095, 4097, 1, 0, Plan.All, 5707974UL, 0x4CABEB8C6FCE4240UL),
        (Sorter.Quad, 8192, 8192, 1, 0, Plan.All, 3953749UL, 0x0526229BD20B1AFCUL),
        (Sorter.Quad, 20000, 20000, 1, 0, Plan.Core, 2144309UL, 0x920879DF0C1BFB40UL),
        (Sorter.Quad, 97, 1097, 100, 32, Plan.All, 2687799UL, 0x9EA228EB9B71B463UL),
        (Sorter.Quad, 97, 1097, 100, 64, Plan.All, 2630394UL, 0x88568C7585FABF77UL),
        (Sorter.Quad, 97, 1097, 100, 100, Plan.All, 2614682UL, 0x4AD6AF9886FC6E0FUL),
        (Sorter.Quad, 97, 1097, 100, 512, Plan.All, 2551007UL, 0xB38F1EADE2339147UL),
        (Sorter.Quad, 5000, 5000, 1, 32, Plan.Core, 500736UL, 0xCEC15628B35CF506UL),
        (Sorter.Quad, 5000, 5000, 1, 64, Plan.Core, 490197UL, 0x0FED1AAD9A2D7447UL),
        (Sorter.Quad, 5000, 5000, 1, 512, Plan.All, 2420531UL, 0xB4B21CFC00D127B0UL),
        (Sorter.Quad, 20000, 20000, 1, 2048, Plan.Core, 2173238UL, 0x20AEF5C42D4F7B6EUL),
        (Sorter.Quad, 1536, 1536, 1, 512, Plan.Core, 121817UL, 0xE023771305A711B0UL),
        (Sorter.Quad, 300000, 300000, 1, 70000, Plan.Core, 39852986UL, 0xBB948842E3E727FBUL),
        (Sorter.Quad, 4500000, 4500000, 1, 0, Plan.Capped, 10217440UL, 0x6EF441EF45FE5F23UL),
        (Sorter.Flux, 2, 132, 13, 0, Plan.All, 210248UL, 0x00C825BED1F4DC29UL),
        (Sorter.Flux, 133, 140, 1, 0, Plan.AllEveryQuarter, 10280372UL, 0xADB323047F372AB0UL),
        (Sorter.Flux, 256, 264, 1, 0, Plan.AllSomeQuarters, 1494871UL, 0xDEC9C9ECF454A511UL),
        (Sorter.Flux, 380, 392, 1, 0, Plan.AllSomeQuarters, 3056014UL, 0x4D5CB877267F43A2UL),
        (Sorter.Flux, 510, 524, 1, 0, Plan.AllSomeQuarters, 4710250UL, 0x914B2D9799DB666AUL),
        (Sorter.Flux, 1000, 1000, 1, 0, Plan.AllSomeQuarters, 635638UL, 0x431D762F8F902CCCUL),
        (Sorter.Flux, 2000, 2000, 1, 0, Plan.All, 1034225UL, 0xDF596728ED72006BUL),
        (Sorter.Flux, 2044, 2052, 1, 0, Plan.AllSomeQuarters, 12633271UL, 0x0C727818C6CB38DFUL),
        (Sorter.Flux, 4095, 4097, 1, 0, Plan.AllSomeQuarters, 8775035UL, 0x75024BAE50A8F39DUL),
        (Sorter.Flux, 8190, 8200, 1, 0, Plan.AllSomeQuarters, 68035769UL, 0x046FF133CFEFA0D3UL),
        (Sorter.Flux, 20000, 20000, 1, 0, Plan.Core, 2235036UL, 0x1D2BCE24E556A92CUL),
        (Sorter.Flux, 32768, 32769, 1, 0, Plan.Core, 7708274UL, 0x8F99BDDBD573964EUL),
        (Sorter.Flux, 100000, 100000, 1, 0, Plan.Core, 12665845UL, 0xCB12EC0CE8A67F38UL),
    ];

    sealed class Trace
    {
        public ulong Count;
        public ulong Hash = FnvOffset;
    }

    readonly struct TraceComparer(Trace trace) : IComparer<long>
    {
        public int Compare(long a, long b)
        {
            trace.Count++;
            trace.Hash = (trace.Hash ^ (ulong)a) * FnvPrime;
            trace.Hash = (trace.Hash ^ (ulong)b) * FnvPrime;
            return (a >> KeyShift).CompareTo(b >> KeyShift);
        }
    }

    /// <summary>The generator of trace.c: the keys of one input, each packed above its position so equal keys stay distinguishable.</summary>
    sealed class InputGenerator(int shape, long p, long p2, int n)
    {
        ulong rng = 1000003UL + (ulong)shape * 257UL + (ulong)p * 65537UL + (ulong)p2 * 17UL + (ulong)n;

        ulong NextRandom()
        {
            var z = rng += 0x9E3779B97F4A7C15UL;
            z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
            z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
            return z ^ (z >> 31);
        }

        long Below(long m) => (long)(NextRandom() % (ulong)m);

        public void Fill(long[] a)
        {
            long size = n;
            long half1 = size / 2, quad1 = half1 / 2, quad2 = half1 - quad1, half2 = size - half1, quad3 = half2 / 2, quad4 = half2 - quad3;
            long[] start = [0, quad1, half1, half1 + quad3];
            long[] length = [quad1, quad2, quad3, quad4];

            switch (shape)
            {
                case 0: for (long i = 0; i < size; i++) a[i] = Below(1L << 32); break;
                case 1: for (long i = 0; i < size; i++) a[i] = Below(p); break;
                case 2: for (long i = 0; i < size; i++) a[i] = i; break;
                case 3: for (long i = 0; i < size; i++) a[i] = size - i; break;
                case 4: for (long i = 0; i < size; i++) a[i] = (size - i) / p; break;
                case 5: for (long i = 0; i < size; i++) a[i] = 7; break;
                case 6:
                    var bandBase = 16 * size;
                    for (var q = 0; q < 4; q++)
                    {
                        var mode = (int)((p >> (2 * q)) & 3);
                        for (long j = 0; j < length[q]; j++)
                        {
                            var v = mode == 0 ? Below(size) : mode == 1 ? j : mode == 2 ? length[q] - j : Below(4);
                            a[start[q] + j] = bandBase + v;
                        }
                        bandBase += ((p2 >> q) & 1) != 0 ? -2 * size : 2 * size;
                    }
                    break;
                case 7: for (long i = 0; i < size; i++) a[i] = i < size * p / 100 ? Below(size / 10 + 1) : size + Below(size); break;
                case 8: for (long i = 0; i < size; i++) a[i] = i % p; break;
                case 9: for (long i = 0; i < size; i++) a[i] = p - i % p; break;
                case 10: for (long i = 0; i < size; i++) a[i] = i < size - i ? i : size - i; break;
                case 11:
                    for (long i = 0; i < size; i++) a[i] = i;
                    for (long i = 0; i < size / p; i++)
                    {
                        var x = Below(size);
                        var y = Below(size);
                        (a[x], a[y]) = (a[y], a[x]);
                    }
                    break;
                case 12: for (long i = 0; i < size; i++) a[i] = i < size - size / p ? i : Below(size); break;
                case 13: for (long i = 0; i < size; i++) a[i] = i < size / p ? Below(size) : i; break;
                case 14: for (long i = 0; i < size; i++) a[i] = Below(100) < p ? size / 2 : Below(size); break;
                case 15:
                    for (var q = 0; q < 4; q++)
                        for (long j = 0; j < length[q]; j++) a[start[q] + j] = j < length[q] * p / 100 ? j : Below(size);
                    break;
                case 16:
                    for (var q = 0; q < 4; q++)
                        for (long j = 0; j < length[q]; j++) a[start[q] + j] = j < length[q] * p / 100 ? length[q] - j : Below(size);
                    break;
                case 17: for (long i = 0; i < size; i++) a[i] = i < size / 2 ? 5 : Below(p); break;
                case 18:
                    long block = 0;
                    for (long i = 0; i < size; i++)
                    {
                        if (i % p == 0) block = Below(size);
                        a[i] = block + i % p;
                    }
                    break;
                case 19: for (long i = 0; i < size; i++) a[i] = (i & 1) != 0 ? size + i : i; break;
                case 20:
                    for (long i = 0; i < size; i++)
                    {
                        var b = i / p;
                        a[i] = b * p * 2 + ((b & 1) != 0 ? p - i % p : i % p);
                    }
                    break;
                case 21:
                    for (long i = 0; i < size; i++)
                    {
                        var b = i / p;
                        a[i] = (size / p - b + 1) * p * 2 + ((b & 1) != 0 ? p - i % p : i % p);
                    }
                    break;
                // Descending stretches whose only ties fall between the pairs the quad swap compares first, alone and after a group of
                // eight that is ascending (22) or descending (23). Treating a tie as descending reverses equal keys.
                case 22: for (long i = 0; i < size; i++) a[i] = i < 8 ? i - 1000 : -((i - 7) / 2); break;
                case 23: for (long i = 0; i < size; i++) a[i] = i < 8 ? 1000 - i : -((i - 7) / 2); break;
                case 24: for (long i = 0; i < size; i++) a[i] = -((i + 1) / 2); break;
                // Strictly descending quarters whose boundary p ties, so the analysis must not reverse across it.
                case 25: for (long i = 0; i < size; i++) a[i] = size - i + (i >= start[p] ? 1 : 0); break;
                // The pseudomedian candidates are low and p percent of the rest is lower, so the not-greater side is small.
                case 26:
                    var lowStride = Math.Max(size / 9, 1);
                    for (long i = 0; i < size; i++) a[i] = i % lowStride == 0 ? 100 + i / lowStride : Below(100) < p ? Below(100) : 1000 + Below(size);
                    break;
                // The pseudomedian candidates and p percent of the rest hold the maximum, so the reverse partition takes out a few equal keys.
                case 27:
                    var highStride = Math.Max(size / 9, 1);
                    for (long i = 0; i < size; i++) a[i] = i % highStride == 0 || Below(100) < p ? size : Below(size);
                    break;
                // Nine equal pseudomedian candidates, with one side of the partition near the 1:32 limit at 2,000 elements.
                case 28:
                    var smallSideStride = Math.Max(size / 9, 1);
                    var lowStride28 = Math.Max(size / p, 1);
                    for (long i = 0; i < size; i++) a[i] = i % smallSideStride == 0 ? 500 : (i + 7) % lowStride28 == 0 ? Below(400) : 10000 + Below(size);
                    break;
                case 29:
                    var largeSideStride = Math.Max(size / 9, 1);
                    var highStride29 = Math.Max(size / p, 1);
                    for (long i = 0; i < size; i++) a[i] = i % largeSideStride == 0 ? 500 : (i + 7) % highStride29 == 0 ? 10000 + Below(size) : Below(400);
                    break;
                default: throw new ArgumentOutOfRangeException(nameof(shape));
            }

            for (long i = 0; i < size; i++) a[i] = (a[i] << KeyShift) | i;
        }
    }

    /// <summary>Runs one input and returns a description of the failure when its result is not ordered by key and then by position.</summary>
    static string? RunCase(Sorter sorter, int swap, int shape, long p, long p2, long[] input, Trace trace)
    {
        new InputGenerator(shape, p, p2, input.Length).Fill(input);
        var comparer = new TraceComparer(trace);

        if (sorter == Sorter.Flux) FluxSort.Sort(input.AsSpan(), comparer, NullContext.Default);
        else if (swap == 0) QuadSort.Sort(input.AsSpan(), comparer, NullContext.Default);
        else QuadSort.SortWithSwapSize(input.AsSpan(), swap, comparer, NullContext.Default);

        for (var i = 1; i < input.Length; i++)
        {
            if (input[i - 1] >= input[i]) return $"shape {shape}, p {p}, p2 {p2}, n {input.Length}: unsorted or unstable at {i}";
        }
        return null;
    }

    static string? RunRow((Sorter Sorter, int First, int Last, int Step, int Swap, Plan Plan, ulong Count, ulong Hash) row, Trace trace)
    {
        var shapes = row.Plan switch
        {
            Plan.Core => CoreShapes,
            Plan.Capped => CappedShapes,
            _ => AllShapes,
        };

        for (var n = row.First; n <= row.Last; n += row.Step)
        {
            var input = new long[n];
            foreach (var (shape, p) in shapes)
            {
                if (RunCase(row.Sorter, row.Swap, shape, p, 0, input, trace) is { } failure) return failure;
            }

            var (maskStep, boundaryStep, boundaryEnd) = row.Plan switch
            {
                Plan.AllSomeQuarters => (17, 5, 5),
                Plan.AllEveryQuarter => (1, 1, 7),
                _ => (0, 0, 0),
            };
            if (maskStep == 0) continue;

            for (var mask = 0; mask < 256; mask += maskStep)
            {
                for (var boundary = 0; boundary <= boundaryEnd; boundary += boundaryStep)
                {
                    if (RunCase(row.Sorter, row.Swap, 6, mask, boundary, input, trace) is { } failure) return failure;
                }
            }
        }
        return null;
    }

    static async Task VerifyRows(Sorter sorter)
    {
        foreach (var row in Rows.Where(r => r.Sorter == sorter))
        {
            var trace = new Trace();
            var failure = RunRow(row, trace);
            var label = $"{sorter} n={row.First}..{row.Last} step {row.Step} swap {row.Swap} {row.Plan}";

            await Assert.That(failure).IsNull().Because(label);
            await Assert.That(trace.Count).IsEqualTo(row.Count).Because(label);
            await Assert.That(trace.Hash).IsEqualTo(row.Hash).Because(label);
        }
    }

    [Test]
    public Task QuadSortMakesTheReferenceComparisons() => VerifyRows(Sorter.Quad);

    [Test]
    public Task FluxSortMakesTheReferenceComparisons() => VerifyRows(Sorter.Flux);
}
