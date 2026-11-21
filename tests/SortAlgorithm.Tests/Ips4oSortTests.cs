using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using System.Runtime.CompilerServices;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class Ips4oSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => Ips4oSort.Sort(span, context);

    // The initial check recognises sorted input with n - 1 comparisons and moves nothing.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    public async Task RangeSortTest()
    {
        var source = TestHelpers.ShuffledRange(10_000, 5);
        var expected = source.ToArray();
        Array.Sort(expected, 7, 9_986);
        var actual = source.ToArray();

        Ips4oSort.Sort(actual.AsSpan(), 7, 9_993, new StatisticsContext());

        await Assert.That(actual).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    /// <summary>A comparer that is not the natural order of a primitive type runs the checks for inconsistent comparers.</summary>
    [Test]
    public async Task CustomComparerDescendingTest()
    {
        var array = TestHelpers.ShuffledRange(50_000, 42);
        var descending = Comparer<int>.Create(static (a, b) => b.CompareTo(a));

        Ips4oSort.Sort(array.AsSpan(), descending, new StatisticsContext());

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, 50_000).Reverse().ToArray(), CollectionOrdering.Matching);
    }

    /// <summary>
    /// Every length up to 300 and the lengths around each threshold: insertion sort up to 256 and up to 32 inside the recursion, the
    /// last level up to 4,096, and two levels up to 1,048,576, under both contexts.
    /// </summary>
    [Test]
    public async Task EveryShapeAndLengthSorts()
    {
        var lengths = Enumerable.Range(0, 301).Concat([511, 512, 513, 4095, 4096, 4097, 20_000, 100_000]);

        foreach (var n in lengths)
        {
            foreach (var pattern in CrumSortTests.Patterns)
            {
                var observed = CrumSortTests.Pattern(pattern, n, n);
                var expected = observed.Order().ToArray();
                var unobserved = observed.ToArray();

                Ips4oSort.Sort(observed.AsSpan(), new StatisticsContext());
                Ips4oSort.Sort(unobserved.AsSpan());

                await Assert.That(observed).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"{pattern} n={n} observed");
                await Assert.That(unobserved).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"{pattern} n={n} unobserved");
            }
        }
    }

    /// <summary>
    /// The four classes of the initial check: the last element is not less than the first and the input is sorted (done) or not
    /// (sorted further); the last element is less and the input never increases (reversed, done) or does somewhere (sorted further).
    /// </summary>
    [Test]
    [Arguments("sorted", true, 0)]
    [Arguments("sortedWithDuplicates", true, 0)]
    [Arguments("allEqual", true, 0)]
    [Arguments("lastNotLessUnsorted", false, 0)]
    [Arguments("nonIncreasing", true, 500)]
    [Arguments("lastLessIncreasing", false, 0)]
    public async Task InitialCheckClasses(string shape, bool done, int swaps)
    {
        const int n = 1000;
        var array = shape switch
        {
            "sorted" => Enumerable.Range(0, n).ToArray(),
            "sortedWithDuplicates" => Enumerable.Range(0, n).Select(i => i / 3).ToArray(),
            "allEqual" => new int[n],
            "lastNotLessUnsorted" => Enumerable.Range(0, n).Select(i => i == 1 ? -1 : i).ToArray(),
            "nonIncreasing" => Enumerable.Range(0, n).Select(i => (n - i) / 3).ToArray(),
            _ => Enumerable.Range(0, n).Select(i => i == n / 2 ? n : n - i).ToArray(),
        };
        var expected = array.Order().ToArray();
        var stats = new StatisticsContext();

        Ips4oSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
        if (done)
        {
            // One comparison of the last element with the first, and one per adjacent pair.
            await Assert.That(stats.CompareCount).IsEqualTo((ulong)n);
            await Assert.That(stats.SwapCount).IsEqualTo((ulong)swaps);
        }
        else
        {
            await Assert.That(stats.CompareCount).IsGreaterThan((ulong)n);
        }
    }

    /// <summary>Up to 256 elements go to insertion sort after the initial check; from 257 the first partitioning step draws a sample.</summary>
    [Test]
    [Arguments(256, false)]
    [Arguments(257, true)]
    public async Task WholeInputBaseCaseThreshold(int n, bool partitions)
    {
        var phases = new List<SortPhase>();
        var context = new VisualizationContext(onPhase: (phase, _, _, _) => phases.Add(phase));
        var array = TestHelpers.ShuffledRange(n, 7);

        Ips4oSort.Sort(array.AsSpan(), context);

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
        await Assert.That(phases[0]).IsEqualTo(SortPhase.Ips4oSimpleCases);
        await Assert.That(phases.Contains(SortPhase.Ips4oSampling)).IsEqualTo(partitions);
    }

    /// <summary>
    /// The bucket count of the step on the whole input follows its size: ℓ = ⌊log₂(n/16)⌋ up to 4,096 elements, half of
    /// ⌊log₂(n/16)⌋ + 1 up to 1,048,576, and 8 above. Distinct keys repeat no splitter, so the count is 2^ℓ. The sample's own steps
    /// run first and are told apart by their range.
    /// </summary>
    [Test]
    [Arguments(300, 16)]
    [Arguments(4096, 256)]
    [Arguments(4097, 16)]
    [Arguments(100_000, 64)]
    [Arguments(1_048_576, 256)]
    [Arguments(1_048_577, 256)]
    public async Task FirstStepBucketCount(int n, int buckets)
    {
        var first = -1;
        var context = new VisualizationContext(onPhase: (phase, start, last, count) =>
        {
            if (phase == SortPhase.Ips4oClassification && start == 0 && last == n - 1) first = count;
        });
        var array = TestHelpers.ShuffledRange(n, 11);

        Ips4oSort.Sort(array.AsSpan(), context);

        await Assert.That(first).IsEqualTo(buckets);
    }

    /// <summary>
    /// Four distinct keys fill every splitter candidate with repeats, so equality buckets are used: four splitters padded to eight
    /// leaves, two buckets each. Elements equal to a splitter are final, so after the first step only the base cases remain.
    /// </summary>
    [Test]
    public async Task FewDistinctKeysUseEqualityBuckets()
    {
        const int n = 200_000;
        var random = new Random(42);
        var keys = Enumerable.Range(0, n).Select(_ => random.Next(4)).ToArray();
        var expected = keys.Order().ToArray();
        var classifications = new List<int>();
        var context = new VisualizationContext(onPhase: (phase, _, _, count) =>
        {
            if (phase == SortPhase.Ips4oClassification) classifications.Add(count);
        });
        var array = keys.ToArray();
        var counted = keys.ToArray();
        var stats = new StatisticsContext();

        Ips4oSort.Sort(array.AsSpan(), context);
        Ips4oSort.Sort(counted.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
        // The sample's own step, then the step on the whole input. Neither leaves an ordinary bucket to partition again.
        await Assert.That(classifications).IsEquivalentTo(new[] { 16, 16 }, CollectionOrdering.Matching);
        // Three comparisons down the tree and one equality comparison per element, plus the sample and the block heads.
        await Assert.That(stats.CompareCount).IsLessThan((ulong)(5 * n));
    }

    /// <summary>Distinct keys: comparisons stay near n log₂ n, below the 2 n log₂ n of a quicksort with a fixed pivot rule.</summary>
    [Test]
    public async Task DistinctKeysStayNearNLogN()
    {
        const int n = 100_000;
        var array = TestHelpers.ShuffledRange(n, 3);
        var stats = new StatisticsContext();

        Ips4oSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
        await Assert.That(stats.CompareCount).IsLessThan((ulong)(1.6 * n * Math.Log2(n)));
    }

    /// <summary>The samples, and with them every operation, depend only on the seed.</summary>
    [Test]
    public async Task SeedDeterminesTheSamples()
    {
        const int n = 50_000;
        var source = TestHelpers.ShuffledRange(n, 9);

        ulong Comparisons(ulong seed)
        {
            var array = source.ToArray();
            var stats = new StatisticsContext();
            Ips4oSort.Sort(array.AsSpan(), new ComparableComparer<int>(), stats, seed);
            if (!array.SequenceEqual(Enumerable.Range(0, n))) throw new InvalidOperationException($"seed {seed}: not sorted");
            return stats.CompareCount;
        }

        var first = Comparisons(1);
        await Assert.That(Comparisons(1)).IsEqualTo(first);
        await Assert.That(Comparisons(2)).IsNotEqualTo(first);
        await Assert.That(Comparisons(0)).IsNotEqualTo(first);
    }

    /// <summary>
    /// The block holds 2 KiB of elements: 2,048 bytes, 128 two-long structs, 256 references, and a single element when one element is
    /// larger than 2 KiB.
    /// </summary>
    [Test]
    public async Task EveryBlockSizeSorts()
    {
        var random = new Random(5);

        var bytes = Enumerable.Range(0, 60_000).Select(_ => (byte)random.Next(256)).ToArray();
        var expectedBytes = bytes.Order().ToArray();
        Ips4oSort.Sort(bytes.AsSpan());
        await Assert.That(bytes).IsEquivalentTo(expectedBytes, CollectionOrdering.Matching);

        var strings = Enumerable.Range(0, 20_000).Select(_ => random.Next(1_000_000).ToString("D7")).ToArray();
        var expectedStrings = strings.Order(StringComparer.Ordinal).ToArray();
        Ips4oSort.Sort(strings.AsSpan(), StringComparer.Ordinal, new StatisticsContext());
        await Assert.That(strings).IsEquivalentTo(expectedStrings, CollectionOrdering.Matching);

        var large = Enumerable.Range(0, 6_000).Select(_ => new Large(random.Next(1000))).ToArray();
        var expectedLarge = large.Select(x => x.Key).Order().ToArray();
        Ips4oSort.Sort(large.AsSpan());
        await Assert.That(large.Select(x => x.Key).ToArray()).IsEquivalentTo(expectedLarge, CollectionOrdering.Matching);
    }

    [InlineArray(260)]
    struct Payload
    {
        long element;
    }

    /// <summary>2,088 bytes: more than a block's 2 KiB, so a block holds one element.</summary>
    struct Large : IComparable<Large>
    {
        public readonly int Key;
        Payload payload;

        public Large(int key)
        {
            Key = key;
            payload[0] = key;
        }

        public readonly int CompareTo(Large other) => Key.CompareTo(other.Key);
    }

    /// <summary>
    /// Replays every reported operation on its own copy of each buffer: a read of an auxiliary buffer must follow a write to it, a copy
    /// must carry the values the replay holds, and the main buffer must end as the sort left it. The statistics must count the same
    /// operations.
    /// </summary>
    [Test]
    public async Task ReportedOperationsReplay()
    {
        foreach (var n in new[] { 257, 1000, 5000, 70_000 })
        {
            foreach (var pattern in new[] { "random", "fewDistinct", "pipeOrgan", "randomTail" })
            {
                var input = CrumSortTests.Pattern(pattern, n, n).Select((v, i) => new Item(v, i)).ToArray();
                var replay = new Replay(input);
                var observed = input.ToArray();
                Ips4oSort.Sort(observed.AsSpan(), new ItemComparer(), replay);

                for (var i = 0; i < n; i++)
                {
                    if (!ReferenceEquals(replay.Main[i], observed[i])) throw new InvalidOperationException($"{pattern} n={n}: replay differs at {i}");
                    if (i > 0 && observed[i].Value < observed[i - 1].Value) throw new InvalidOperationException($"{pattern} n={n}: not sorted at {i}");
                }

                var stats = new StatisticsContext();
                Ips4oSort.Sort(input.ToArray().AsSpan(), new ItemComparer(), stats);
                await Assert.That((stats.CompareCount, stats.IndexReadCount, stats.IndexWriteCount, stats.SwapCount))
                    .IsEqualTo((replay.Comparisons, replay.Reads, replay.Writes, replay.Swaps)).Because($"{pattern} n={n}");
                // The swap and overflow blocks are used only when blocks are permuted and when one would end past the range.
                await Assert.That(replay.Buffers.IsSupersetOf([0, 1, 2, 3]) && replay.Buffers.IsSubsetOf([0, 1, 2, 3, 4, 5])).IsTrue().Because($"{pattern} n={n}");
            }
        }
    }

    sealed record Item(int Value, int Position);

    readonly struct ItemComparer : IComparer<Item>
    {
        public int Compare(Item? x, Item? y) => x!.Value.CompareTo(y!.Value);
    }

    sealed class Replay(Item[] input) : ISortContext
    {
        public readonly Item[] Main = input.ToArray();
        readonly Dictionary<(int Buffer, int Index), Item> aux = [];
        public readonly SortedSet<int> Buffers = [];
        public ulong Reads, Writes, Comparisons, Swaps;

        Item Read(int index, int buffer)
        {
            Reads++;
            Buffers.Add(buffer);
            if (buffer == 0) return Main[index];
            return aux.TryGetValue((buffer, index), out var value) ? value : throw new InvalidOperationException($"read of buffer {buffer} at {index} before a write");
        }

        void Write(int index, int buffer, Item value)
        {
            Writes++;
            Buffers.Add(buffer);
            if (buffer is < 0 or > 5) throw new InvalidOperationException($"unknown buffer {buffer}");
            if (buffer == 0) Main[index] = value;
            else aux[(buffer, index)] = value;
        }

        public void OnIndexRead(int index, int bufferId) => Read(index, bufferId);
        public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) => Comparisons++;
        public void OnSwap(int i, int j, int bufferId)
        {
            var left = Read(i, bufferId);
            var right = Read(j, bufferId);
            Write(i, bufferId, right);
            Write(j, bufferId, left);
            Swaps++;
        }
        public void OnIndexWrite(int index, int bufferId) => throw new InvalidOperationException("element write without a value");
        public void OnIndexWrite<TValue>(int index, int bufferId, TValue value) => Write(index, bufferId, (Item)(object)value!);
        public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId)
            => throw new InvalidOperationException("copy without values");
        public void OnRangeCopy<TValue>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<TValue> copied)
        {
            var snapshot = new Item[length];
            for (var i = 0; i < length; i++)
            {
                snapshot[i] = Read(sourceIndex + i, sourceBufferId);
                if (!ReferenceEquals(snapshot[i], copied[i])) throw new InvalidOperationException("copy payload differs from the replay");
            }
            for (var i = 0; i < length; i++) Write(destinationIndex + i, destinationBufferId, snapshot[i]);
        }
        public void OnPhase(SortPhase phase, int a = -1, int b = -1, int c = -1) { }
        public void OnRole(int index, int bufferId, RoleType role) { }
        public void OnLink(int parentIndex, int childIndex, int bufferId, LinkSide side) => throw new InvalidOperationException("unexpected tree link");
    }
}
