using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using System.Numerics;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class RadsortTests : IntegerSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => Radsort.Sort(span, context);

    // Distribution sort: digits are tested as integers, never through the comparer. Every pass writes every element
    // into an output block whatever the order, so sorted input is written too; nothing is ever exchanged.
    protected override CountExpectation SortedInputCompares => CountExpectation.Zero;
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    private const int Sigma = 256;

    // The KeySelector overload carries satellite data, which makes stability observable:
    // these are the canonical stability tests, driven through SortBy(span, keySelector, context).

    [Test]
    [MethodDataSource(typeof(MockStabilityData), nameof(MockStabilityData.Generate))]
    public async Task StabilityTest(StabilityTestItem[] items)
    {
        var stats = new StatisticsContext();

        Radsort.SortBy(items.AsSpan(), x => x.Value, stats);

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

        Radsort.SortBy(items.AsSpan(), x => x.Key, stats);

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

        Radsort.SortBy(items.AsSpan(), x => x.Value, stats);

        foreach (var item in items) await Assert.That(item.Value).IsEqualTo(1);
        await Assert.That(items.Select(x => x.OriginalIndex).ToArray()).IsEquivalentTo(MockStabilityAllEqualsData.Sorted, CollectionOrdering.Matching);
    }

    [Test]
    public async Task KeySelectorNegativeKeysTest()
    {
        var records = new (int Key, string Name)[] { (3, "c"), (-5, "a"), (0, "b"), (-5, "a2"), (3, "c2"), (int.MinValue, "min"), (int.MaxValue, "max") };
        Radsort.SortBy(records.AsSpan(), x => x.Key);

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
        var records = Enumerable.Range(0, 5000).Select(i => (Key: random.Next(-100000, 100000), Index: i)).ToArray();
        var expected = records.OrderBy(x => x.Key).ThenBy(x => x.Index).ToArray();

        Radsort.SortBy(records.AsSpan(), x => x.Key);

        await Assert.That(records).IsEquivalentTo(expected, CollectionOrdering.Matching);
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
        var records = Enumerable.Range(0, 3000).Select(i => (Key: random.NextInt64(long.MinValue, long.MaxValue), Index: i)).ToArray();
        var expected = records.OrderBy(x => x.Key).ThenBy(x => x.Index).ToArray();

        Radsort.SortBy(records.AsSpan(), default(LongKeyRadixSelector));

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
        Assert.Throws<NotSupportedException>(() => Radsort.SortBy(array.AsSpan(), default(TooWideRadixSelector)));
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateHalf))]
    public async Task SortHalfResultOrderTest(IInputSample<Half> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        Radsort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateFloat))]
    public async Task SortFloatResultOrderTest(IInputSample<float> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        Radsort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateDouble))]
    public async Task SortDoubleResultOrderTest(IInputSample<double> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        Radsort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    // ── Block structure ─────────────────────────────────────────────────────────────────────────────
    //
    // The algorithm is correct for any block size b; the size policy only picks one from n. Driving the block size
    // directly reaches the shapes the policy never produces for a given n, and each shape takes a different path:
    //   n < b          every element starts in the scratch tail block, A has no blocks, the finalization only copies
    //   n mod b == 0   the tail partial block is empty
    //   n mod b != 0   the tail partial block holds elements
    //   b == 1         every write fills a block, so every partial block is empty
    //   shared digit   every element falls into one bucket, which then fills on the last element of each full input
    //                  block and draws the block it is reading (i_out == i_in, the edge of Lemma 1)

    private readonly struct RecordKey : IRadixKeySelector<(int Key, int Index)>
    {
        public static int KeyBits => 32;
        public ulong GetKey((int Key, int Index) value) => (uint)value.Key ^ 0x8000_0000u;
    }

    private static (int Key, int Index)[] Records(int[] keys)
        => keys.Select((key, index) => (Key: key, Index: index)).ToArray();

    private static (int Key, int Index)[] StableOrder((int Key, int Index)[] records)
        => records.OrderBy(x => x.Key).ThenBy(x => x.Index).ToArray();

    /// <summary>Key distributions, each sending the elements through the buckets differently.</summary>
    private static IEnumerable<(string Name, int[] Keys)> Distributions(int n, int seed)
    {
        var random = new Random(seed);
        yield return ("full range", Enumerable.Range(0, n).Select(_ => random.Next(int.MinValue, int.MaxValue)).ToArray());
        yield return ("few distinct", Enumerable.Range(0, n).Select(_ => random.Next(5) * 65_537).ToArray());
        yield return ("shared low digit", Enumerable.Range(0, n).Select(_ => random.Next(1000) * Sigma).ToArray());
        yield return ("ascending", Enumerable.Range(0, n).Select(i => i * 37).ToArray());
        yield return ("descending", Enumerable.Range(0, n).Select(i => (n - i) * 37).ToArray());
        yield return ("one outlier", Enumerable.Range(0, n).Select(i => i == n / 2 ? 1 : 0).ToArray());
    }

    /// <summary>Lengths around the block boundaries and around σ blocks, where the head start and the scratch area turn over.</summary>
    private static IEnumerable<int> LengthsAround(int blockSize)
        => new[]
        {
            2, 3, blockSize - 1, blockSize, blockSize + 1, 2 * blockSize, 3 * blockSize + 1,
            Sigma * blockSize - 1, Sigma * blockSize + 5, 2 * Sigma * blockSize + 3,
        }
        .Where(n => n >= 2)
        .Distinct();

    [Test]
    [Arguments(0)]
    [Arguments(1)]
    [Arguments(3)]
    [Arguments(6)]
    public async Task ExplicitBlockSizesSortStablyTest(int blockShift)
    {
        var failures = new List<string>();
        foreach (var n in LengthsAround(1 << blockShift))
        {
            foreach (var (name, keys) in Distributions(n, seed: n))
            {
                var records = Records(keys);
                var expected = StableOrder(records);
                Radsort.SortWithBlockShift(records.AsSpan(), default(RecordKey), blockShift, NullContext.Default);
                if (!records.SequenceEqual(expected)) failures.Add($"b = {1 << blockShift}, n = {n}, {name}");
            }
        }

        await Assert.That(failures).IsEmpty();
    }

    [Test]
    public async Task ExhaustiveSmallInputsAreSortedStablyTest()
    {
        // Every sequence up to length 7 over {0, 1, 256, 257}: two digits with two values each, so the second pass
        // only sorts correctly if the first one was stable. Block sizes 1 to 8 cover every length both below and above b,
        // with and without a partial tail.
        int[] alphabet = [0, 1, 256, 257];
        var failures = new List<string>();

        for (var blockShift = 0; blockShift <= 3; blockShift++)
        {
            for (var length = 0; length <= 7; length++)
            {
                var count = (int)Math.Pow(alphabet.Length, length);
                var keys = new int[length];
                for (var code = 0; code < count; code++)
                {
                    var c = code;
                    for (var i = 0; i < length; i++)
                    {
                        keys[i] = alphabet[c % alphabet.Length];
                        c /= alphabet.Length;
                    }

                    var records = Records(keys);
                    var expected = StableOrder(records);
                    Radsort.SortWithBlockShift(records.AsSpan(), default(RecordKey), blockShift, NullContext.Default);

                    if (!records.SequenceEqual(expected) && failures.Count < 10)
                    {
                        failures.Add($"b = {1 << blockShift}: [{string.Join(", ", keys)}] -> [{string.Join(", ", records)}]");
                    }
                }
            }
        }

        await Assert.That(failures).IsEmpty();
    }

    [Test]
    public async Task BlockShiftOutOfRangeThrowsTest()
    {
        var records = Records([3, 1, 2]);
        Assert.Throws<ArgumentOutOfRangeException>(() => Radsort.SortWithBlockShift(records.AsSpan(), default(RecordKey), -1, NullContext.Default));
        Assert.Throws<ArgumentOutOfRangeException>(() => Radsort.SortWithBlockShift(records.AsSpan(), default(RecordKey), Radsort.MaxBlockShift + 1, NullContext.Default));
    }

    // ── Block size policy ───────────────────────────────────────────────────────────────────────────

    [Test]
    [Arguments(2, 0)]
    [Arguments(454, 0)]         // the last length at which b = 1 is cheapest for 4-byte elements
    [Arguments(455, 1)]
    [Arguments(1024, 1)]
    [Arguments(8192, 3)]
    [Arguments(1 << 20, 6)]
    public async Task BlockSizeForIntElementsTest(int n, int expectedShift)
    {
        // Scratch 2σ × b × 4 bytes against 9 bytes of π, π′ and U per block: the minimum is the power of two nearest
        // √(9n / 2048) ≈ √n / 15, ties going to the larger block.
        await Assert.That(Radsort.BlockShift(n, sizeof(int))).IsEqualTo(expectedShift);
    }

    [Test]
    public async Task BlockSizeGrowsAsSquareRootTest()
    {
        // b ∈ Θ(√n): quadrupling n doubles b. For 4-byte elements b = √n / 16 at every even power of two shown here.
        for (var k = 12; k <= 30; k += 2)
        {
            await Assert.That(Radsort.BlockShift(1 << k, sizeof(int))).IsEqualTo(k / 2 - 4);
        }
    }

    [Test]
    public async Task BlockSizeShrinksForLargerElementsTest()
    {
        // The scratch term scales with the element size and the bookkeeping does not, so larger elements get smaller
        // blocks: each fourfold growth of the element halves b.
        var n = 1 << 20;
        await Assert.That(Radsort.BlockShift(n, 1)).IsEqualTo(Radsort.BlockShift(n, 4) + 1);
        await Assert.That(Radsort.BlockShift(n, 16)).IsEqualTo(Radsort.BlockShift(n, 4) - 1);
        await Assert.That(Radsort.BlockShift(n, 64)).IsEqualTo(Radsort.BlockShift(n, 4) - 2);
    }

    // ── Operation counts ────────────────────────────────────────────────────────────────────────────

    [Test]
    public async Task TheoreticalValuesTwoElementsTest()
    {
        // Traced by hand. n = 2 gives b = 1 and an empty tail; keys normalize to [0, 1], so one pass.
        // Setup: logical 256, 257 are A's blocks (physical 512, 513), logical 258 is the empty tail (scratch 256).
        // Pass: both elements are read once and written once into their buckets' head-start blocks (scratch 0 and 1),
        // and each write fills its block, so the buckets draw A's two blocks as their next — empty — blocks.
        var reversed = new[] { 1, 0 };
        var stats = new StatisticsContext();
        Radsort.Sort(reversed.AsSpan(), stats);

        // Reversed: bucket 1 holds logical 258 (scratch 1, the 1) and its empty partial 259 (physical 512), bucket 0
        // logical 256 (scratch 0, the 0) and 257 (physical 513). Filling A[0] finds 259 in the way and pushes it
        // (1 copy), then pulls 256; A[1]'s block is 257 itself and empty, so nothing moves; 258 is then pulled from
        // scratch. Three copied elements.
        await Assert.That(reversed).IsEquivalentTo([0, 1], CollectionOrdering.Matching);
        await Assert.That(stats.IndexReadCount).IsEqualTo(2UL + 2UL + 3UL); // range scan + pass + finalization
        await Assert.That(stats.IndexWriteCount).IsEqualTo(2UL + 3UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);

        // Sorted: bucket 0's empty partial now sits on A[0] and bucket 1's on A[1], so both are pushed before their
        // places are filled: four copied elements. The algorithm does not look at whether its input was ordered.
        var sorted = new[] { 0, 1 };
        stats = new StatisticsContext();
        Radsort.Sort(sorted.AsSpan(), stats);

        await Assert.That(sorted).IsEquivalentTo([0, 1], CollectionOrdering.Matching);
        await Assert.That(stats.IndexReadCount).IsEqualTo(2UL + 2UL + 4UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(2UL + 4UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);
    }

    [Test]
    public async Task TheoreticalValuesAllEqualTest()
    {
        // Range 0: the scan is the whole sort.
        var values = Enumerable.Repeat(7, 50).ToArray();
        var stats = new StatisticsContext();
        Radsort.Sort(values.AsSpan(), stats);

        await Assert.That(stats.IndexReadCount).IsEqualTo(50UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);
    }

    [Test]
    [Arguments(10)]
    [Arguments(100)]
    [Arguments(1000)]
    [Arguments(5000)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        await AssertOperationProfile(Enumerable.Range(0, n).ToArray());
    }

    [Test]
    [Arguments(10)]
    [Arguments(100)]
    [Arguments(1000)]
    [Arguments(5000)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        await AssertOperationProfile(Enumerable.Range(0, n).Reverse().ToArray());
    }

    [Test]
    [Arguments(10, 42)]
    [Arguments(10, 1234)]
    [Arguments(100, 42)]
    [Arguments(100, 1234)]
    [Arguments(1000, 42)]
    [Arguments(1000, 1234)]
    [Arguments(5000, 42)]
    [Arguments(5000, 1234)]
    public async Task TheoreticalValuesRandomTest(int n, int seed)
    {
        await AssertOperationProfile(TestHelpers.ShuffledRange(n, seed));
    }

    [Test]
    [Arguments(1000, 42, 1_000)]       // range 999,000       -> 20 bits -> 3 passes
    [Arguments(1000, 42, 1_000_000)]   // range 999,000,000   -> 30 bits -> 4 passes
    public async Task TheoreticalValuesWideRangeTest(int n, int seed, int stride)
    {
        // Scaling the values widens the key range without changing n, so the pass count is what varies here.
        await AssertOperationProfile(TestHelpers.ShuffledRangeScaled(n, seed, stride));
    }

    [Test]
    [Arguments(1000, 42)]
    [Arguments(1000, 1234)]
    public async Task TheoreticalValuesSharedDigitPassesStillMoveEverythingTest(int n, int seed)
    {
        // Unlike RadixLSD256Sort, Radsort cannot skip a pass whose digit every element shares: it has no counting pass to
        // find out. Multiples of 256 share digit 0, and that pass still reads and writes every element once.
        var values = TestHelpers.ShuffledRangeScaled(n, seed, Sigma);
        await AssertOperationProfile(values);
    }

    /// <summary>
    /// Sorts <paramref name="values"/> under an observing context and checks every phase's operations against the structure of
    /// the algorithm: the scan only reads; the first pass moves the n mod b tail into scratch and then reads and writes every
    /// element once, as does every later pass, so the fixups move nothing; and the finalization only copies, pulling every
    /// element at most once and pushing at most ⌊n/b⌋ whole blocks.
    /// </summary>
    private static async Task AssertOperationProfile(int[] values)
    {
        var n = values.Length;
        var expected = values.OrderBy(x => x).ToArray();
        var digitCount = DigitCount(values);
        var blockSize = 1 << Radsort.BlockShift(n, sizeof(int));
        var tail = n % blockSize;

        var ledger = new PhaseLedger();
        Radsort.Sort(values.AsSpan(), ledger.Context);

        await Assert.That(values).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Assert.That(ledger.Compares).IsEqualTo(0L);
        await Assert.That(ledger.Swaps).IsEqualTo(0L);

        SortPhase[] phaseOrder = [SortPhase.KeyRangeScan, .. Enumerable.Repeat(SortPhase.RadixPass, digitCount), SortPhase.DistributionWrite];
        await Assert.That(ledger.Phases.Select(p => p.Phase).ToArray()).IsEquivalentTo(phaseOrder, CollectionOrdering.Matching);

        var scan = ledger.Phases[0];
        await Assert.That(scan.Reads).IsEqualTo((long)n);
        await Assert.That(scan.Writes).IsEqualTo(0L);

        for (var d = 0; d < digitCount; d++)
        {
            var pass = ledger.Phases[1 + d];
            var moved = n + (d == 0 ? tail : 0);
            await Assert.That((pass.Param1, pass.Param2)).IsEqualTo((d, digitCount));
            await Assert.That(pass.Reads).IsEqualTo((long)moved);
            await Assert.That(pass.Writes).IsEqualTo((long)moved);
        }

        var finalization = ledger.Phases[^1];
        await Assert.That(finalization.Writes).IsEqualTo(finalization.Reads);
        await Assert.That(finalization.Writes).IsGreaterThan(0L);
        await Assert.That(finalization.Writes).IsLessThanOrEqualTo((long)(n + n / blockSize * blockSize));
        await Assert.That(finalization.ElementWrites).IsEqualTo(0L);   // block copies only
        await Assert.That(finalization.LongestCopy).IsLessThanOrEqualTo(blockSize);
    }

    /// <summary>Passes the sort runs for int keys: ⌈bitlength(max − min) / 8⌉ on the sign-flipped keys.</summary>
    private static int DigitCount(int[] values)
    {
        var keys = values.Select(v => (ulong)((uint)v ^ 0x8000_0000u)).ToArray();
        var range = keys.Max() - keys.Min();
        var requiredBits = 64 - BitOperations.LeadingZeroCount(range);
        return (requiredBits + 7) / 8;
    }

    /// <summary>Tallies element operations per announced phase.</summary>
    private sealed class PhaseLedger
    {
        public sealed class Tally(SortPhase phase, int param1, int param2)
        {
            public SortPhase Phase { get; } = phase;
            public int Param1 { get; } = param1;
            public int Param2 { get; } = param2;
            public long Reads { get; set; }
            public long Writes { get; set; }
            public long ElementWrites { get; set; }
            public int LongestCopy { get; set; }
        }

        public List<Tally> Phases { get; } = [];
        public long Compares { get; private set; }
        public long Swaps { get; private set; }
        public HashSet<int> Buffers { get; } = [];
        public int ScratchExtent { get; private set; }   // one past the highest scratch index touched

        public VisualizationContext Context { get; }

        public PhaseLedger()
        {
            Context = new VisualizationContext(
                onCompare: (_, _, _, _, _) => Compares++,
                onSwap: (_, _, _) => Swaps++,
                onIndexRead: (index, buffer) =>
                {
                    Touch(buffer, index, 1);
                    Phases[^1].Reads++;
                },
                onIndexWrite: (index, buffer, _) =>
                {
                    Touch(buffer, index, 1);
                    Phases[^1].Writes++;
                    Phases[^1].ElementWrites++;
                },
                onRangeCopy: (source, destination, length, sourceBuffer, destinationBuffer, _) =>
                {
                    Touch(sourceBuffer, source, length);
                    Touch(destinationBuffer, destination, length);
                    Phases[^1].Reads += length;
                    Phases[^1].Writes += length;
                    Phases[^1].LongestCopy = Math.Max(Phases[^1].LongestCopy, length);
                },
                onPhase: (phase, p1, p2, _) => Phases.Add(new Tally(phase, p1, p2)));
        }

        void Touch(int buffer, int index, int length)
        {
            Buffers.Add(buffer);
            if (buffer == 1) ScratchExtent = Math.Max(ScratchExtent, index + length);
        }
    }

    // ── Observation ─────────────────────────────────────────────────────────────────────────────────

    [Test]
    [Arguments(200, 42)]
    [Arguments(1000, 1234)]
    [Arguments(10_000, 42)]
    public async Task AuxiliarySpaceIsTheScratchBlocksTest(int n, int seed)
    {
        // Two buffers: A (0) and the scratch blocks T (1). Nothing outside 2σ blocks of b elements is ever touched, which
        // is the O(√n) element space; the head start lives in T, so a sort that moves anything always uses it.
        var values = TestHelpers.ShuffledRangeScaled(n, seed, 37);
        var blockSize = 1 << Radsort.BlockShift(n, sizeof(int));

        var ledger = new PhaseLedger();
        Radsort.Sort(values.AsSpan(), ledger.Context);

        await Assert.That(values).IsEquivalentTo(values.OrderBy(x => x), CollectionOrdering.Matching);
        await Assert.That(ledger.Buffers.Order().ToArray()).IsEquivalentTo([0, 1], CollectionOrdering.Matching);
        await Assert.That(ledger.ScratchExtent).IsLessThanOrEqualTo(2 * Sigma * blockSize);
    }

    [Test]
    [Arguments(300, 42)]
    [Arguments(3000, 1234)]
    [Arguments(20_000, 42)]
    public async Task ObservedOperationsReplayToTheSortedSpanTest(int n, int seed)
    {
        // Replays the stream on a model of both buffers. Element values live in the reported writes and copies, so the replay
        // ends at the sorted span only if every movement was reported with the value it moved; a copy must report what its
        // source holds; and no scratch element may be read before a write or copy initialized it.
        var values = TestHelpers.ShuffledRangeScaled(n, seed, 37);
        var blockSize = 1 << Radsort.BlockShift(n, sizeof(int));
        var main = values.Select(v => (int?)v).ToArray();
        var scratch = new int?[2 * Sigma * blockSize];   // null: never written
        var problems = new List<string>();

        int?[] Model(int buffer) => buffer == 0 ? main : scratch;

        Radsort.Sort(values.AsSpan(), new VisualizationContext(
            onIndexRead: (index, buffer) =>
            {
                if (Model(buffer)[index] is null) problems.Add($"read of uninitialized {buffer}:{index}");
            },
            onIndexWrite: (index, buffer, value) => Model(buffer)[index] = (int)value!,
            onRangeCopy: (source, destination, length, sourceBuffer, destinationBuffer, copied) =>
            {
                var from = Model(sourceBuffer);
                var to = Model(destinationBuffer);
                for (var i = 0; i < length; i++)
                {
                    if (from[source + i] != (int)copied![i]!) problems.Add($"copy from {sourceBuffer}:{source + i} reports a value the source does not hold");
                }
                for (var i = 0; i < length; i++)
                {
                    to[destination + i] = (int)copied![i]!;
                }
            }));

        await Assert.That(problems.Take(5).ToArray()).IsEmpty();
        await Assert.That(values).IsEquivalentTo(values.OrderBy(x => x), CollectionOrdering.Matching);
        await Assert.That(main.Select(v => v!.Value).ToArray()).IsEquivalentTo(values, CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(100, 1)]          // [-50, 50): straddles zero, range 99 -> 7 bits -> 1 pass, not the 4 the key width would give
    [Arguments(100, 100_000)]    // range 9,900,000 -> 24 bits -> 3 passes
    public async Task PassCountFollowsKeyRangeTest(int n, int stride)
    {
        var values = Enumerable.Range(-n / 2, n).Select(v => v * stride).ToArray();
        new Random(42).Shuffle(values);

        var phases = new List<(SortPhase Phase, int Digit, int Total)>();
        Radsort.Sort(values.AsSpan(), new VisualizationContext(
            onPhase: (phase, p1, p2, _) => phases.Add((phase, p1, p2))));

        var requiredBits = 64 - BitOperations.LeadingZeroCount((ulong)((n - 1) * stride));
        var digitCount = (requiredBits + 7) / 8;

        await Assert.That(values).IsEquivalentTo(values.OrderBy(x => x), CollectionOrdering.Matching);
        await Assert.That(phases[0].Phase).IsEqualTo(SortPhase.KeyRangeScan);
        await Assert.That(phases.Skip(1).Take(digitCount).ToArray()).IsEquivalentTo(
            Enumerable.Range(0, digitCount).Select(d => (SortPhase.RadixPass, d, digitCount)).ToArray(),
            CollectionOrdering.Matching);
        await Assert.That(phases[^1].Phase).IsEqualTo(SortPhase.DistributionWrite);
        await Assert.That(phases.Count).IsEqualTo(digitCount + 2);
    }
}
