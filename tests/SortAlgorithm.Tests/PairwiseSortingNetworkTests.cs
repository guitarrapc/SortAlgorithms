using System.Numerics;
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class PairwiseSortingNetworkTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => PairwiseSortingNetwork.Sort(span, context);

    // A comparator exchanges only when its pair is out of order, and sorted input has no such pair.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    [MethodDataSource(typeof(MockPowerOfTwoRandomData), nameof(MockPowerOfTwoRandomData.Generate))]
    [MethodDataSource(typeof(MockPowerOfTwoNegativePositiveRandomData), nameof(MockPowerOfTwoNegativePositiveRandomData.Generate))]
    [MethodDataSource(typeof(MockPowerOfTwoReversedData), nameof(MockPowerOfTwoReversedData.Generate))]
    [MethodDataSource(typeof(MockPowerOfTwoNearlySortedData), nameof(MockPowerOfTwoNearlySortedData.Generate))]
    [MethodDataSource(typeof(MockPowerOfTwoSameValuesData), nameof(MockPowerOfTwoSameValuesData.Generate))]
    public async Task PowerOfTwoSortResultOrderTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        PairwiseSortingNetwork.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    public static IEnumerable<int> ZeroOneSizes() => Enumerable.Range(1, 18);

    /// <summary>
    /// The zero-one principle: a comparator network sorts every input of length n if and only if it sorts every
    /// 0-1 input of length n. Enumerating all 2^n bit strings is therefore a proof, not a sample, that the network
    /// built for this n sorts — including the non-power-of-two lengths the paper does not define.
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

            PairwiseSortingNetwork.Sort(bits.AsSpan());

            // Sorted 0-1 output with the same number of ones: zeros in [0, n - ones), ones after.
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

    public static IEnumerable<int> LayerSizes() =>
    [
        2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17, 18, 19, 20, 23, 24, 25, 31, 32, 33,
        63, 64, 65, 100, 127, 128, 129, 255, 256, 257, 1000, 1024,
    ];

    /// <summary>
    /// The comparators, and the layers they are grouped into, are exactly those of Parberry's recursive construction
    /// unrolled in the paper's order: every Step 1 layer first (distances 1, 2, 4, ...), then the Step 3 layers from the
    /// deepest recursion level to the top, each level with its pair offset s descending. The reference is built from the
    /// recursive definition in the paper rather than from the implementation's loops, and for non-power-of-two n it is the
    /// network for the next power of two with the comparators that reach past n removed.
    /// </summary>
    [Test]
    [MethodDataSource(nameof(LayerSizes))]
    public async Task LayersMatchParberryRecursiveConstructionTest(int n)
    {
        var recorder = new NetworkRecorder();
        var values = TestHelpers.ShuffledRange(n, 42);
        PairwiseSortingNetwork.Sort(values.AsSpan(), recorder);

        var expected = ParberryLayers(n);

        await Assert.That(recorder.Unannounced).IsEqualTo(0).Because("every comparator belongs to an announced layer");
        await Assert.That(recorder.Layers.Count).IsEqualTo(expected.Count);

        for (var i = 0; i < expected.Count; i++)
        {
            var actual = recorder.Layers[i];
            var (phase, param1, param2, comparators) = expected[i];

            await Assert.That(actual.Phase).IsEqualTo(phase).Because($"layer {i}");
            await Assert.That(actual.Param1).IsEqualTo(param1).Because($"layer {i} distance");
            await Assert.That(phase == SortPhase.PairwisePairLayer ? actual.Param2 : actual.Param3).IsEqualTo(n).Because($"layer {i} count");
            if (phase == SortPhase.PairwiseSortedPairsLayer)
                await Assert.That(actual.Param2).IsEqualTo(param2).Because($"layer {i} stride");

            var observed = actual.Comparators.Order().ToList();
            await Assert.That(observed).IsEquivalentTo(comparators, CollectionOrdering.Matching).Because($"layer {i} comparators");

            // A layer is a set of comparators that share no position, which is what makes the depth count meaningful.
            var positions = actual.Comparators.SelectMany(c => new[] { c.Lo, c.Hi }).ToList();
            await Assert.That(positions.Distinct().Count()).IsEqualTo(positions.Count).Because($"layer {i} reuses a position");
        }
    }

    /// <summary>
    /// The network is data-oblivious: the sequence of compared positions depends only on n.
    /// </summary>
    [Test]
    [Arguments(5)]
    [Arguments(12)]
    [Arguments(31)]
    [Arguments(64)]
    [Arguments(100)]
    public async Task ComparatorSequenceIsDataIndependentTest(int n)
    {
        int[][] inputs =
        [
            Enumerable.Range(0, n).ToArray(),
            Enumerable.Range(0, n).Reverse().ToArray(),
            Enumerable.Repeat(7, n).ToArray(),
            TestHelpers.ShuffledRange(n, 42),
            TestHelpers.ShuffledRange(n, 1234),
        ];

        List<(int Lo, int Hi)>? reference = null;
        foreach (var input in inputs)
        {
            var recorder = new NetworkRecorder();
            PairwiseSortingNetwork.Sort(input.AsSpan(), recorder);
            var sequence = recorder.Layers.SelectMany(l => l.Comparators).ToList();

            if (reference is null) reference = sequence;
            else await Assert.That(sequence).IsEquivalentTo(reference, CollectionOrdering.Matching);
        }
    }

    [Test]
    [Arguments(2)]
    [Arguments(4)]
    [Arguments(8)]
    [Arguments(16)]
    [Arguments(32)]
    [Arguments(64)]
    [Arguments(128)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        PairwiseSortingNetwork.Sort(sorted.AsSpan(), stats);

        // Data-oblivious: Parberry's size S(n) = n log n (log n - 1) / 4 + n - 1, and no comparator fires on sorted input.
        var expectedCompares = ParberrySize(n);

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedCompares * 2);
    }

    [Test]
    [Arguments(2)]
    [Arguments(4)]
    [Arguments(8)]
    [Arguments(16)]
    [Arguments(32)]
    [Arguments(64)]
    [Arguments(128)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        PairwiseSortingNetwork.Sort(reversed.AsSpan(), stats);

        var expectedCompares = ParberrySize(n);

        // Each comparison reads its two positions; each swap reads and writes both again.
        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount > 0).IsTrue().Because("Reversed array should require swaps");
        await Assert.That(stats.SwapCount <= expectedCompares).IsTrue().Because("at most one swap per comparator");
        await Assert.That(stats.IndexWriteCount).IsEqualTo(stats.SwapCount * 2);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedCompares * 2 + stats.SwapCount * 2);
    }

    [Test]
    [Arguments(2, 42)]
    [Arguments(2, 1234)]
    [Arguments(4, 42)]
    [Arguments(4, 1234)]
    [Arguments(8, 42)]
    [Arguments(8, 1234)]
    [Arguments(16, 42)]
    [Arguments(16, 1234)]
    [Arguments(32, 42)]
    [Arguments(32, 1234)]
    [Arguments(64, 42)]
    [Arguments(64, 1234)]
    [Arguments(128, 42)]
    [Arguments(128, 1234)]
    public async Task TheoreticalValuesRandomTest(int n, int seed)
    {
        var stats = new StatisticsContext();
        var random = TestHelpers.ShuffledRange(n, seed);
        PairwiseSortingNetwork.Sort(random.AsSpan(), stats);

        var expectedCompares = ParberrySize(n);

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount <= expectedCompares).IsTrue().Because("at most one swap per comparator");
        await Assert.That(stats.IndexWriteCount).IsEqualTo(stats.SwapCount * 2);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedCompares * 2 + stats.SwapCount * 2);
    }

    /// <summary>
    /// For n = 2^k every announced layer is one layer of the network, so the announcements count Parberry's depth
    /// D(n) = log n (log n + 1) / 2.
    /// </summary>
    [Test]
    [Arguments(2, 1)]
    [Arguments(4, 3)]
    [Arguments(8, 6)]
    [Arguments(16, 10)]
    [Arguments(32, 15)]
    [Arguments(64, 21)]
    [Arguments(1024, 55)]
    public async Task LayerCountIsNetworkDepthTest(int n, int depth)
    {
        var recorder = new NetworkRecorder();
        PairwiseSortingNetwork.Sort(TestHelpers.ShuffledRange(n, 42).AsSpan(), recorder);

        var log = BitOperations.Log2((uint)n);
        await Assert.That(depth).IsEqualTo(log * (log + 1) / 2);
        await Assert.That(recorder.Layers.Count).IsEqualTo(depth);

        // The first log n layers compare positions 1, 2, 4, ..., n/2 apart, each with n/2 comparators.
        for (var h = 0; h < log; h++)
        {
            await Assert.That(recorder.Layers[h].Phase).IsEqualTo(SortPhase.PairwisePairLayer);
            await Assert.That(recorder.Layers[h].Comparators.All(c => c.Hi - c.Lo == 1 << h)).IsTrue();
            await Assert.That(recorder.Layers[h].Comparators.Count).IsEqualTo(n / 2);
        }
    }

    /// <summary>
    /// Parberry's network has exactly the size of Batcher's odd-even merge sort, but it is a different network.
    /// </summary>
    [Test]
    [Arguments(8)]
    [Arguments(16)]
    [Arguments(32)]
    [Arguments(64)]
    [Arguments(256)]
    public async Task SameSizeAsBatcherButDifferentComparatorsTest(int n)
    {
        var pairwise = new NetworkRecorder();
        var batcher = new NetworkRecorder();
        PairwiseSortingNetwork.Sort(TestHelpers.ShuffledRange(n, 42).AsSpan(), pairwise);
        BatcherOddEvenMergeSort.Sort(TestHelpers.ShuffledRange(n, 42).AsSpan(), batcher);

        var pairwiseComparators = pairwise.Layers.SelectMany(l => l.Comparators).Order().ToList();
        var batcherComparators = batcher.Layers.SelectMany(l => l.Comparators).Order().ToList();

        await Assert.That(pairwiseComparators.Count).IsEqualTo(batcherComparators.Count);
        await Assert.That((ulong)pairwiseComparators.Count).IsEqualTo(ParberrySize(n));
        await Assert.That(pairwiseComparators.SequenceEqual(batcherComparators)).IsFalse();
    }

    [Test]
    public async Task EqualKeysAreReorderedTest()
    {
        // [1a, 1b, 0]: the distance-2 pair layer compares positions 0 and 2 and carries 1a past 1b,
        // and the final layer leaves the equal pair alone. This is the reason IsStable is false.
        var items = new StabilityTestItem[] { new(1, 0), new(1, 1), new(0, 2) };

        PairwiseSortingNetwork.Sort(items.AsSpan());

        await Assert.That(items.Select(x => x.Value)).IsEquivalentTo([0, 1, 1], CollectionOrdering.Matching);
        await Assert.That(items.Select(x => x.OriginalIndex)).IsEquivalentTo([2, 1, 0], CollectionOrdering.Matching);
        await Assert.That(PairwiseSortingNetwork.IsStable).IsFalse();
    }

    [Test]
    public async Task CustomComparerTest()
    {
        var array = new[] { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5 };
        var descending = Comparer<int>.Create(static (a, b) => b.CompareTo(a));

        PairwiseSortingNetwork.Sort(array.AsSpan(), descending, NullContext.Default);

        await Assert.That(array).IsEquivalentTo([9, 6, 5, 5, 5, 4, 3, 3, 2, 1, 1], CollectionOrdering.Matching);
    }

    [Test]
    public async Task SortsSliceWithoutTouchingNeighboursTest()
    {
        var array = new[] { 100, 7, 3, 9, 1, 8, 2, -100 };

        PairwiseSortingNetwork.Sort(array.AsSpan(1, 6));

        await Assert.That(array).IsEquivalentTo([100, 1, 2, 3, 7, 8, 9, -100], CollectionOrdering.Matching);
    }

    [Test]
    public async Task LargeCountIndexArithmeticDoesNotWrapTest()
    {
        // For count = int.MaxValue the largest stride is 2^30; the step past it must still exceed count instead of wrapping to 0.
        await Assert.That(PairwiseSortingNetwork.NextStride(1u << 30)).IsEqualTo(1u << 31);
        await Assert.That(PairwiseSortingNetwork.NextStride(1u << 30) > int.MaxValue).IsTrue();

        // The last block start below count - distance plus two strides must not wrap past uint.MaxValue.
        var lastBlock = (uint)int.MaxValue - 2;
        await Assert.That(PairwiseSortingNetwork.NextBlock(lastBlock, 1u << 30) > lastBlock).IsTrue();
    }

    /// <summary>For n = 2^k: S(n) = n log n (log n - 1) / 4 + n - 1 (Parberry 1992, Section 4).</summary>
    static ulong ParberrySize(int n)
    {
        if (n <= 1) return 0;
        var log = BitOperations.Log2((uint)n);
        return (ulong)(n * log * (log - 1) / 4 + n - 1);
    }

    readonly record struct LayerKey(int Step, int Depth, int Offset);

    /// <summary>
    /// Parberry's recursive construction on the positions in <paramref name="wires"/> (a power-of-two count), transcribed
    /// from Sections 2 and 3 of the paper with 0-based indices:
    /// Step 1 compares x[2t] with x[2t+1]; Step 2 recursively sorts the even- and odd-indexed positions;
    /// Step 3 views the k = m/2 pairs as (a_t, b_t) = (x[2t], x[2t+1]) and, for s = k/2, ..., 1, compares b_t with a_(t+s).
    /// Each comparator is tagged with the step, recursion depth, and pair offset it came from.
    /// </summary>
    static void Construct(int[] wires, int depth, List<(LayerKey Key, int Lo, int Hi)> output)
    {
        var m = wires.Length;
        if (m < 2) return;

        for (var t = 0; t < m / 2; t++)
            output.Add((new LayerKey(1, depth, 0), wires[2 * t], wires[2 * t + 1]));

        Construct(wires.Where((_, i) => i % 2 == 0).ToArray(), depth + 1, output);
        Construct(wires.Where((_, i) => i % 2 == 1).ToArray(), depth + 1, output);

        var k = m / 2;
        for (var s = k / 2; s >= 1; s /= 2)
            for (var t = 0; t + s < k; t++)
                output.Add((new LayerKey(3, depth, s), wires[2 * t + 1], wires[2 * (t + s)]));
    }

    /// <summary>
    /// The layers of the n-input network in the paper's order. A Step 1 at depth h runs before the recursion below it,
    /// and a Step 3 at depth h after it, so unrolling puts all Step 1 layers first by depth ascending, then all Step 3
    /// layers by depth descending, each depth with its offsets descending.
    /// </summary>
    static List<(SortPhase Phase, int Distance, int Stride, List<(int Lo, int Hi)> Comparators)> ParberryLayers(int n)
    {
        var width = (int)BitOperations.RoundUpToPowerOf2((uint)n);
        var all = new List<(LayerKey Key, int Lo, int Hi)>();
        Construct(Enumerable.Range(0, width).ToArray(), 0, all);

        return all
            .Where(c => c.Hi < n)
            .GroupBy(c => c.Key)
            .OrderBy(g => g.Key.Step)
            .ThenBy(g => g.Key.Step == 1 ? g.Key.Depth : -g.Key.Depth)
            .ThenByDescending(g => g.Key.Offset)
            .Select(g =>
            {
                var stride = 1 << g.Key.Depth;
                return g.Key.Step == 1
                    ? (SortPhase.PairwisePairLayer, stride, stride, g.Select(c => (c.Lo, c.Hi)).Order().ToList())
                    : (SortPhase.PairwiseSortedPairsLayer, stride * (2 * g.Key.Offset - 1), stride, g.Select(c => (c.Lo, c.Hi)).Order().ToList());
            })
            .ToList();
    }

    sealed record RecordedLayer(SortPhase Phase, int Param1, int Param2, int Param3, List<(int Lo, int Hi)> Comparators);

    /// <summary>
    /// Records each announced phase and the comparators that run after it, as (lower, upper) positions.
    /// </summary>
    sealed class NetworkRecorder : ISortContext
    {
        public List<RecordedLayer> Layers { get; } = [];
        public int Unannounced { get; private set; }

        public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0)
            => Layers.Add(new RecordedLayer(phase, param1, param2, param3, []));

        public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ)
        {
            if (Layers.Count == 0) { Unannounced++; return; }
            Layers[^1].Comparators.Add((Math.Min(i, j), Math.Max(i, j)));
        }

        public void OnSwap(int i, int j, int bufferId) { }
        public void OnIndexRead(int index, int bufferId) { }
        public void OnIndexWrite(int index, int bufferId) { }
        public void OnIndexWrite<T>(int index, int bufferId, T value) { }
        public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId) { }
        public void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values) { }
        public void OnRole(int index, int bufferId, RoleType role) { }
    }
}
