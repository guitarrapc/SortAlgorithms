using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using System.Runtime.Intrinsics;

namespace SortAlgorithm.Tests;

/// <summary>
/// SimdQuickSort's partition is a port of Partition512 in Bramas's sort512.hpp, so that source defines where every element goes. Each row
/// holds the boundary and an FNV-1a hash of the array that Partition512 leaves for one generated int array and a pivot taken from it. A
/// change to the order in which the partition reads vectors or stores lanes changes the hash even when the boundary stays the same. The
/// reference is written for 512-bit vectors of int, so the test runs only where that path does.
/// sandbox/SimdQuickSortReferenceTrace/trace.cpp prints the rows.
/// </summary>
public class SimdQuickSortReferenceTraceTests
{
    static readonly (int Shape, int N, int Bound, ulong Hash)[] Rows =
    [
        (0, 32, 9, 0x6D76B30347222726UL),
        (0, 33, 27, 0x142653270D7D2580UL),
        (0, 40, 8, 0xC5AA3E852029A172UL),
        (0, 47, 22, 0x1C4DDB5A2AB29D96UL),
        (0, 48, 34, 0xA05E1170E7CFA22EUL),
        (0, 49, 32, 0xD8710DDED70525C6UL),
        (0, 63, 21, 0x2C46397455BA7E2DUL),
        (0, 64, 32, 0x98B797B2F4C0B0CDUL),
        (0, 65, 49, 0x3DCCB41CFEDA4883UL),
        (0, 100, 78, 0x4B8873DBC5176223UL),
        (0, 255, 140, 0x89A52FE8937D29E8UL),
        (0, 256, 237, 0xA4E743EE3C84CF47UL),
        (0, 257, 247, 0x9417048F4BAFC33DUL),
        (0, 1000, 812, 0x7A5B27C5B15FB8B6UL),
        (0, 4096, 1001, 0xD6B0A5DA29854657UL),
        (0, 10007, 6372, 0xA31E260CBAFC3334UL),
        (1, 32, 11, 0xB6C1B2A42DD68035UL),
        (1, 33, 12, 0x8BFB0885DC0B202FUL),
        (1, 40, 14, 0x339A4A2EE81DAEADUL),
        (1, 47, 16, 0x4804A170DB2449E8UL),
        (1, 48, 17, 0x141914A0BDBED375UL),
        (1, 49, 17, 0x185C2375831EE7DFUL),
        (1, 63, 22, 0x042F88FC64C51592UL),
        (1, 64, 22, 0x0A1ACFB597765655UL),
        (1, 65, 22, 0x223D388A5DBD81E1UL),
        (1, 100, 34, 0xC15A34DC55320915UL),
        (1, 255, 86, 0x19CA7491318A6F12UL),
        (1, 256, 86, 0xF357B81B1C0B8B15UL),
        (1, 257, 86, 0x6DFB958BBF81CDE1UL),
        (1, 1000, 334, 0x8429ED6695B1BA8DUL),
        (1, 4096, 1366, 0x4E81FBEC8E2E6215UL),
        (1, 10007, 3336, 0x2974037FFAF16120UL),
        (2, 32, 22, 0x8B3920623F4FA9C5UL),
        (2, 33, 22, 0xB13243048C156ABCUL),
        (2, 40, 27, 0x218871D086CE70B5UL),
        (2, 47, 32, 0x58B88522F0DF3017UL),
        (2, 48, 32, 0xACA451624BA4A305UL),
        (2, 49, 33, 0x5DA9B85F26D572ACUL),
        (2, 63, 42, 0xE519F25111C1791DUL),
        (2, 64, 43, 0xD1456E595E215565UL),
        (2, 65, 44, 0x63891C2F6FAAC6B0UL),
        (2, 100, 67, 0x8A6A9BFC0B51C965UL),
        (2, 255, 170, 0xEEC4199C8F887F7DUL),
        (2, 256, 171, 0x7F3B9B666A2190A5UL),
        (2, 257, 172, 0x4B6E891F96304830UL),
        (2, 1000, 667, 0x39BB424FA8AF3F3DUL),
        (2, 4096, 2731, 0x1D4568DB455C85A5UL),
        (2, 10007, 6672, 0xAA8121E3F4816BA7UL),
        (3, 32, 24, 0x9D50A6BD999538DCUL),
        (3, 33, 10, 0x738990AF4564F783UL),
        (3, 40, 13, 0x9D6FABF2F482E00AUL),
        (3, 47, 36, 0x76449A96DC9A135EUL),
        (3, 48, 35, 0x1FAFF8B53A7653B2UL),
        (3, 49, 39, 0xA59EA3AB1B71855EUL),
        (3, 63, 47, 0x54FBED12884FCE75UL),
        (3, 64, 64, 0xEB813A6408022346UL),
        (3, 65, 65, 0x6761EAA194B11BE3UL),
        (3, 100, 39, 0x412D1A4F6178F4AAUL),
        (3, 255, 255, 0xCD24F9C699A98EB3UL),
        (3, 256, 132, 0x514C0E1FFDDDF529UL),
        (3, 257, 257, 0xA4E948ECD50C34D6UL),
        (3, 1000, 777, 0x21D5BAD1EFE28B96UL),
        (3, 4096, 4096, 0xA364B45B4E30CA5DUL),
        (3, 10007, 10007, 0x29A8C6CB1B8E991FUL),
    ];

    [Test]
    public async Task PartitionMatchesTheReference()
    {
        Skip.When(!SimdQuickSortLanes512<int>.IsSupported, "The reference partitions 512-bit vectors, which this machine does not run.");

        var problems = new List<string>();
        foreach (var (shape, n, bound, hash) in Rows)
        {
            var unobserved = Generate(shape, n);
            var pivot = unobserved[n / 3];
            var observed = unobserved.ToArray();

            var s = new SortSpan<int, ComparableComparer<int>, NullContext>(unobserved, NullContext.Default, default, 0);
            var unobservedBound = SimdQuickSort.Partition<int, ComparableComparer<int>, NullContext, SimdQuickSortLanes512<int>, Vector512<int>>(s, 0, n - 1, pivot);
            var o = new SortSpan<int, ComparableComparer<int>, StatisticsContext>(observed, new StatisticsContext(), default, 0);
            var observedBound = SimdQuickSort.Partition<int, ComparableComparer<int>, StatisticsContext, SimdQuickSortLanes512<int>, Vector512<int>>(o, 0, n - 1, pivot);

            if (unobservedBound != bound || Hash(unobserved) != hash || observedBound != bound || Hash(observed) != hash)
            {
                problems.Add($"shape {shape}, n = {n}");
            }
        }

        await Assert.That(problems).IsEmpty();
    }

    static int[] Generate(int shape, int n)
    {
        var rng = (ulong)shape * 1000003 + (ulong)n;
        var a = new int[n];
        for (var i = 0; i < n; i++)
        {
            a[i] = shape switch
            {
                0 => (int)(Next(ref rng) % 2000001) - 1000000,
                1 => i,
                2 => n - i,
                _ => (int)(Next(ref rng) % 4),
            };
        }
        return a;
    }

    static ulong Next(ref ulong state)
    {
        var z = state += 0x9E3779B97F4A7C15UL;
        z = (z ^ (z >> 30)) * 0xBF58476D1CE4E5B9UL;
        z = (z ^ (z >> 27)) * 0x94D049BB133111EBUL;
        return z ^ (z >> 31);
    }

    static ulong Hash(int[] a)
    {
        var hash = 14695981039346656037UL;
        foreach (var x in a)
        {
            hash = (hash ^ (uint)x) * 0x100000001b3UL;
        }
        return hash;
    }
}
