using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class BitonicSortAnyLengthSimdTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => BitonicSortAnyLengthSimd.Sort(span, context);

    // Every comparator writes both of its positions instead of exchanging them, so no input swaps.
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    static int Lanes<T>() where T : IComparable<T> => BitonicSortAnyLengthSimd.LaneCount<T, ComparableComparer<T>>();

    [Test]
    public async Task SameComparisonsAsTheScalarNetworkOnEveryVectorizedType()
    {
        List<string> problems =
        [
            .. Check(x => (int)x), .. Check(x => (uint)x), .. Check(x => x), .. Check(x => (ulong)x),
            .. Check(x => (nint)x), .. Check(x => (nuint)x),
        ];

        await Assert.That(problems).IsEmpty();

        static List<string> Check<T>(Func<long, T> convert) where T : IComparable<T>
        {
            // Every length up to several blocks of 512-bit int vectors, and lengths around powers of two beyond them.
            int[] sizes = [.. Enumerable.Range(0, 300), 511, 512, 513, 1000, 1023, 1024, 1025, 4097];
            var random = new Random(42);
            var problems = new List<string>();
            foreach (var n in sizes)
            {
                for (var pattern = 0; pattern < 6; pattern++)
                {
                    var source = new T[n];
                    for (var i = 0; i < n; i++)
                    {
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
                    var scalar = new StatisticsContext();
                    BitonicSortAnyLength.Sort(((T[])source.Clone()).AsSpan(), scalar);
                    var observed = (T[])source.Clone();
                    var stats = new StatisticsContext();
                    BitonicSortAnyLengthSimd.Sort(observed.AsSpan(), stats);
                    var unobserved = (T[])source.Clone();
                    BitonicSortAnyLengthSimd.Sort(unobserved.AsSpan());

                    if (!observed.SequenceEqual(expected) || !unobserved.SequenceEqual(expected) ||
                        stats.CompareCount != scalar.CompareCount || stats.SwapCount != 0)
                    {
                        problems.Add($"{typeof(T).Name}: n = {n}, pattern {pattern}");
                    }
                }
            }
            return problems;
        }
    }

    [Test]
    public async Task OneBlockIsReadAndWrittenOnce()
    {
        var w = Lanes<int>();
        Skip.When(w == 1, "No vector width is accelerated on this machine.");

        var stats = new StatisticsContext();
        BitonicSortAnyLengthSimd.Sort(TestHelpers.ShuffledRange(w, 3).ToArray().AsSpan(), stats);

        // Sorting exactly W elements is one block: W reads, W / 2 comparisons in each of log₂ W (log₂ W + 1) / 2 stages, W writes.
        var log = (int)Math.Log2(w);
        await Assert.That(stats.IndexReadCount).IsEqualTo((ulong)w);
        await Assert.That(stats.IndexWriteCount).IsEqualTo((ulong)w);
        await Assert.That(stats.CompareCount).IsEqualTo((ulong)(w / 2 * log * (log + 1) / 2));
    }

    [Test]
    public async Task TwoBlocksAndOneComparatorRun()
    {
        var w = Lanes<int>();
        Skip.When(w == 1, "No vector width is accelerated on this machine.");

        var stats = new StatisticsContext();
        BitonicSortAnyLengthSimd.Sort(TestHelpers.ShuffledRange(2 * w, 3).ToArray().AsSpan(), stats);

        // 2W elements: each half is sorted as one block, the merge compares (i, i + W) as one vector pair and then merges each half as one
        // block. The blocks read and write W elements each, the pair 2W.
        var log = (int)Math.Log2(w);
        var sortBlock = w / 2 * log * (log + 1) / 2;
        var mergeBlock = w / 2 * log;
        await Assert.That(stats.IndexReadCount).IsEqualTo((ulong)(6 * w));
        await Assert.That(stats.IndexWriteCount).IsEqualTo((ulong)(6 * w));
        await Assert.That(stats.CompareCount).IsEqualTo((ulong)(2 * sortBlock + w + 2 * mergeBlock));
    }

    [Test]
    [Arguments(17)]
    [Arguments(100)]
    [Arguments(129)]
    public async Task CustomComparerRunsTheScalarComparatorsInLangsOrder(int n)
    {
        var input = TestHelpers.ShuffledRange(n, 11);
        var expected = new BitonicSortTests.NetworkEventRecorder();
        BitonicSortAnyLength.Sort(input.ToArray().AsSpan(), expected);
        var actual = new BitonicSortTests.NetworkEventRecorder();
        var array = input.ToArray();
        BitonicSortAnyLengthSimd.Sort(array.AsSpan(), new AscendingInt(), actual);
        var stats = new StatisticsContext();
        BitonicSortAnyLengthSimd.Sort(input.ToArray().AsSpan(), new AscendingInt(), stats);

        // One comparator at a time: the same comparisons in the same order, each writing both positions and none swapping.
        await Assert.That(BitonicSortAnyLengthSimd.LaneCount<int, AscendingInt>()).IsEqualTo(1);
        await Assert.That(actual.Events).IsEquivalentTo(expected.Events.Where(e => e.StartsWith("compare", StringComparison.Ordinal)).ToList(), CollectionOrdering.Matching);
        await Assert.That(stats.IndexReadCount).IsEqualTo(2 * stats.CompareCount);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(2 * stats.CompareCount);
        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
    }

    readonly struct AscendingInt : IComparer<int>
    {
        public int Compare(int x, int y) => x.CompareTo(y);
    }
}
