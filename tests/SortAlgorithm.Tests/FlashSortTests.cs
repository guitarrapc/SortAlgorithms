using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class FlashSortTests : IntegerSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => FlashSort.Sort(span, context);

    // Classification always redistributes elements, so writes occur even on sorted input;
    // compares/swaps are not pinned (classification is arithmetic, not comparison).
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;

    [Test]
    public async Task SortNativeIntegerTypesTest()
    {
        // nint/nuint coverage carried over from the old SortDifferentIntegerTypes
        // (the base class covers fixed-width integer types only).
        var stats = new StatisticsContext();
        var signed = new nint[] { -5, 2, -8, 1, 9 };
        Sort(signed.AsSpan(), stats);
        await Assert.That(IsSorted(signed)).IsTrue();

        var unsigned = new nuint[] { 5, 2, 8, 1, 9 };
        Sort(unsigned.AsSpan(), stats);
        await Assert.That(IsSorted(unsigned)).IsTrue();
    }

    /// <summary>
    /// クラス定数 c1 は double なので、64 ビット幅のキー差は変換で下位ビットを失う。それでもクラス番号は [0, m) に収まり、
    /// キーが大きくなって減ることはない。キー空間の両端と、最大値の直下に下位ビットだけが異なる密集を置いて、両方の性質を確かめる。
    /// </summary>
    [Test]
    [Arguments(2, 1)]
    [Arguments(1000, 42)]
    [Arguments(5000, 1234)]
    public async Task FullWidthKeysTest(int n, int seed)
    {
        var random = new Random(seed);
        var signed = new long[n];
        for (var i = 0; i < n; i++) signed[i] = random.NextInt64(long.MinValue, long.MaxValue);
        for (var i = 1; i < n / 2; i++) signed[i] = long.MaxValue - random.Next(0, 1000);
        signed[0] = long.MinValue;
        signed[^1] = long.MaxValue;
        var expectedSigned = signed.Order().ToArray();
        FlashSort.Sort(signed.AsSpan());
        await Assert.That(signed).IsEquivalentTo(expectedSigned, CollectionOrdering.Matching);

        var unsigned = new ulong[n];
        for (var i = 0; i < n; i++) unsigned[i] = (ulong)random.NextInt64(long.MinValue, long.MaxValue);
        for (var i = 1; i < n / 2; i++) unsigned[i] = ulong.MaxValue - (ulong)random.Next(0, 1000);
        unsigned[0] = ulong.MinValue;
        unsigned[^1] = ulong.MaxValue;
        var expectedUnsigned = unsigned.Order().ToArray();
        FlashSort.Sort(unsigned.AsSpan());
        await Assert.That(unsigned).IsEquivalentTo(expectedUnsigned, CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        FlashSort.Sort(sorted.AsSpan(), stats);

        // FlashSort on sorted data (n > InsertionSortThreshold=16, uniform distribution):
        //
        // Swaps: exactly 1 — always from s.Swap(maxIdx, 0) that moves the maximum element to
        //   index 0 to anchor the permutation cycle (counted even if maxIdx == 0).
        //
        // Comparisons: come only from insertion sort within each class (not from the permutation phase).
        //   With m=⌊0.43n⌋ classes and ~n/m ≈ 2.3 elements per class, total < n.
        //   Observations: n=20 → 17, n=50 → 38, n=100 → 72.
        //
        // Writes: initial swap (2) + permutation writes (~n-1) + insertion sort writes (~n) ≈ 2n.
        //   Observations: n=20 → 41, n=50 → 101, n=100 → 201.
        //
        // Reads: min/max scan (n) + count scan (n) + permutation + insertion sort ≥ 2n.
        //   Observations: n=20 → 122, n=50 → 306, n=100 → 612.
        await Assert.That(stats.SwapCount).IsEqualTo(1UL);
        await Assert.That(stats.CompareCount).IsBetween(0UL, (ulong)n);
        await Assert.That(stats.IndexWriteCount).IsBetween((ulong)n, (ulong)(4 * n));
        await Assert.That(stats.IndexReadCount).IsBetween((ulong)(2 * n), (ulong)(10 * n));
    }

    [Test]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        FlashSort.Sort(reversed.AsSpan(), stats);

        // FlashSort on reversed data (n > InsertionSortThreshold=16, uniform distribution):
        // Class assignment depends only on key values, not their order, so the statistical
        // profile is similar to sorted data despite the different element arrangement.
        //
        // Swaps: exactly 1 — s.Swap(maxIdx, 0); for reversed input maxIdx=0, a self-swap.
        // Comparisons: < n (from per-class insertion sort; uniform key distribution).
        //   Observations: n=20 → 15, n=50 → 34, n=100 → 65.
        // Writes: ~2n.  Observations: n=20 → 43, n=50 → 97, n=100 → 184.
        // Reads: ≥ 2n.  Observations: n=20 → 106, n=50 → 258, n=100 → 514.
        await Assert.That(stats.SwapCount).IsEqualTo(1UL);
        await Assert.That(stats.CompareCount).IsBetween(0UL, (ulong)n);
        await Assert.That(stats.IndexWriteCount).IsBetween((ulong)n, (ulong)(4 * n));
        await Assert.That(stats.IndexReadCount).IsBetween((ulong)(2 * n), (ulong)(10 * n));
    }

    [Test]
    [Arguments(20, 42)]
    [Arguments(20, 1234)]
    [Arguments(50, 42)]
    [Arguments(50, 1234)]
    [Arguments(100, 42)]
    [Arguments(100, 1234)]
    public async Task TheoreticalValuesRandomTest(int n, int seed)
    {
        var stats = new StatisticsContext();
        var random = TestHelpers.ShuffledRange(n, seed);
        FlashSort.Sort(random.AsSpan(), stats);

        // FlashSort on random data (n > InsertionSortThreshold=16, uniform distribution):
        // Range(0, n) shuffled has the same key-value set as sorted/reversed, so class
        // distribution is identical and the statistics fall in the same range.
        //
        // Swaps: exactly 1 (initial s.Swap(maxIdx, 0)).
        // Comparisons: < n (per-class insertion sort with ~2.3 elements per class on average).
        //   Observations: n=20 → 15-16, n=50 → 35-36, n=100 → 65-68.
        // Writes: ~2n.  Observations: n=20 → 38-43, n=50 → 85-92, n=100 → 171-179.
        // Reads: ≥ 2n.  Observations: n=20 → 92-100, n=50 → 218-271, n=100 → 472-493.
        await Assert.That(stats.SwapCount).IsEqualTo(1UL);
        await Assert.That(stats.CompareCount).IsBetween(0UL, (ulong)n);
        await Assert.That(stats.IndexWriteCount).IsBetween((ulong)n, (ulong)(4 * n));
        await Assert.That(stats.IndexReadCount).IsBetween((ulong)(2 * n), (ulong)(10 * n));
    }
}
