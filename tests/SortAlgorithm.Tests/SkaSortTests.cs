using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class SkaSortTests : IntegerSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => SkaSort.Sort(span, context);

    // SkaSort is UNSTABLE: exchanges into buckets and std::sort may reorder equal keys,
    // so keySelector tests assert key order and permutation integrity only.

    [Test]
    public async Task KeySelectorSortsByKeyTest()
    {
        var random = new Random(42);
        var records = Enumerable.Range(0, 2000).Select(i => (Key: random.Next(-10000, 10000), Index: i)).ToArray();

        SkaSort.SortBy(records.AsSpan(), x => x.Key);

        var keys = records.Select(x => x.Key).ToArray();
        await Assert.That(keys).IsEquivalentTo(keys.Order().ToArray(), CollectionOrdering.Matching);
        await Assert.That(records.Select(x => x.Index).Order().ToArray())
            .IsEquivalentTo(Enumerable.Range(0, 2000).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task KeySelectorNegativeKeysTest()
    {
        // Seven records go straight to std::sort, which compares the keys.
        var records = new (int Key, string Name)[] { (3, "c"), (-5, "a"), (0, "b"), (-5, "a2"), (3, "c2"), (int.MinValue, "min"), (int.MaxValue, "max") };
        SkaSort.SortBy(records.AsSpan(), x => x.Key);

        await Assert.That(records.Select(x => x.Key).ToArray())
            .IsEquivalentTo([int.MinValue, -5, -5, 0, 3, 3, int.MaxValue], CollectionOrdering.Matching);
    }

    /// <summary>12 significant bits make two digit levels; the top one holds only four of them.</summary>
    readonly struct TwelveBitKey : IRadixKeySelector<int>
    {
        public static int KeyBits => 12;
        public ulong GetKey(int value) => (ulong)value;
    }

    [Test]
    [Arguments(100)]
    [Arguments(1000)]
    [Arguments(5000)]
    public async Task KeyWidthThatIsNotAWholeNumberOfBytesTest(int n)
    {
        var random = new Random(42);
        var array = Enumerable.Range(0, n).Select(_ => random.Next(0, 4096)).ToArray();
        var expected = array.Order().ToArray();

        var totals = new HashSet<int>();
        SkaSort.SortBy(array.AsSpan(), new TwelveBitKey(), new VisualizationContext(onPhase: (phase, _, p2, _) =>
        {
            if (phase == SortPhase.RadixPass) totals.Add(p2);
        }));

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
        if (n >= 128) await Assert.That(totals.ToArray()).IsEquivalentTo([2]);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateHalf))]
    public async Task SortHalfResultOrderTest(IInputSample<Half> inputSample)
    {
        var array = inputSample.Samples.ToArray();
        SkaSort.Sort(array.AsSpan(), new StatisticsContext());

        // NaN-first total order, same as Array.Sort
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateFloat))]
    public async Task SortFloatResultOrderTest(IInputSample<float> inputSample)
    {
        var array = inputSample.Samples.ToArray();
        SkaSort.Sort(array.AsSpan(), new StatisticsContext());

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateDouble))]
    public async Task SortDoubleResultOrderTest(IInputSample<double> inputSample)
    {
        var array = inputSample.Samples.ToArray();
        SkaSort.Sort(array.AsSpan(), new StatisticsContext());

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    static readonly string[] Patterns = ["random", "range1000", "distinct16", "sorted", "reversed", "equal", "pipeorgan"];

    static int[] Pattern(string pattern, int n, int seed)
    {
        var random = new Random(seed);
        return pattern switch
        {
            "random" => Enumerable.Range(0, n).Select(_ => random.Next(int.MinValue, int.MaxValue)).ToArray(),
            "range1000" => Enumerable.Range(0, n).Select(_ => random.Next(0, 1000)).ToArray(),
            "distinct16" => Enumerable.Range(0, n).Select(_ => random.Next(0, 16) * 1_000_003 - 8_000_000).ToArray(),
            "sorted" => Enumerable.Range(0, n).ToArray(),
            "reversed" => Enumerable.Range(0, n).Reverse().ToArray(),
            "equal" => Enumerable.Repeat(-7, n).ToArray(),
            _ => Enumerable.Range(0, n).Select(i => Math.Min(i, n - 1 - i)).ToArray(),
        };
    }

    /// <summary>Each size sits on one side of a threshold: 128 (std::sort), 1,024 (rounds), and StdSort's own 6 and 24.</summary>
    [Test]
    [Arguments(2)]
    [Arguments(5)]
    [Arguments(6)]
    [Arguments(23)]
    [Arguments(24)]
    [Arguments(127)]
    [Arguments(128)]
    [Arguments(129)]
    [Arguments(1023)]
    [Arguments(1024)]
    [Arguments(1025)]
    [Arguments(5000)]
    [Arguments(70000)]
    public async Task SortsEveryPatternAcrossTheThresholds(int n)
    {
        foreach (var pattern in Patterns)
        {
            var array = Pattern(pattern, n, n);
            var expected = array.Order().ToArray();

            SkaSort.Sort(array.AsSpan());

            await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"{pattern} n={n}");
        }
    }

    [Test]
    [Arguments(129)]
    [Arguments(1024)]
    [Arguments(70000)]
    public async Task SortsLongKeysAcrossEveryByte(int n)
    {
        var random = new Random(n);
        var array = Enumerable.Range(0, n).Select(_ => random.NextInt64(long.MinValue, long.MaxValue)).ToArray();
        array[0] = long.MinValue;
        array[1] = long.MaxValue;
        var expected = array.Order().ToArray();

        SkaSort.Sort(array.AsSpan(), new StatisticsContext());

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    /// <summary>
    /// The entry decides by size alone: below 128 the whole input goes to std::sort, below 1,024 the first level is American flag
    /// sort, and from 1,024 on it runs in rounds.
    /// </summary>
    [Test]
    [Arguments(2, SortPhase.SkaSortStdSort)]
    [Arguments(127, SortPhase.SkaSortStdSort)]
    [Arguments(128, SortPhase.SkaSortAmericanFlag)]
    [Arguments(1023, SortPhase.SkaSortAmericanFlag)]
    [Arguments(1024, SortPhase.SkaSortRound)]
    [Arguments(5000, SortPhase.SkaSortRound)]
    public async Task FirstStepFollowsTheSizeThresholds(int n, SortPhase expected)
    {
        var array = Pattern("random", n, 42);
        var steps = new List<(SortPhase Phase, int P1, int P2, int P3)>();
        var radixPasses = 0;

        SkaSort.Sort(array.AsSpan(), new VisualizationContext(onPhase: (phase, p1, p2, p3) =>
        {
            if (phase is SortPhase.SkaSortStdSort or SortPhase.SkaSortAmericanFlag or SortPhase.SkaSortRound)
                steps.Add((phase, p1, p2, p3));
            else if (phase == SortPhase.RadixPass)
                radixPasses++;
        }));

        await Assert.That(IsSorted(array)).IsTrue();
        await Assert.That(steps[0].Phase).IsEqualTo(expected);
        await Assert.That(steps[0].P1).IsEqualTo(0);
        await Assert.That(steps[0].P2).IsEqualTo(n - 1);
        if (expected == SortPhase.SkaSortStdSort)
        {
            await Assert.That(steps.Count).IsEqualTo(1);
            await Assert.That(steps[0].P3).IsEqualTo(128);
            await Assert.That(radixPasses).IsEqualTo(0);
        }
    }

    /// <summary>
    /// The key range is not measured, so every byte costs a counting pass even when every key shares it. Equal keys move nothing.
    /// </summary>
    [Test]
    [Arguments(1000)] // American flag sort at every level
    [Arguments(5000)] // rounds at every level
    public async Task EqualKeysCountEveryByteOfTheKey(int n)
    {
        var array = Enumerable.Repeat(42, n).ToArray();

        var digits = new List<int>();
        SkaSort.Sort(array.AsSpan(), new VisualizationContext(onPhase: (phase, p1, _, _) =>
        {
            if (phase == SortPhase.RadixPass) digits.Add(p1);
        }));
        await Assert.That(digits).IsEquivalentTo([3, 2, 1, 0], CollectionOrdering.Matching);

        var stats = new StatisticsContext();
        SkaSort.Sort(array.AsSpan(), stats);

        await Assert.That(stats.IndexReadCount).IsEqualTo((ulong)(4 * n));
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);
    }

    /// <summary>
    /// A round exchanges every element it looks at, with itself when it is home, so sorted input still swaps each element once.
    /// American flag sort leaves an element of the bucket being filled where it is. Four copies of each byte value, one level.
    /// </summary>
    [Test]
    public async Task RoundExchangesEveryElementItLooksAt()
    {
        var rounds = Enumerable.Range(0, 1024).Select(i => (byte)(i / 4)).ToArray();
        var stats = new StatisticsContext();
        SkaSort.Sort(rounds.AsSpan(), stats);

        await Assert.That(IsSorted(rounds)).IsTrue();
        await Assert.That(stats.SwapCount).IsEqualTo(1024UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(2048UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo(4096UL); // count, one read per slot, two per exchange
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);

        // 255 byte values: American flag sort reads every slot up to the last bucket, which it does not walk
        var flag = Enumerable.Range(0, 1020).Select(i => (byte)(i / 4)).ToArray();
        stats = new StatisticsContext();
        SkaSort.Sort(flag.AsSpan(), stats);

        await Assert.That(IsSorted(flag)).IsTrue();
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo((ulong)(1020 + 1016));
    }

    /// <summary>The list of unfinished buckets never grows, and the first round starts with every occupied bucket.</summary>
    [Test]
    [Arguments(4096, 42)]
    [Arguments(50000, 1234)]
    public async Task RoundsShrinkTheirList(int n, int seed)
    {
        var random = new Random(seed);
        var array = Enumerable.Range(0, n).Select(_ => (byte)random.Next(256)).ToArray();
        var occupied = array.Distinct().Count();

        var listed = new List<int>();
        SkaSort.Sort(array.AsSpan(), new VisualizationContext(onPhase: (phase, p1, p2, p3) =>
        {
            if (phase == SortPhase.SkaSortRound) listed.Add(p3);
        }));

        await Assert.That(IsSorted(array)).IsTrue();
        await Assert.That(listed.Count).IsGreaterThanOrEqualTo(2);
        await Assert.That(listed[0]).IsEqualTo(occupied);
        for (var i = 1; i < listed.Count; i++)
            await Assert.That(listed[i]).IsLessThanOrEqualTo(listed[i - 1]);
        await Assert.That(listed[^1]).IsGreaterThan(1);
    }

    /// <summary>
    /// Every range handed to std::sort has 2 to 127 elements, no two overlap, and nothing outside them is compared. Keys that differ
    /// only in their last byte, and groups of 128 or more equal keys, reach the last byte and are never compared.
    /// </summary>
    [Test]
    [Arguments(50000, "random", true)]
    [Arguments(50000, "range1000", false)]
    [Arguments(3000, "distinct16", false)]
    public async Task OnlyStdSortRangesCompare(int n, string pattern, bool expectStdSort)
    {
        var array = Pattern(pattern, n, 42);
        var ranges = new List<(int First, int Last)>();
        var inStdSort = false;
        var comparesOutside = 0;

        SkaSort.Sort(array.AsSpan(), new VisualizationContext(
            onCompare: (_, _, _, _, _) => { if (!inStdSort) comparesOutside++; },
            onPhase: (phase, p1, p2, p3) =>
            {
                switch (phase)
                {
                    case SortPhase.SkaSortStdSort:
                        inStdSort = true;
                        ranges.Add((p1, p2));
                        break;
                    case SortPhase.RadixPass or SortPhase.DistributionBucket or SortPhase.SkaSortAmericanFlag or SortPhase.SkaSortRound:
                        inStdSort = false;
                        break;
                }
            }));

        await Assert.That(IsSorted(array)).IsTrue();
        await Assert.That(comparesOutside).IsEqualTo(0);
        await Assert.That(ranges.Count > 0).IsEqualTo(expectStdSort);
        await Assert.That(ranges.All(r => r.Last - r.First + 1 is >= 2 and < 128)).IsTrue();

        var ordered = ranges.OrderBy(r => r.First).ToArray();
        for (var i = 1; i < ordered.Length; i++)
            await Assert.That(ordered[i].First).IsGreaterThan(ordered[i - 1].Last);
    }

    /// <summary>Nothing follows the last byte: a one-byte key is distributed once and never compared.</summary>
    [Test]
    [Arguments(200)]
    [Arguments(5000)]
    public async Task LastByteEndsTheSortWithoutComparisons(int n)
    {
        var random = new Random(42);
        var array = Enumerable.Range(0, n).Select(_ => (sbyte)random.Next(-128, 128)).ToArray();
        var expected = array.Order().ToArray();
        var observed = array.ToArray();

        var stdSorts = 0;
        var stats = new StatisticsContext();
        SkaSort.Sort(array.AsSpan(), stats);
        SkaSort.Sort(observed.AsSpan(), new VisualizationContext(onPhase: (phase, _, _, _) =>
        {
            if (phase == SortPhase.SkaSortStdSort) stdSorts++;
        }));

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);
        await Assert.That(stdSorts).IsEqualTo(0);
    }
}
