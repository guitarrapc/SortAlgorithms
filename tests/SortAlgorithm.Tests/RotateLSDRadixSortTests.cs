using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using System.Numerics;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class RotateLSDRadixSortTests : IntegerSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => RotateLSDRadixSort.Sort(span, context);

    // Distribution sort: digits are tested as integers, never through the comparer. Sorted input [1, n]
    // is not ordered by its lowest digit (it cycles 1, 2, 3, 0, ...), so the first pass has to rotate.
    protected override CountExpectation SortedInputCompares => CountExpectation.Zero;
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.NonZero;

    // The KeySelector overload carries satellite data, which makes stability observable:
    // these are the canonical stability tests, driven through SortBy(span, keySelector, context).

    [Test]
    [MethodDataSource(typeof(MockStabilityData), nameof(MockStabilityData.Generate))]
    public async Task StabilityTest(StabilityTestItem[] items)
    {
        var stats = new StatisticsContext();

        RotateLSDRadixSort.SortBy(items.AsSpan(), x => x.Value, stats);

        await Assert.That(items.Select(x => x.Value).ToArray()).IsEquivalentTo(MockStabilityData.Sorted, CollectionOrdering.Matching);

        var value1Indices = items.Where(x => x.Value == 1).Select(x => x.OriginalIndex).ToArray();
        var value2Indices = items.Where(x => x.Value == 2).Select(x => x.OriginalIndex).ToArray();
        var value3Indices = items.Where(x => x.Value == 3).Select(x => x.OriginalIndex).ToArray();

        await Assert.That(value1Indices).IsEquivalentTo(MockStabilityData.Sorted1, CollectionOrdering.Matching);
        await Assert.That(value2Indices).IsEquivalentTo(MockStabilityData.Sorted2, CollectionOrdering.Matching);
        await Assert.That(value3Indices).IsEquivalentTo(MockStabilityData.Sorted3, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockStabilityWithIdData), nameof(MockStabilityWithIdData.Generate))]
    public async Task StabilityTestWithComplex(StabilityTestItemWithId[] items)
    {
        var stats = new StatisticsContext();

        RotateLSDRadixSort.SortBy(items.AsSpan(), x => x.Key, stats);

        for (var i = 0; i < items.Length; i++)
        {
            await Assert.That(items[i].Key).IsEqualTo(MockStabilityWithIdData.Sorted[i].Key);
            await Assert.That(items[i].Id).IsEqualTo(MockStabilityWithIdData.Sorted[i].Id);
        }
    }

    [Test]
    [MethodDataSource(typeof(MockStabilityAllEqualsData), nameof(MockStabilityAllEqualsData.Generate))]
    public async Task StabilityTestWithAllEqual(StabilityTestItem[] items)
    {
        var stats = new StatisticsContext();

        RotateLSDRadixSort.SortBy(items.AsSpan(), x => x.Value, stats);

        foreach (var item in items) await Assert.That(item.Value).IsEqualTo(1);
        await Assert.That(items.Select(x => x.OriginalIndex).ToArray()).IsEquivalentTo(MockStabilityAllEqualsData.Sorted, CollectionOrdering.Matching);
    }

    [Test]
    public async Task KeySelectorNegativeKeysTest()
    {
        var records = new (int Key, string Name)[] { (3, "c"), (-5, "a"), (0, "b"), (-5, "a2"), (3, "c2"), (int.MinValue, "min"), (int.MaxValue, "max") };
        RotateLSDRadixSort.SortBy(records.AsSpan(), x => x.Key);

        await Assert.That(records.Select(x => x.Key).ToArray())
            .IsEquivalentTo([int.MinValue, -5, -5, 0, 3, 3, int.MaxValue], CollectionOrdering.Matching);
        await Assert.That(records.Select(x => x.Name).ToArray())
            .IsEquivalentTo(["min", "a", "a2", "b", "c", "c2", "max"], CollectionOrdering.Matching);
    }

    [Test]
    public async Task KeySelectorLargeInputTest()
    {
        // Many duplicates across several digit passes: every pass has to keep the order the previous ones built.
        var random = new Random(42);
        var records = Enumerable.Range(0, 1000).Select(i => (Key: random.Next(-10000, 10000), Index: i)).ToArray();
        var expected = records.OrderBy(x => x.Key).ThenBy(x => x.Index).ToArray();

        RotateLSDRadixSort.SortBy(records.AsSpan(), x => x.Key);

        await Assert.That(records).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task ExhaustiveSmallInputsAreSortedStablyTest()
    {
        // Every sequence up to a small length over a small alphabet, with each element's position as satellite data.
        // Alphabet 4 is one digit, so each sequence takes a single pass and exercises every shape the merge and the
        // rotation can see for those lengths: either run empty, either side of a rotation longer, both sides equal.
        // Alphabet 16 adds a second pass, which only sorts correctly if the first one was stable.
        var failures = new List<string>();
        CheckAllSequences(alphabet: 4, maxLength: 7, failures);
        CheckAllSequences(alphabet: 16, maxLength: 4, failures);

        await Assert.That(failures).IsEmpty();
    }

    private static void CheckAllSequences(int alphabet, int maxLength, List<string> failures)
    {
        for (var length = 0; length <= maxLength; length++)
        {
            var count = (int)Math.Pow(alphabet, length);
            var keys = new int[length];
            for (var code = 0; code < count; code++)
            {
                var c = code;
                for (var i = 0; i < length; i++)
                {
                    keys[i] = c % alphabet;
                    c /= alphabet;
                }

                var records = keys.Select((key, index) => (Key: key, Index: index)).ToArray();
                var expected = records.OrderBy(x => x.Key).ThenBy(x => x.Index).ToArray();
                RotateLSDRadixSort.SortBy(records.AsSpan(), x => x.Key);

                if (!records.SequenceEqual(expected) && failures.Count < 10)
                {
                    failures.Add($"[{string.Join(", ", keys)}] -> [{string.Join(", ", records)}]");
                }
            }
        }
    }

    // Full-control overload: a user-defined struct selector with a 64-bit key,
    // beyond the 32-bit limit of the Func<T, int> convenience overload.
    private readonly struct LongKeyRadixSelector : IRadixKeySelector<(long Key, int Index)>
    {
        public static int KeyBits => 64;
        public ulong GetKey((long Key, int Index) value) => (ulong)value.Key ^ 0x8000_0000_0000_0000;
    }

    [Test]
    public async Task CustomRadixKeySelectorTest()
    {
        var random = new Random(42);
        var records = Enumerable.Range(0, 1000).Select(i => (Key: random.NextInt64(long.MinValue, long.MaxValue), Index: i)).ToArray();
        var expected = records.OrderBy(x => x.Key).ThenBy(x => x.Index).ToArray();

        RotateLSDRadixSort.SortBy(records.AsSpan(), default(LongKeyRadixSelector));

        await Assert.That(records).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    private readonly struct TooWideRadixSelector : IRadixKeySelector<int>
    {
        public static int KeyBits => 128;
        public ulong GetKey(int value) => (ulong)value;
    }

    [Test]
    public async Task CustomSelectorKeyBitsOutOfRangeThrowsTest()
    {
        var array = new[] { 3, 1, 2 };
        Assert.Throws<NotSupportedException>(() => RotateLSDRadixSort.SortBy(array.AsSpan(), default(TooWideRadixSelector)));
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateHalf))]
    public async Task SortHalfResultOrderTest(IInputSample<Half> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        RotateLSDRadixSort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateFloat))]
    public async Task SortFloatResultOrderTest(IInputSample<float> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        RotateLSDRadixSort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateDouble))]
    public async Task SortDoubleResultOrderTest(IInputSample<double> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        RotateLSDRadixSort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    // ── Operation counts ────────────────────────────────────────────────────────────────────────────

    [Test]
    public async Task TheoreticalValuesTwoElementsTest()
    {
        // Traced by hand. Keys normalize to [0, 1]: one bit, one pass, one merge of two single-element runs.
        // Digit range [0, 4) splits at 2: both lower-bound searches probe once, and both elements are low, so the
        // rotation is empty. The low half [0, 2) splits at 1: two more probes, and the rotation is the only
        // element movement the sort can make — a swap when the input is reversed, nothing when it is not.
        var reversed = new[] { 1, 0 };
        var stats = new StatisticsContext();
        RotateLSDRadixSort.Sort(reversed.AsSpan(), stats);

        await Assert.That(reversed).IsEquivalentTo([0, 1], CollectionOrdering.Matching);
        await Assert.That(stats.IndexReadCount).IsEqualTo(2UL + 4UL + 2UL); // range scan + probes + one swap
        await Assert.That(stats.IndexWriteCount).IsEqualTo(2UL);
        await Assert.That(stats.SwapCount).IsEqualTo(1UL);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);

        var sorted = new[] { 0, 1 };
        stats = new StatisticsContext();
        RotateLSDRadixSort.Sort(sorted.AsSpan(), stats);

        await Assert.That(sorted).IsEquivalentTo([0, 1], CollectionOrdering.Matching);
        await Assert.That(stats.IndexReadCount).IsEqualTo(2UL + 4UL); // range scan + probes
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);
    }

    [Test]
    public async Task TheoreticalValuesThreeElementsReversedTest()
    {
        // Traced by hand. Keys normalize to [0, 2]: one pass. The merge sort splits [2 | 1 0].
        //   Merge [1] + [0]: 4 probes, 1 swap (as in the two-element case) -> [2 | 0 1].
        //   Merge [2] + [0 1] on [0, 4): split at 2 probes 1 + 1, rotates [2 | 0 1] (l = 1, r = 2) -> [0 1 | 2],
        //   which is 1 + 2 - gcd(1, 2) = 2 swaps. The low half is now [] + [0 1]: one run is empty, so it ends
        //   without probing — that early return is what this count pins down. The high half holds one element.
        var array = new[] { 2, 1, 0 };
        var stats = new StatisticsContext();
        RotateLSDRadixSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo([0, 1, 2], CollectionOrdering.Matching);
        await Assert.That(stats.SwapCount).IsEqualTo(3UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(6UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo(3UL + 6UL + 6UL); // range scan + probes + 3 swaps
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var values = Enumerable.Range(0, n).ToArray();
        await AssertOperationProfile(values, expectMovement: true);
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        var values = Enumerable.Range(0, n).Reverse().ToArray();
        await AssertOperationProfile(values, expectMovement: true);
    }

    [Test]
    [Arguments(10, 42)]
    [Arguments(10, 1234)]
    [Arguments(20, 42)]
    [Arguments(20, 1234)]
    [Arguments(50, 42)]
    [Arguments(50, 1234)]
    [Arguments(100, 42)]
    [Arguments(100, 1234)]
    public async Task TheoreticalValuesRandomTest(int n, int seed)
    {
        var values = TestHelpers.ShuffledRange(n, seed);
        await AssertOperationProfile(values, expectMovement: true);
    }

    [Test]
    [Arguments(100, 42, 10)]         // range 990        -> 10 bits ->  5 passes
    [Arguments(100, 42, 1_000)]      // range 99,000     -> 17 bits ->  9 passes
    [Arguments(100, 42, 1_000_000)]  // range 99,000,000 -> 27 bits -> 14 passes
    public async Task TheoreticalValuesWideRangeTest(int n, int seed, int stride)
    {
        // Scaling the values widens the key range without changing n, so the pass count is what varies here.
        var values = TestHelpers.ShuffledRangeScaled(n, seed, stride);
        await AssertOperationProfile(values, expectMovement: true);
    }

    public static IEnumerable<Func<int[]>> DigitOrderedInputs()
    {
        yield return () => [0, 0, 0, 1, 1, 2, 3, 3, 3];               // one digit, already in order: 1 pass
        yield return () => [0, 0, 16, 16, 16, 32, 48, 48];            // digits 0 and 1 shared, digit 2 in order: 3 passes
        yield return () => [-64, -64, -48, -32, -32, -16, -16];       // same shape below zero: normalized by the minimum
    }

    [Test]
    [MethodDataSource(nameof(DigitOrderedInputs))]
    public async Task TheoreticalValuesDigitOrderedInputMovesNothingTest(int[] values)
    {
        // The best case with a non-zero range: every pass finds its digit already in order, so each lower-bound search
        // lands on the boundary between the runs, every rotation is empty, and the sort is nothing but probes.
        var original = values.ToArray();
        var stats = await AssertOperationProfile(values, expectMovement: false);

        await Assert.That(values).IsEquivalentTo(original, CollectionOrdering.Matching);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
    }

    [Test]
    [Arguments(64, 42)]
    [Arguments(100, 1234)]
    public async Task TheoreticalValuesSharedDigitPassesMoveNothingTest(int n, int seed)
    {
        // Values in [0, 4) take one pass. The same values times 16 take three: two whose digit every element shares,
        // then one that sees exactly the digits the single pass saw. If a shared-digit pass moves nothing, both sorts
        // perform the same swaps, and the scaled one pays only for the extra passes' probes — at least two per merge.
        var random = new Random(seed);
        var small = Enumerable.Range(0, n).Select(_ => random.Next(4)).ToArray();
        var scaled = small.Select(v => v * 16).ToArray();

        var smallStats = new StatisticsContext();
        var scaledStats = new StatisticsContext();
        RotateLSDRadixSort.Sort(small.AsSpan(), smallStats);
        RotateLSDRadixSort.Sort(scaled.AsSpan(), scaledStats);

        await Assert.That(small).IsEquivalentTo(small.OrderBy(x => x), CollectionOrdering.Matching);
        await Assert.That(scaled).IsEquivalentTo(small.Select(v => v * 16), CollectionOrdering.Matching);
        await Assert.That(DigitCount(scaled)).IsEqualTo(DigitCount(small) + 2);

        await Assert.That(smallStats.SwapCount).IsGreaterThan(0UL);
        await Assert.That(scaledStats.SwapCount).IsEqualTo(smallStats.SwapCount);
        await Assert.That(scaledStats.IndexWriteCount).IsEqualTo(smallStats.IndexWriteCount);
        await Assert.That(scaledStats.IndexReadCount - smallStats.IndexReadCount).IsGreaterThanOrEqualTo((ulong)(2 * 2 * (n - 1)));
    }

    [Test]
    public async Task TheoreticalValuesAllEqualTest()
    {
        // Range 0: the scan is the whole sort.
        var values = Enumerable.Repeat(7, 50).ToArray();
        var stats = new StatisticsContext();
        RotateLSDRadixSort.Sort(values.AsSpan(), stats);

        await Assert.That(stats.IndexReadCount).IsEqualTo(50UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);
    }

    /// <summary>
    /// Sorts <paramref name="values"/> and checks the counts against the structure of the algorithm rather than against
    /// constants: every write belongs to a swap, every read is the range scan, a swap, or a binary-search probe, and the
    /// swaps and probes stay inside bounds that follow from the merge-sort tree, which depends on n alone.
    /// </summary>
    private static async Task<StatisticsContext> AssertOperationProfile(int[] values, bool expectMovement)
    {
        var n = values.Length;
        var expected = values.OrderBy(x => x).ToArray();
        var digitCount = DigitCount(values);
        var (merges, mergedElements, probeBound) = MergeTree(n);

        var stats = new StatisticsContext();
        RotateLSDRadixSort.Sort(values.AsSpan(), stats);

        await Assert.That(values).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);

        // In place: nothing is written except by a swap, and a swap writes two elements.
        await Assert.That(stats.IndexWriteCount).IsEqualTo(2 * stats.SwapCount);

        // A merge rotates within its own range on each of the two levels of the radix-4 bisection, and a block-swap
        // rotation of k elements costs fewer than k swaps, so a merge of L elements costs fewer than 2L swaps.
        if (expectMovement)
        {
            await Assert.That(stats.SwapCount).IsGreaterThan(0UL);
        }
        await Assert.That(stats.SwapCount).IsLessThan((ulong)(digitCount * 2 * mergedElements));

        // Whatever is not the range scan or a swap is a probe. The first split of every merge probes both runs, and
        // both are non-empty in a merge sort, so a pass probes at least twice per merge. At most three bisection nodes
        // run per merge, each with one lower-bound search per run, which bounds a pass by MergeTree's probe bound.
        var probes = stats.IndexReadCount - (ulong)n - 2 * stats.SwapCount;
        await Assert.That(probes).IsGreaterThanOrEqualTo((ulong)(digitCount * 2 * merges));
        await Assert.That(probes).IsLessThanOrEqualTo((ulong)(digitCount * probeBound));

        return stats;
    }

    /// <summary>Passes the sort runs for int keys: ⌈bitlength(max − min) / 2⌉ on the sign-flipped keys.</summary>
    private static int DigitCount(int[] values)
    {
        var keys = values.Select(v => (ulong)((uint)v ^ 0x8000_0000u)).ToArray();
        var range = keys.Max() - keys.Min();
        var requiredBits = 64 - BitOperations.LeadingZeroCount(range);
        return (requiredBits + 1) / 2;
    }

    /// <summary>
    /// Shape of one pass's merge sort, which splits [a, b) at a + (b − a) / 2 down to single elements regardless of the
    /// values: the number of merges, the total number of elements they merge, and an upper bound on their probes —
    /// three bisection nodes per merge, each searching the two runs with at most ⌈log₂(k + 1)⌉ probes for a run of k.
    /// </summary>
    private static (long Merges, long MergedElements, long ProbeBound) MergeTree(int n)
    {
        long merges = 0, mergedElements = 0, probeBound = 0;
        Walk(0, n);
        return (merges, mergedElements, probeBound);

        void Walk(int a, int b)
        {
            if (b - a < 2) return;
            var m = a + (b - a) / 2;
            Walk(a, m);
            Walk(m, b);
            merges++;
            mergedElements += b - a;
            probeBound += 3 * (MaxProbes(m - a) + MaxProbes(b - m));
        }

        static int MaxProbes(int k) => 32 - BitOperations.LeadingZeroCount((uint)k); // ⌈log₂(k + 1)⌉
    }

    // ── Observation ─────────────────────────────────────────────────────────────────────────────────

    [Test]
    public async Task EveryOperationIsAnInPlaceSwapOrReadTest()
    {
        // In place means the main span is the only storage: every read and swap names buffer 0, and no element is
        // written or copied other than by a swap.
        var values = TestHelpers.ShuffledRangeScaled(200, 42, 37);
        var buffers = new HashSet<int>();
        var directWrites = 0;
        var rangeCopies = 0;

        RotateLSDRadixSort.Sort(values.AsSpan(), new VisualizationContext(
            onSwap: (_, _, buffer) => buffers.Add(buffer),
            onIndexRead: (_, buffer) => buffers.Add(buffer),
            onIndexWrite: (_, _, _) => directWrites++,
            onRangeCopy: (_, _, _, _, _, _) => rangeCopies++));

        await Assert.That(values).IsEquivalentTo(values.OrderBy(x => x), CollectionOrdering.Matching);
        await Assert.That(buffers).IsEquivalentTo([0]);
        await Assert.That(directWrites).IsEqualTo(0);
        await Assert.That(rangeCopies).IsEqualTo(0);
    }

    [Test]
    [Arguments(100, 1)]          // [-50, 50): straddles zero, range 99 -> 7 bits -> 4 passes, not the 16 the key width would give
    [Arguments(100, 1_000)]      // range 99,000 -> 17 bits -> 9 passes
    public async Task PassCountFollowsKeyRangeTest(int n, int stride)
    {
        var values = Enumerable.Range(-n / 2, n).Select(v => v * stride).ToArray();
        new Random(42).Shuffle(values);

        var phases = new List<(SortPhase Phase, int Digit, int Total)>();
        RotateLSDRadixSort.Sort(values.AsSpan(), new VisualizationContext(
            onPhase: (phase, p1, p2, _) => phases.Add((phase, p1, p2))));

        var requiredBits = 64 - BitOperations.LeadingZeroCount((ulong)((n - 1) * stride));
        var digitCount = (requiredBits + 1) / 2;

        await Assert.That(values).IsEquivalentTo(values.OrderBy(x => x), CollectionOrdering.Matching);
        await Assert.That(phases[0].Phase).IsEqualTo(SortPhase.KeyRangeScan);
        await Assert.That(phases.Skip(1).ToArray()).IsEquivalentTo(
            Enumerable.Range(0, digitCount).Select(d => (SortPhase.RadixPass, d, digitCount)).ToArray(),
            CollectionOrdering.Matching);
    }
}
