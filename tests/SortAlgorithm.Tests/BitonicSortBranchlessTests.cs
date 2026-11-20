using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class BitonicSortBranchlessTests
{
    [Test]
    [MethodDataSource(typeof(MockPowerOfTwoRandomData), nameof(MockPowerOfTwoRandomData.Generate))]
    [MethodDataSource(typeof(MockPowerOfTwoNegativePositiveRandomData), nameof(MockPowerOfTwoNegativePositiveRandomData.Generate))]
    [MethodDataSource(typeof(MockPowerOfTwoReversedData), nameof(MockPowerOfTwoReversedData.Generate))]
    [MethodDataSource(typeof(MockPowerOfTwoNearlySortedData), nameof(MockPowerOfTwoNearlySortedData.Generate))]
    [MethodDataSource(typeof(MockPowerOfTwoSameValuesData), nameof(MockPowerOfTwoSameValuesData.Generate))]
    public async Task SortResultOrderTest(IInputSample<int> inputSample)
    {
        var observed = inputSample.Samples.ToArray();
        BitonicSortBranchless.Sort(observed.AsSpan(), new StatisticsContext());
        var unobserved = inputSample.Samples.ToArray();
        BitonicSortBranchless.Sort(unobserved.AsSpan());

        Array.Sort(inputSample.Samples);
        await Assert.That(observed).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
        await Assert.That(unobserved).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    public async Task ThrowsOnNonPowerOfTwo()
    {
        var array = new int[] { 3, 1, 4, 1, 5, 9, 2 };
        Assert.Throws<ArgumentException>(() => BitonicSortBranchless.Sort(array.AsSpan(), new StatisticsContext()));
        Assert.Throws<ArgumentException>(() => BitonicSortBranchless.Sort(array.AsSpan()));
        await Assert.That(array).IsEquivalentTo([3, 1, 4, 1, 5, 9, 2], CollectionOrdering.Matching);
    }

    [Test]
    public async Task EmptyAndSingleElement()
    {
        var empty = Array.Empty<int>();
        BitonicSortBranchless.Sort(empty.AsSpan());
        var single = new[] { 42 };
        var stats = new StatisticsContext();
        BitonicSortBranchless.Sort(single.AsSpan(), stats);

        await Assert.That(empty).IsEmpty();
        await Assert.That(single[0]).IsEqualTo(42);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
    }

    public static IEnumerable<int> SequenceSizes() => [2, 4, 8, 16, 32, 64, 128, 256, 1024];

    /// <summary>
    /// The network is BitonicSort's: the same phases and the same comparisons in the same order. Where BitonicSort
    /// exchanges an out-of-order pair, every comparator here writes the lower position and then the upper one,
    /// whether or not the pair was in order, and never swaps.
    /// </summary>
    [Test]
    [MethodDataSource(nameof(SequenceSizes))]
    public async Task EventSequenceMatchesBitonicSortTest(int n)
    {
        var input = TestHelpers.ShuffledRange(n, 42);
        var reference = new BitonicSortTests.NetworkEventRecorder();
        var expectedArray = input.ToArray();
        BitonicSort.Sort(expectedArray.AsSpan(), reference);

        var expected = new List<string>();
        foreach (var e in reference.Events)
        {
            if (e.StartsWith("swap ", StringComparison.Ordinal)) continue;
            expected.Add(e);
            if (e.StartsWith("compare ", StringComparison.Ordinal))
            {
                var parts = e.Split(' ');
                expected.Add($"write {parts[1]}");
                expected.Add($"write {parts[2]}");
            }
        }

        var recorder = new WriteRecorder();
        var actualArray = input.ToArray();
        BitonicSortBranchless.Sort(actualArray.AsSpan(), recorder);

        await Assert.That(recorder.Events).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Assert.That(actualArray).IsEquivalentTo(expectedArray, CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(2, 42)]
    [Arguments(16, 42)]
    [Arguments(16, 1234)]
    [Arguments(128, 42)]
    [Arguments(128, 1234)]
    public async Task TheoreticalValuesRandomTest(int n, int seed)
    {
        await AssertCounts(TestHelpers.ShuffledRange(n, seed));
    }

    [Test]
    [Arguments(2)]
    [Arguments(16)]
    [Arguments(128)]
    public async Task TheoreticalValuesSortedAndReversedTest(int n)
    {
        await AssertCounts(Enumerable.Range(0, n).ToArray());
        await AssertCounts(Enumerable.Range(0, n).Reverse().ToArray());
    }

    /// <summary>
    /// Every comparator reads two positions, compares once and writes both positions, whatever the input.
    /// </summary>
    static async Task AssertCounts(int[] array)
    {
        var stats = new StatisticsContext();
        BitonicSortBranchless.Sort(array.AsSpan(), stats);

        var n = array.Length;
        var log = 0;
        while (1 << log < n) log++;
        var comparators = (ulong)(n * log * (log + 1) / 4);

        await Assert.That(stats.CompareCount).IsEqualTo(comparators);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo(2 * comparators);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(2 * comparators);
    }

    public static IEnumerable<int> ZeroOneSizes() => [1, 2, 4, 8, 16];

    /// <summary>
    /// The zero-one principle on the unobserved path, which selects the minimum and maximum in a vector register.
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

            BitonicSortBranchless.Sort(bits.AsSpan());

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

    // Each type the unobserved vector path accepts, with its extremes, plus types that fall back to the comparison.
    [Test]
    public async Task PrimitiveKeysSortInNaturalOrderTest()
    {
        await AssertSortsLikeArraySort<byte>(i => (byte)(i * 37), [byte.MinValue, byte.MaxValue]);
        await AssertSortsLikeArraySort<sbyte>(i => (sbyte)(i * 37), [sbyte.MinValue, sbyte.MaxValue]);
        await AssertSortsLikeArraySort<short>(i => (short)(i * 7919), [short.MinValue, short.MaxValue]);
        await AssertSortsLikeArraySort<ushort>(i => (ushort)(i * 7919), [ushort.MinValue, ushort.MaxValue]);
        await AssertSortsLikeArraySort<int>(i => i * 1_000_003, [int.MinValue, int.MaxValue]);
        await AssertSortsLikeArraySort<uint>(i => (uint)i * 2_654_435_761u, [uint.MinValue, uint.MaxValue, 0x8000_0000u]);
        await AssertSortsLikeArraySort<long>(i => i * 6_364_136_223_846_793_005L, [long.MinValue, long.MaxValue]);
        await AssertSortsLikeArraySort<ulong>(i => (ulong)i * 11_400_714_819_323_198_485UL, [ulong.MinValue, ulong.MaxValue, 0x8000_0000_0000_0000UL]);
        await AssertSortsLikeArraySort<nint>(i => (nint)(i * 1_000_003), [nint.MinValue, nint.MaxValue]);
        await AssertSortsLikeArraySort<nuint>(i => (nuint)(uint)(i * 1_000_003), [nuint.MinValue, nuint.MaxValue]);
        await AssertSortsLikeArraySort<decimal>(i => i * 1.5m - 40m, [decimal.MinValue, decimal.MaxValue]);
        await AssertSortsLikeArraySort<char>(i => (char)('a' + i % 26), [char.MinValue, char.MaxValue]);
    }

    /// <summary>
    /// <see cref="IComparable{T}.CompareTo"/> orders NaN below every number and treats -0 and +0 as equal. The vector
    /// minimum and maximum of floating-point values do neither, so the unobserved path compares and blends instead.
    /// </summary>
    [Test]
    public async Task FloatingPointKeysFollowCompareToOrderTest()
    {
        double[] doubleSpecials = [double.NaN, double.NegativeInfinity, double.PositiveInfinity, -0.0, 0.0, double.Epsilon, -double.Epsilon, double.MaxValue, double.MinValue];
        await AssertSortsLikeArraySort<double>(i => (i * 7919 % 64) - 31.5, doubleSpecials);
        await AssertSortsLikeArraySort<double>(i => i % 3 == 0 ? double.NaN : i, []);

        float[] floatSpecials = [float.NaN, float.NegativeInfinity, float.PositiveInfinity, -0.0f, 0.0f, float.Epsilon, float.MaxValue, float.MinValue];
        await AssertSortsLikeArraySort<float>(i => (i * 7919 % 64) - 31.5f, floatSpecials);
        await AssertSortsLikeArraySort<float>(i => i % 3 == 0 ? float.NaN : i, []);

        Half[] halfSpecials = [Half.NaN, Half.NegativeInfinity, Half.PositiveInfinity, Half.NegativeZero, Half.Zero];
        await AssertSortsLikeArraySort<Half>(i => (Half)((i * 7919 % 64) - 31.5), halfSpecials);
    }

    [Test]
    public async Task StringKeysTest()
    {
        await AssertSortsLikeArraySort<string>(i => (i * 7919 % 101).ToString("D3"), ["", "zzz"]);
    }

    /// <summary>
    /// A custom comparer turns the vector path off, so the order is the comparer's, not the natural one.
    /// </summary>
    [Test]
    public async Task CustomComparerTest()
    {
        var input = TestHelpers.ShuffledRange(64, 42);
        var descending = Comparer<int>.Create((a, b) => b.CompareTo(a));

        var unobserved = input.ToArray();
        BitonicSortBranchless.Sort(unobserved.AsSpan(), descending, NullContext.Default);
        var observed = input.ToArray();
        BitonicSortBranchless.Sort(observed.AsSpan(), descending, new StatisticsContext());

        var expected = Enumerable.Range(0, 64).Reverse().ToArray();
        await Assert.That(unobserved).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Assert.That(observed).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    /// <summary>
    /// Sorts 64 generated keys followed by the given special values, padded by repeating the generator to the next
    /// power of two, on both paths, and compares with <see cref="Array.Sort{T}(T[])"/>.
    /// </summary>
    static async Task AssertSortsLikeArraySort<T>(Func<int, T> generate, T[] specials) where T : IComparable<T>
    {
        var length = 64;
        while (length < 64 + specials.Length) length *= 2;
        var input = new T[length];
        for (var i = 0; i < length; i++) input[i] = generate(i);
        specials.CopyTo(input, 0);
        new Random(7).Shuffle(input);

        var expected = input.ToArray();
        Array.Sort(expected);
        var unobserved = input.ToArray();
        BitonicSortBranchless.Sort(unobserved.AsSpan());
        var observed = input.ToArray();
        BitonicSortBranchless.Sort(observed.AsSpan(), new StatisticsContext());

        await Assert.That(unobserved).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"{typeof(T).Name} unobserved");
        await Assert.That(observed).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"{typeof(T).Name} observed");
    }

    /// <summary>
    /// Records phase announcements, comparisons, writes and swaps, in the order they happen, as text.
    /// </summary>
    sealed class WriteRecorder : ISortContext
    {
        public List<string> Events { get; } = [];

        public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0)
            => Events.Add(phase switch
            {
                SortPhase.BitonicLevel => $"level {param1} {param2}",
                SortPhase.BitonicStage => $"stage {param1} {param2} {param3}",
                _ => $"unexpected {phase}",
            });

        public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) => Events.Add($"compare {i} {j}");
        public void OnSwap(int i, int j, int bufferId) => Events.Add($"swap {i} {j}");
        public void OnIndexRead(int index, int bufferId) { }
        public void OnIndexWrite(int index, int bufferId) => Events.Add($"write {index}");
        public void OnIndexWrite<T>(int index, int bufferId, T value) => Events.Add($"write {index}");
        public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId) { }
        public void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values) { }
        public void OnRole(int index, int bufferId, RoleType role) { }
    }
}
