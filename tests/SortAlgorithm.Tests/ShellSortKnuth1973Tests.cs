using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

// Tests using Knuth1973 - the classic and most well-known gap sequence
[InheritsTests]
public class ShellSortKnuth1973Tests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => ShellSortKnuth1973.Sort(span, context);

    // Sorted input needs no element movement in any gap pass: no writes, no swaps.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        ShellSortKnuth1973.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        ShellSortKnuth1973.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // Sort a single element range [2, 3)
        ShellSortKnuth1973.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        await Assert.That(array).IsEquivalentTo([5, 3, 8, 1, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        // Sort only the first 5 elements [0, 5)
        ShellSortKnuth1973.Sort(array.AsSpan(), 0, 5, stats);

        // Expected: first 5 sorted, last 4 unchanged
        await Assert.That(array).IsEquivalentTo([1, 3, 5, 7, 9, 2, 4, 6, 8], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        // Sort only the last 4 elements [5, 9)
        ShellSortKnuth1973.Sort(array.AsSpan(), 5, 9, stats);

        // Expected: first 5 unchanged, last 4 sorted
        await Assert.That(array).IsEquivalentTo([1, 3, 5, 7, 9, 2, 4, 6, 8], CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        var expected = ShellSortSwapForm.Count(sorted, ShellGapSequences.Knuth);
        ShellSortKnuth1973.Sort(sorted.AsSpan(), stats);

        // Every held element stops at its first comparison, so nothing moves and each comparison pairs with one held element.
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsEqualTo(expected.Compares);
        await Assert.That(stats.CompareCount >= (ulong)(n - 1)).IsTrue().Because($"CompareCount ({stats.CompareCount}) should cover the final h=1 pass");
        await Assert.That(stats.IndexReadCount).IsEqualTo(stats.CompareCount * 2);
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        var expected = ShellSortSwapForm.Count(reversed, ShellGapSequences.Knuth);
        ShellSortKnuth1973.Sort(reversed.AsSpan(), stats);

        // These checks pin the operations of each pass; they do not identify a worst-case input.
        // Holding the element makes the comparisons of the exchanging form, but writes once per move and once to place the element.
        await Assert.That(stats.CompareCount).IsEqualTo(expected.Compares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expected.Exchanges + expected.Moved);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expected.Compares + expected.Held);
        await Assert.That(expected.Exchanges >= 1UL).IsTrue();
        await Assert.That(reversed).IsEquivalentTo(Enumerable.Range(0, n), CollectionOrdering.Matching);
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
        var expected = ShellSortSwapForm.Count(random, ShellGapSequences.Knuth);
        ShellSortKnuth1973.Sort(random.AsSpan(), stats);

        // Random input checks read/write accounting.
        // A finite sample does not establish an average-case exponent or rank gap sequences.
        await Assert.That(stats.CompareCount).IsEqualTo(expected.Compares);
        await Assert.That(stats.CompareCount >= (ulong)(n - 1)).IsTrue().Because($"CompareCount ({stats.CompareCount}) should cover the final h=1 pass");
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expected.Exchanges + expected.Moved);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expected.Compares + expected.Held);
    }
}
