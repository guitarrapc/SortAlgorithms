using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class BitonicSortAnyLengthTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => BitonicSortAnyLength.Sort(span, context);

    public static IEnumerable<int> SequenceSizes() => [.. Enumerable.Range(1, 40), 63, 64, 65, 100, 127, 128, 129, 1000];

    /// <summary>
    /// The comparisons and swaps are those of the program on Lang's page, in its order. The keys are distinct, so its exchange of equal keys
    /// in a descending comparator, which this class leaves out, never applies.
    /// </summary>
    [Test]
    [MethodDataSource(nameof(SequenceSizes))]
    public async Task EventSequenceMatchesLangsProgramTest(int n)
    {
        var input = TestHelpers.ShuffledRange(n, 42);
        var recorder = new BitonicSortTests.NetworkEventRecorder();
        BitonicSortAnyLength.Sort(input.ToArray().AsSpan(), recorder);

        var a = input.ToArray();
        var expected = new List<string>();
        BitonicSort(0, n, true);

        await Assert.That(recorder.Events).IsEquivalentTo(expected, CollectionOrdering.Matching);

        void BitonicSort(int lo, int count, bool dir)
        {
            if (count > 1)
            {
                var m = count / 2;
                BitonicSort(lo, m, !dir);
                BitonicSort(lo + m, count - m, dir);
                BitonicMerge(lo, count, dir);
            }
        }

        void BitonicMerge(int lo, int count, bool dir)
        {
            if (count > 1)
            {
                var m = 1;
                while (m < count) m <<= 1;
                m >>= 1;
                for (var i = lo; i < lo + count - m; i++)
                {
                    expected.Add($"compare {i} {i + m}");
                    if (dir == (a[i] > a[i + m]))
                    {
                        (a[i], a[i + m]) = (a[i + m], a[i]);
                        expected.Add($"swap {i} {i + m}");
                    }
                }
                BitonicMerge(lo, m, dir);
                BitonicMerge(lo + m, count - m, dir);
            }
        }
    }

    public static IEnumerable<int> ZeroOneSizes() => Enumerable.Range(1, 16);

    /// <summary>
    /// The zero-one principle: a comparator network sorts every input of length n if and only if it sorts every 0-1 input of length n, so
    /// enumerating all 2^n bit strings proves the network for this n sorts. Every length up to 16 is checked, not only the powers of two.
    /// </summary>
    [Test]
    [MethodDataSource(nameof(ZeroOneSizes))]
    public async Task ZeroOnePrincipleTest(int n)
    {
        var bits = new int[n];
        var failures = new List<string>();

        for (var mask = 0; mask < 1 << n; mask++)
        {
            var ones = 0;
            for (var i = 0; i < n; i++)
            {
                bits[i] = (mask >> i) & 1;
                ones += bits[i];
            }

            BitonicSortAnyLength.Sort(bits.AsSpan());

            for (var i = 0; i < n; i++)
            {
                if (bits[i] != (i < n - ones ? 0 : 1))
                {
                    failures.Add($"mask 0b{Convert.ToString(mask, 2).PadLeft(n, '0')}: position {i} holds {bits[i]}");
                    break;
                }
            }
            if (failures.Count >= 5) break;
        }

        await Assert.That(failures).IsEmpty();
    }

    [Test]
    [Arguments(2)]
    [Arguments(5)]
    [Arguments(17)]
    [Arguments(64)]
    [Arguments(100)]
    [Arguments(1024)]
    [Arguments(1000)]
    public async Task ComparisonsDependOnlyOnTheLengthTest(int n)
    {
        var counts = new List<ulong>();
        foreach (var input in new[] { Enumerable.Range(0, n).ToArray(), Enumerable.Range(0, n).Reverse().ToArray(), TestHelpers.ShuffledRange(n, 7), new int[n] })
        {
            var stats = new StatisticsContext();
            BitonicSortAnyLength.Sort(input.AsSpan(), stats);
            counts.Add(stats.CompareCount);
        }

        // For a power of two the network is the full bitonic network: n log₂ n (log₂ n + 1) / 4 comparators. Any other length uses a
        // subset of the network for the next power of two.
        var log = (int)Math.Ceiling(Math.Log2(n));
        var full = (ulong)((1L << log) * log * (log + 1) / 4);
        await Assert.That(counts.Distinct().Count()).IsEqualTo(1);
        if (int.IsPow2(n)) await Assert.That(counts[0]).IsEqualTo(full);
        else await Assert.That(counts[0]).IsLessThan(full);
    }
}
