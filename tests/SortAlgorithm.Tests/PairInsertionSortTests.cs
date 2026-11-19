using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class PairInsertionSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => PairInsertionSort.Sort(span, context);

    // O(n^2) algorithm: keep data-driven inputs small.
    protected override int MaxOrderTestSize => 512;

    // A pair that is in order and stops at once stays where it is: no writes, and nothing is ever exchanged.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    public async Task OddElementsTest()
    {
        // Odd length pairs every element after the first
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 2 };

        PairInsertionSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(new[] { 1, 2, 3, 5, 8 }, CollectionOrdering.Matching);
    }

    [Test]
    public async Task EvenElementsTest()
    {
        // Even length leaves the last element without a partner
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 2, 0 };

        PairInsertionSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(new[] { 0, 1, 2, 3, 5, 8 }, CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(10)]
    [Arguments(11)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        PairInsertionSort.Sort(sorted.AsSpan(), stats);

        // Pairs start after the first element; an even n leaves the last element unpaired.
        // Each pair: 1 comparison orders it, the larger element stops at its predecessor (1),
        // and the smaller one compares with that same predecessor (1). Nothing moves.
        // The unpaired element stops at its predecessor (1).
        var pairs = (n - 1) / 2;
        var unpaired = (n - 1) % 2;
        var expectedCompares = (ulong)(pairs * 3 + unpaired);

        // One read per comparison, plus the second element of each pair and the held unpaired element.
        var expectedReads = expectedCompares + (ulong)(pairs + unpaired);

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    [Test]
    [Arguments(10)]
    [Arguments(11)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        PairInsertionSort.Sort(reversed.AsSpan(), stats);

        // Reversed input: every prefix element exceeds both elements of the pair.
        // A pair after a prefix of p elements: 1 comparison orders it, the larger element passes all p (p comparisons,
        // each element moved two places with one write), and the smaller one has nothing left to compare.
        // Both are then written: p + 2 writes. Straight insertion would compare and move the prefix twice.
        // The unpaired element passes the whole prefix of n - 1: n - 1 comparisons and n writes.
        ulong expectedCompares = 0, expectedWrites = 0, expectedReads = 0;
        var p = 1;
        for (; p + 1 < n; p += 2)
        {
            expectedCompares += (ulong)(1 + p);
            expectedWrites += (ulong)(p + 2);
            expectedReads += (ulong)(2 + p);
        }
        if (p < n)
        {
            expectedCompares += (ulong)p;
            expectedWrites += (ulong)(p + 1);
            expectedReads += (ulong)(1 + p);
        }

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL); // Insertion sort uses shifts, not swaps

        // About n²/4, half of straight insertion's n(n-1)/2
        await Assert.That(stats.CompareCount < (ulong)(n * (n - 1) / 2)).IsTrue();

        var expected = Enumerable.Range(0, n).ToArray();
        await Assert.That(reversed).IsEquivalentTo(expected, CollectionOrdering.Matching);
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
        var stats = new StatisticsContext();
        var random = TestHelpers.ShuffledRange(n, seed);
        PairInsertionSort.Sort(random.AsSpan(), stats);

        // Pair insertion on random data: average case about n²/6 comparisons and writes
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n * (n - 1) / 2);
        var maxWrites = (ulong)((n - 1) * (n + 2) / 2);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(0UL, maxWrites);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL); // Insertion sort uses shifts, not swaps

        // One read per comparison, plus the second element of each pair and the held unpaired element.
        var held = (ulong)((n - 1) / 2 + (n - 1) % 2);
        await Assert.That(stats.IndexReadCount).IsEqualTo(stats.CompareCount + held);
    }

    /// <summary>
    /// Elements greater than both elements of a pair are compared and moved once per pair, where straight insertion handles them
    /// once for each element. On random input that tends to two thirds of straight insertion's comparisons and writes; the few
    /// comparisons each pair adds keep small inputs above that.
    /// </summary>
    [Test]
    [Arguments(100, 42)]
    [Arguments(100, 1234)]
    [Arguments(1000, 42)]
    public async Task PairsShareTheirShifts(int n, int seed)
    {
        var input = TestHelpers.ShuffledRange(n, seed);
        var pair = new StatisticsContext();
        var straight = new StatisticsContext();
        PairInsertionSort.Sort(input.ToArray().AsSpan(), pair);
        InsertionSort.Sort(input.ToArray().AsSpan(), straight);

        // Below 4/5 even at n = 100, and below 7/10 by n = 1000.
        var (num, den) = n >= 1000 ? (7UL, 10UL) : (4UL, 5UL);
        await Assert.That(pair.CompareCount * den < straight.CompareCount * num).IsTrue()
            .Because($"pair {pair.CompareCount} vs straight {straight.CompareCount} comparisons");
        await Assert.That(pair.IndexWriteCount * den < straight.IndexWriteCount * num).IsTrue()
            .Because($"pair {pair.IndexWriteCount} vs straight {straight.IndexWriteCount} writes");
    }

    /// <summary>
    /// A tied pair and ties with the prefix: the later of two equal keys stays after the earlier one.
    /// </summary>
    [Test]
    public async Task TiedPairKeepsItsOrder()
    {
        var data = new[] { (3, 'a'), (1, 'b'), (1, 'c'), (3, 'd'), (3, 'e'), (1, 'f'), (2, 'g') };
        PairInsertionSort.Sort(data.AsSpan(), Comparer<(int Key, char Id)>.Create(static (x, y) => x.Key.CompareTo(y.Key)), NullContext.Default);

        await Assert.That(data.Select(x => x.Item2)).IsEquivalentTo(new[] { 'b', 'c', 'f', 'g', 'a', 'd', 'e' }, CollectionOrdering.Matching);
    }
}
