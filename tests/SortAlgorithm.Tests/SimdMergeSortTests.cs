using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class SimdMergeSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => SimdMergeSort.Sort(span, context);

    // Every level loads and stores every element, and the networks write minima and maxima instead of exchanging elements.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    static int Lanes<T>() where T : IComparable<T> => SimdMergeSort.LaneCount<T, ComparableComparer<T>>();

    [Test]
    public async Task RangeSortTest()
    {
        var source = TestHelpers.ShuffledRange(3000, 5);
        var expected = source.ToArray();
        Array.Sort(expected, 7, 2990);
        var actual = source.ToArray();

        SimdMergeSort.Sort(actual.AsSpan(), 7, 2997, new StatisticsContext());

        await Assert.That(actual).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task CustomComparerDescendingTest()
    {
        var array = TestHelpers.ShuffledRange(5000, 42);
        var descending = Comparer<int>.Create(static (a, b) => b.CompareTo(a));

        SimdMergeSort.Sort(array.AsSpan(), descending, new StatisticsContext());

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, 5000).Reverse().ToArray(), CollectionOrdering.Matching);
    }

    /// <summary>
    /// Every length up to 300 and the lengths around the chunk and unit sizes, for each type the vector path runs on, with keys at both
    /// ends of the type: the largest key is also the value of the positions past n.
    /// </summary>
    [Test]
    public async Task EveryVectorizedTypeSorts()
    {
        List<string> problems =
        [
            .. Check(x => (int)x), .. Check(x => (uint)x), .. Check(x => x), .. Check(x => (ulong)x),
            .. Check(x => (nint)x), .. Check(x => (nuint)x),
        ];

        await Assert.That(problems).IsEmpty();

        static List<string> Check<T>(Func<long, T> convert) where T : IComparable<T>
        {
            int[] sizes = [.. Enumerable.Range(0, 300), 511, 512, 513, 1023, 1024, 1025, 4097, 20_000];
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
                    var observed = (T[])source.Clone();
                    var stats = new StatisticsContext();
                    SimdMergeSort.Sort(observed.AsSpan(), stats);
                    var unobserved = (T[])source.Clone();
                    SimdMergeSort.Sort(unobserved.AsSpan());

                    if (!observed.SequenceEqual(expected) || !unobserved.SequenceEqual(expected) || stats.SwapCount != 0)
                    {
                        problems.Add($"{typeof(T).Name}: n = {n}, pattern {pattern}");
                    }
                }
            }
            return problems;
        }
    }

    /// <summary>The lengths around one and two blocks, where the merges across blocks start.</summary>
    [Test]
    public async Task BlockBoundariesSort()
    {
        foreach (var n in new[] { SimdMergeSort.BlockLength<long>() - 1, SimdMergeSort.BlockLength<long>() + 1, 2 * SimdMergeSort.BlockLength<long>() + 3 })
        {
            var random = new Random(n);
            var keys = Enumerable.Range(0, n).Select(_ => random.NextInt64(long.MinValue, long.MaxValue)).ToArray();
            var expected = keys.Order().ToArray();
            SimdMergeSort.Sort(keys.AsSpan());
            await Assert.That(keys).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"n = {n}");
        }

        foreach (var n in new[] { SimdMergeSort.BlockLength<int>() + 1, 2 * SimdMergeSort.BlockLength<int>() + 3 })
        {
            var random = new Random(n);
            var keys = Enumerable.Range(0, n).Select(_ => random.Next(int.MinValue, int.MaxValue)).ToArray();
            var expected = keys.Order().ToArray();
            SimdMergeSort.Sort(keys.AsSpan());
            await Assert.That(keys).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"n = {n}");
        }
    }

    /// <summary>
    /// The in-register sort uses Batcher's odd-even merge sort of K inputs: K − 1 + K log₂K (log₂K − 1) / 4 comparators (the paper's
    /// 2(K − 1 + K log K (log K − 1) / 4) min/max operations), and by the 0-1 principle it sorts every input.
    /// </summary>
    [Test]
    [Arguments(2)]
    [Arguments(4)]
    [Arguments(8)]
    [Arguments(16)]
    public async Task OddEvenNetworkMatchesThePaper(int k)
    {
        var network = SimdMergeSort.OddEvenSortingNetwork(k);
        var log = (int)Math.Log2(k);

        await Assert.That(network.Length / 2).IsEqualTo(k - 1 + k * log * (log - 1) / 4);
        for (var input = 0; input < 1 << k; input++)
        {
            var bits = input;
            for (var p = 0; p < network.Length; p += 2)
            {
                var lo = (bits >> network[p]) & 1;
                var hi = (bits >> network[p + 1]) & 1;
                if (lo > hi) bits ^= (1 << network[p]) | (1 << network[p + 1]);
            }
            var ones = int.PopCount(input);
            await Assert.That(bits).IsEqualTo(((1 << ones) - 1) << (k - ones)).Because($"input {input:B}");
        }
    }

    /// <summary>
    /// The comparisons of values held in registers follow the paper's counts on power-of-two lengths: K·C(K) per chunk of K² elements, and
    /// for each pair of sequences of L elements, 2L/U − 1 networks of U log₂ 2U comparators, the unit U chosen by the number of pairs in
    /// flight. Every other comparison is the choice of the next unit, between two list heads in memory, at most one per network.
    /// </summary>
    [Test]
    [Arguments(1, false)]
    [Arguments(4, false)]
    [Arguments(64, false)]
    [Arguments(2, true)]
    public async Task NetworkComparisonsFollowThePaper(int multiple, bool acrossBlocks)
    {
        var k = Lanes<long>();
        var block = SimdMergeSort.BlockLength<long>();
        var n = acrossBlocks ? multiple * block : multiple * k * k;
        var (expectedNetwork, maxHeads) = Expected(n, k, block);

        long held = 0, heads = 0;
        var context = new VisualizationContext(onCompare: (i, j, _, bi, bj) =>
        {
            if (i < 0 && j < 0) held++;
            else if (i >= 0 && j >= 0 && bi == bj) heads++;
            else throw new InvalidOperationException($"unexpected operands {i}/{bi} {j}/{bj}");
        });
        var random = new Random(n);
        var keys = Enumerable.Range(0, n).Select(_ => random.NextInt64()).ToArray();
        var expected = keys.Order().ToArray();
        SimdMergeSort.Sort(keys.AsSpan(), context);

        await Assert.That(keys).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Assert.That(held).IsEqualTo(expectedNetwork);
        await Assert.That(heads).IsLessThanOrEqualTo(maxHeads);
    }

    /// <summary>A comparer other than the natural order runs one-lane vectors, whose networks follow the same counts with K = 1.</summary>
    [Test]
    public async Task OneLanePathFollowsThePaperWithKEqualToOne()
    {
        const int n = 4096;
        var (expectedNetwork, maxHeads) = Expected(n, 1, SimdMergeSort.BlockLength<int>());

        var stats = new StatisticsContext();
        var array = TestHelpers.ShuffledRange(n, 9);
        SimdMergeSort.Sort(array.AsSpan(), new AscendingInt(), stats);

        await Assert.That(SimdMergeSort.LaneCount<int, AscendingInt>()).IsEqualTo(1);
        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
        await Assert.That((long)stats.CompareCount).IsGreaterThanOrEqualTo(expectedNetwork);
        await Assert.That((long)stats.CompareCount).IsLessThanOrEqualTo(expectedNetwork + maxHeads);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    /// <summary>
    /// The phases name each block's in-register sort with K, each level inside a block with its sequence length, and the levels across
    /// blocks, which only an input longer than one block has.
    /// </summary>
    [Test]
    [Arguments(1000)]
    [Arguments(70_000)]
    public async Task PhasesFollowTheLevels(int n)
    {
        var k = Lanes<long>();
        var block = SimdMergeSort.BlockLength<long>();
        var phases = new List<(SortPhase Phase, int A, int B, int C)>();
        var context = new VisualizationContext(onPhase: (phase, a, b, c) => phases.Add((phase, a, b, c)));
        var keys = Enumerable.Range(0, n).Select(i => (long)(n - i)).ToArray();

        SimdMergeSort.Sort(keys.AsSpan(), context);

        var expected = new List<(SortPhase, int, int, int)>();
        for (var start = 0; start < n; start += block)
        {
            var end = Math.Min(start + block, n);
            expected.Add((SortPhase.SimdMergeSortInRegisterSort, start, end - 1, k));
            for (var run = k; run < end - start; run *= 2) expected.Add((SortPhase.SimdMergeSortBlockMerge, start, end - 1, run));
        }
        for (long run = block; run < n; run *= 2) expected.Add((SortPhase.SimdMergeSortMerge, 0, n - 1, (int)run));

        await Assert.That(phases).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    /// <summary>
    /// Replays every reported operation on its own copy of both buffers: a read of the auxiliary buffer must follow a write to it, a copy must
    /// carry the values the replay holds, and the input buffer must end as the sort left it. The statistics must count the same operations.
    /// </summary>
    [Test]
    public async Task ReportedOperationsReplay()
    {
        foreach (var n in new[] { 2, 17, 256, 257, 1000, 5000 })
        {
            foreach (var pattern in new[] { "random", "fewDistinct", "pipeOrgan", "reversed" })
            {
                var ints = CrumSortTests.Pattern(pattern, n, n);
                await Verify(ints, new ComparableComparer<int>(), $"int {pattern} n={n}");
                var strings = ints.Select(x => x.ToString("D10")).ToArray();
                await Verify(strings, StringComparer.Ordinal, $"string {pattern} n={n}");
            }
        }

        static async Task Verify<T, TComparer>(T[] input, TComparer comparer, string label) where TComparer : IComparer<T>
        {
            var replay = new Replay<T>(input);
            var observed = input.ToArray();
            SimdMergeSort.Sort(observed.AsSpan(), comparer, replay);
            var expected = input.Order(comparer).ToArray();
            for (var i = 0; i < input.Length; i++)
            {
                if (!EqualityComparer<T>.Default.Equals(replay.Main[i], observed[i])) throw new InvalidOperationException($"{label}: replay differs at {i}");
                if (comparer.Compare(observed[i], expected[i]) != 0) throw new InvalidOperationException($"{label}: not sorted at {i}");
            }

            var stats = new StatisticsContext();
            SimdMergeSort.Sort(input.ToArray().AsSpan(), comparer, stats);
            await Assert.That((stats.CompareCount, stats.IndexReadCount, stats.IndexWriteCount, stats.SwapCount))
                .IsEqualTo((replay.Comparisons, replay.Reads, replay.Writes, 0UL)).Because(label);
        }
    }

    internal static (long Network, long Heads) Expected(int n, int k, int block)
    {
        var chunk = (long)k * k;
        var log = (int)Math.Log2(k);
        long network = n / chunk * k * (k - 1 + k * log * (log - 1) / 4);
        long heads = 0;

        var blockLength = Math.Min(n, block);
        for (var run = k; run < blockLength; run *= 2) Level(n / (2L * run), blockLength / (2L * run), run);
        for (long run = block; run < n; run *= 2) Level(n / (2 * run), n / (2 * run), (int)run);
        return (network, heads);

        void Level(long pairs, long pairsInScope, int run)
        {
            var width = pairsInScope >= 4 ? 1 : pairsInScope >= 2 ? 2 : 4;
            width = Math.Min(width, run / k);
            var unit = width * k;
            var networks = 2L * run / unit - 1;
            network += pairs * networks * unit * (int)Math.Log2(2 * unit);
            heads += pairs * (networks - 1);
        }
    }

    readonly struct AscendingInt : IComparer<int>
    {
        public int Compare(int x, int y) => x.CompareTo(y);
    }

    sealed class Replay<T>(T[] input) : ISortContext
    {
        public readonly T[] Main = input.ToArray();
        readonly T[] aux = new T[input.Length];
        readonly bool[] auxWritten = new bool[input.Length];
        public ulong Reads, Writes, Comparisons;

        T Read(int index, int buffer)
        {
            Reads++;
            if (buffer == 0) return Main[index];
            if (buffer != 1) throw new InvalidOperationException($"unknown buffer {buffer}");
            return auxWritten[index] ? aux[index] : throw new InvalidOperationException($"read of buffer 1 at {index} before a write");
        }

        void Write(int index, int buffer, T value)
        {
            Writes++;
            if (buffer == 0) Main[index] = value;
            else if (buffer == 1) { aux[index] = value; auxWritten[index] = true; }
            else throw new InvalidOperationException($"unknown buffer {buffer}");
        }

        public void OnIndexRead(int index, int bufferId) => Read(index, bufferId);
        public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) => Comparisons++;
        public void OnSwap(int i, int j, int bufferId) => throw new InvalidOperationException("unexpected swap");
        public void OnIndexWrite(int index, int bufferId) => throw new InvalidOperationException("element write without a value");
        public void OnIndexWrite<TValue>(int index, int bufferId, TValue value) => Write(index, bufferId, (T)(object)value!);
        public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId)
            => throw new InvalidOperationException("copy without values");
        public void OnRangeCopy<TValue>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<TValue> copied)
        {
            var snapshot = new T[length];
            for (var i = 0; i < length; i++)
            {
                snapshot[i] = Read(sourceIndex + i, sourceBufferId);
                if (!EqualityComparer<T>.Default.Equals(snapshot[i], (T)(object)copied[i]!)) throw new InvalidOperationException("copy payload differs from the replay");
            }
            for (var i = 0; i < length; i++) Write(destinationIndex + i, destinationBufferId, snapshot[i]);
        }
        public void OnPhase(SortPhase phase, int a = -1, int b = -1, int c = -1) { }
        public void OnRole(int index, int bufferId, RoleType role) { }
        public void OnLink(int parentIndex, int childIndex, int bufferId, LinkSide side) => throw new InvalidOperationException("unexpected tree link");
    }
}
