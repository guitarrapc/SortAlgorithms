using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

/// <summary>
/// Shared tests for a <c>*Branchless</c> sorting network that accepts any length. Such a class runs the network of a
/// conditional-exchange class, but each comparator writes its minimum and maximum to both positions. Derived classes
/// name both sorts; the tests check that the network is the same and that every comparator reads twice, compares once
/// and writes twice.
/// </summary>
[InheritsTests]
public abstract class BranchlessNetworkTestsBase : SortTestsBase
{
    /// <summary>Invokes the branchless sort under test with an explicit comparer.</summary>
    protected abstract void SortWith<T, TComparer, TContext>(Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext;

    /// <summary>Invokes the conditional-exchange sort that runs the same network.</summary>
    protected abstract void ConditionalExchangeSort<T, TContext>(Span<T> span, TContext context)
        where T : IComparable<T>
        where TContext : ISortContext;

    // Every comparator writes both positions, so sorted input writes as much as any other, and nothing is swapped.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    public static IEnumerable<int> SequenceSizes() => [.. Enumerable.Range(2, 34), 63, 64, 65, 100, 255, 256, 257, 1000, 1024];

    /// <summary>
    /// The phases and the comparisons, in order, are the conditional-exchange sort's. Where that sort exchanges an
    /// out-of-order pair, every comparator here writes the lower position and then the upper one, whether or not the
    /// pair was in order, and never swaps. Both leave the same array.
    /// </summary>
    [Test]
    [MethodDataSource(typeof(BranchlessNetworkTestsBase), nameof(SequenceSizes))]
    public async Task EventSequenceMatchesConditionalExchangeTest(int n)
    {
        var input = TestHelpers.ShuffledRange(n, 42);

        var reference = new EventRecorder();
        var expectedArray = input.ToArray();
        ConditionalExchangeSort(expectedArray.AsSpan(), reference);

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

        var recorder = new EventRecorder();
        var actualArray = input.ToArray();
        Sort(actualArray.AsSpan(), recorder);

        await Assert.That(recorder.Events).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Assert.That(actualArray).IsEquivalentTo(expectedArray, CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(2)]
    [Arguments(3)]
    [Arguments(5)]
    [Arguments(16)]
    [Arguments(100)]
    [Arguments(1000)]
    public async Task OperationCountsTest(int n)
    {
        int[][] inputs =
        [
            TestHelpers.ShuffledRange(n, 42),
            TestHelpers.ShuffledRange(n, 1234),
            [.. Enumerable.Range(0, n)],
            [.. Enumerable.Range(0, n).Reverse()],
        ];

        foreach (var input in inputs)
        {
            var reference = new StatisticsContext();
            ConditionalExchangeSort(input.ToArray().AsSpan(), reference);
            var stats = new StatisticsContext();
            Sort(input.ToArray().AsSpan(), stats);

            var comparators = reference.CompareCount;
            await Assert.That(stats.CompareCount).IsEqualTo(comparators);
            await Assert.That(stats.SwapCount).IsEqualTo(0UL);
            await Assert.That(stats.IndexReadCount).IsEqualTo(2 * comparators);
            await Assert.That(stats.IndexWriteCount).IsEqualTo(2 * comparators);
        }
    }

    public static IEnumerable<int> ZeroOneSizes() => Enumerable.Range(1, 16);

    /// <summary>
    /// The zero-one principle on the unobserved path, which selects the minimum and maximum in a vector register,
    /// including lengths that are not powers of two.
    /// </summary>
    [Test]
    [MethodDataSource(typeof(BranchlessNetworkTestsBase), nameof(ZeroOneSizes))]
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

            Sort(bits.AsSpan(), NullContext.Default);

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

    /// <summary>
    /// A custom comparer turns the vector path off, so the order is the comparer's, not the natural one.
    /// </summary>
    [Test]
    public async Task CustomComparerTest()
    {
        var input = TestHelpers.ShuffledRange(100, 42);
        var descending = Comparer<int>.Create((a, b) => b.CompareTo(a));

        var unobserved = input.ToArray();
        SortWith(unobserved.AsSpan(), descending, NullContext.Default);
        var observed = input.ToArray();
        SortWith(observed.AsSpan(), descending, new StatisticsContext());

        var expected = Enumerable.Range(0, 100).Reverse().ToArray();
        await Assert.That(unobserved).IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Assert.That(observed).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    /// <summary>
    /// Sorts 64 generated keys and the given special values on both paths and compares with
    /// <see cref="Array.Sort{T}(T[])"/>. The length is rarely a power of two, so the truncated network is exercised too.
    /// </summary>
    async Task AssertSortsLikeArraySort<T>(Func<int, T> generate, T[] specials) where T : IComparable<T>
    {
        var input = new T[64 + specials.Length];
        for (var i = 0; i < input.Length; i++) input[i] = generate(i);
        specials.CopyTo(input, 0);
        new Random(7).Shuffle(input);

        var expected = input.ToArray();
        Array.Sort(expected);
        var unobserved = input.ToArray();
        Sort(unobserved.AsSpan(), NullContext.Default);
        var observed = input.ToArray();
        Sort(observed.AsSpan(), new StatisticsContext());

        await Assert.That(unobserved).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"{typeof(T).Name} unobserved");
        await Assert.That(observed).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"{typeof(T).Name} observed");
    }

    /// <summary>
    /// Records phase announcements, comparisons, writes and swaps, in the order they happen, as text.
    /// </summary>
    sealed class EventRecorder : ISortContext
    {
        public List<string> Events { get; } = [];

        public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0) => Events.Add($"{phase} {param1} {param2} {param3}");
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
