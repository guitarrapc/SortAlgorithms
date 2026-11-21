using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class SimdQuickSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => SimdQuickSort.Sort(span, context);

    static int Lanes<T>() where T : IComparable<T> => SimdQuickSort.LaneCount<T, ComparableComparer<T>>();

    [Test]
    public async Task SortsEveryVectorizedType()
    {
        List<string> problems =
        [
            .. Check(x => (int)x), .. Check(x => (uint)x), .. Check(x => x), .. Check(x => (ulong)x),
            .. Check(x => (nint)x), .. Check(x => (nuint)x),
        ];

        await Assert.That(problems).IsEmpty();

        static List<string> Check<T>(Func<long, T> convert) where T : IComparable<T>
        {
            // Every size up to well past one network of 512-bit int vectors, plus sizes around the multiples that leave a remainder.
            int[] sizes = [.. Enumerable.Range(0, 300), 383, 384, 385, 511, 512, 513, 1000, 2049, 4097];
            var random = new Random(42);
            var problems = new List<string>();
            foreach (var n in sizes)
            {
                for (var pattern = 0; pattern < 6; pattern++)
                {
                    var source = new T[n];
                    for (var i = 0; i < n; i++)
                    {
                        // Pattern 4 holds the largest value, which is also the padding of the last vector of a network.
                        source[i] = convert(pattern switch
                        {
                            0 => i,
                            1 => n - i,
                            2 => random.Next(-1_000_000, 1_000_000),
                            3 => random.Next(4),
                            4 => (i & 1) == 0 ? long.MinValue : long.MaxValue,
                            _ => random.NextInt64(long.MinValue, long.MaxValue),
                        });
                    }

                    var expected = (T[])source.Clone();
                    Array.Sort(expected);
                    var unobserved = (T[])source.Clone();
                    SimdQuickSort.Sort(unobserved.AsSpan());
                    var observed = (T[])source.Clone();
                    SimdQuickSort.Sort(observed.AsSpan(), new StatisticsContext());

                    if (!unobserved.SequenceEqual(expected) || !observed.SequenceEqual(expected))
                    {
                        problems.Add($"{typeof(T).Name}: n = {n}, pattern {pattern}");
                    }
                }
            }
            return problems;
        }
    }

    [Test]
    public async Task SortedInputCountsFollowTheLaneCount()
    {
        await AssertSortedInputCounts(Lanes<int>(), stats => SimdQuickSort.Sort(Enumerable.Range(0, 16 * Lanes<int>() + 1).ToArray().AsSpan(), stats));
    }

    [Test]
    public async Task SortedInputCountsOnTheOneLanePath()
    {
        // A comparer that is not the natural-order one runs the one-lane path on any type.
        await AssertSortedInputCounts(1, stats => SimdQuickSort.Sort(Enumerable.Range(0, 17).ToArray().AsSpan(), new AscendingInt(), stats));
    }

    static async Task AssertSortedInputCounts(int w, Action<StatisticsContext> sort)
    {
        var stats = new StatisticsContext();
        sort(stats);

        // n = 16w + 1 sorted keys: the median of three is the middle (two comparisons of two reads each), it is swapped to the end and read,
        // the other 16w keys are each read, compared and written once, and the pivot is swapped to index 8w. Each side then holds 8w keys,
        // eight vectors, which the network reads once, sorts and writes once.
        var network = NetworkComparisons(8, w);
        await Assert.That(stats.CompareCount).IsEqualTo((ulong)(2 + 16 * w) + 2 * network);
        await Assert.That(stats.IndexReadCount).IsEqualTo((ulong)(4 + 2 + 1 + 16 * w + 2 + 16 * w));
        await Assert.That(stats.IndexWriteCount).IsEqualTo((ulong)(2 + 16 * w + 2 + 16 * w));
        await Assert.That(stats.SwapCount).IsEqualTo(2UL);
    }

    [Test]
    [Arguments(2)]
    [Arguments(3)]
    [Arguments(17)]
    [Arguments(64)]
    [Arguments(255)]
    [Arguments(256)]
    public async Task NetworkComparisonsFollowTheVectorCount(int n)
    {
        var w = Lanes<int>();
        Skip.When(n > 16 * w, "The range is partitioned with this lane count.");

        var stats = new StatisticsContext();
        SimdQuickSort.Sort(TestHelpers.ShuffledRange(n, 5).ToArray().AsSpan(), stats);

        // A range of at most sixteen vectors goes to the network at once: it is read and written once and partitioned never.
        await Assert.That(stats.CompareCount).IsEqualTo(NetworkComparisons((n + w - 1) / w, w));
        await Assert.That(stats.IndexReadCount).IsEqualTo((ulong)n);
        await Assert.That(stats.IndexWriteCount).IsEqualTo((ulong)n);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    [Test]
    public async Task EqualKeysLoseOnePerPartition()
    {
        var w = Lanes<int>();
        const int extra = 50;
        var partitions = 0;
        var networks = new List<(int Start, int End)>();
        var context = new VisualizationContext(onPhase: (phase, p1, p2, _) =>
        {
            if (phase == SortPhase.QuickSortPartition) partitions++;
            if (phase == SortPhase.SimdQuickSortBitonicSort) networks.Add((p1, p2));
        });
        SimdQuickSort.Sort(Enumerable.Repeat(7, 16 * w + extra).ToArray().AsSpan(), context);

        // Every key equal to the pivot goes left, so each partition removes only the pivot: the defined quadratic case.
        await Assert.That(partitions).IsEqualTo(extra);
        await Assert.That(networks).IsEquivalentTo([(0, 16 * w - 1)], CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(0)]
    [Arguments(1)]
    public async Task EachPartitionWritesItsRangeOnceAndEachNetworkReadsAndWritesItsRange(int path)
    {
        var source = TestHelpers.ShuffledRange(5000, 9);
        var events = new List<(char Kind, int A, int B)>();
        var context = new VisualizationContext(
            onIndexRead: (i, _) => events.Add(('r', i, 0)),
            onIndexWrite: (i, _, _) => events.Add(('w', i, 0)),
            onPhase: (phase, p1, p2, _) =>
            {
                if (phase == SortPhase.QuickSortPartition) events.Add(('p', p1, p2));
                if (phase == SortPhase.SimdQuickSortBitonicSort) events.Add(('n', p1, p2));
            });
        var array = source.ToArray();
        if (path == 0) SimdQuickSort.Sort(array.AsSpan(), context);
        else SimdQuickSort.Sort(array.AsSpan(), new AscendingInt(), context);

        var problems = new List<string>();
        var partitions = 0;
        for (var e = 0; e < events.Count; e++)
        {
            var (kind, start, end) = events[e];
            if (kind is not ('p' or 'n')) continue;

            var window = events.Skip(e + 1).TakeWhile(x => x.Kind is 'r' or 'w').ToList();
            var writes = window.Where(x => x.Kind == 'w').Select(x => x.A).Order().ToList();
            if (kind == 'p')
            {
                // The pivot sits at the end of the range; every other position is written exactly once, the pivot's swaps aside.
                partitions++;
                if (!writes.SequenceEqual(Enumerable.Range(start, end - start))) problems.Add($"partition [{start}, {end}]");
            }
            else
            {
                // The network reads its range first, then writes it; reads after the writes belong to the next pivot selection.
                var reads = window.TakeWhile(x => x.Kind == 'r').Select(x => x.A).Order().ToList();
                if (!writes.SequenceEqual(Enumerable.Range(start, end - start + 1)) || !reads.SequenceEqual(Enumerable.Range(start, end - start + 1)))
                {
                    problems.Add($"network [{start}, {end}]");
                }
            }
        }

        await Assert.That(partitions).IsGreaterThan(1);
        await Assert.That(problems).IsEmpty();
        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, 5000).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task CustomComparerRunsTheOneLanePath()
    {
        var source = TestHelpers.ShuffledRange(3000, 3);
        var actual = source.ToArray();
        SimdQuickSort.Sort(actual.AsSpan(), new DescendingInt(), NullContext.Default);

        await Assert.That(SimdQuickSort.LaneCount<int, DescendingInt>()).IsEqualTo(1);
        await Assert.That(actual).IsEquivalentTo(Enumerable.Range(0, 3000).Reverse().ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(0)]
    [Arguments(1)]
    public async Task RangeSortLeavesOutsideUntouched(int path)
    {
        var source = TestHelpers.ShuffledRange(1600, 11);
        var expected = source.ToArray();
        Array.Sort(expected, 3, 1590);
        var actual = source.ToArray();
        if (path == 0) SimdQuickSort.Sort(actual.AsSpan(), 3, 1593, NullContext.Default);
        else SimdQuickSort.Sort(actual.AsSpan(), 3, 1593, new AscendingInt(), NullContext.Default);

        await Assert.That(actual).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    /// <summary>
    /// The comparators of the network over <paramref name="vectors"/> vectors of <paramref name="w"/> lanes: W / 2 per step inside a vector
    /// and W per step between two vectors, skipping the steps whose second vector lies past the count.
    /// </summary>
    static ulong NetworkComparisons(int vectors, int w)
    {
        var log = 0;
        while (1 << log < w) log++;
        ulong sortVector = (ulong)(w / 2 * log * (log + 1) / 2);
        ulong cleanVector = (ulong)(w / 2 * log);

        return Network(vectors);

        ulong Network(int k)
        {
            if (k == 1) return sortVector;
            var h = 1;
            while (h * 2 < k) h *= 2;
            return Network(h) + Network(k - h) + (ulong)(w * (k - h)) + Clean(h) + Clean(k - h);
        }

        ulong Clean(int m)
        {
            var p = 1;
            while (p < m) p *= 2;
            ulong steps = 0;
            for (var d = p / 2; d >= 1; d /= 2)
            {
                for (var i = 0; i + d < m; i++)
                {
                    if ((i & d) == 0) steps++;
                }
            }
            return steps * (ulong)w + (ulong)m * cleanVector;
        }
    }

    readonly struct AscendingInt : IComparer<int>
    {
        public int Compare(int x, int y) => x.CompareTo(y);
    }

    readonly struct DescendingInt : IComparer<int>
    {
        public int Compare(int x, int y) => y.CompareTo(x);
    }
}
