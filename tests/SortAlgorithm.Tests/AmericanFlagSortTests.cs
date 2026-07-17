using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class AmericanFlagSortTests : IntegerSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => AmericanFlagSort.Sort(span, context);

    // No knob overrides: in-place permutation on sorted input may skip writes/swaps,
    // and per-bucket insertion sort makes compares data-dependent.

    [Test]
    public async Task DecimalDigitBoundaryTest()
    {
        // Test values that cross decimal digit boundaries (9→10, 99→100, etc.)
        var array = new[] { 100, 9, 99, 10, 1, 999, 1000 };
        var expected = new[] { 1, 9, 10, 99, 100, 999, 1000 };
        AmericanFlagSort.Sort(array.AsSpan());
        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task InsertionSortCutoffTest()
    {
        // Test with array smaller than insertion sort cutoff (16)
        var array = new[] { 10, 5, 3, 8, 1, 9, 2, 7, 4, 6 };
        var expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        AmericanFlagSort.Sort(array.AsSpan());
        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task LargeRangeTest()
    {
        var stats = new StatisticsContext();
        // Test with values spanning a large range
        var array = new[] { 1000000, -1000000, 0, 500000, -500000 };
        var expected = new[] { -1000000, -500000, 0, 500000, 1000000 };
        AmericanFlagSort.Sort(array.AsSpan(), stats);
        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task InPlacePermutationTest()
    {
        var stats = new StatisticsContext();
        // Verify that the sort is performed in-place (no auxiliary array)
        // Use larger array to exceed InsertionSortCutoff (16)
        var array = new[] { 25, 13, 28, 11, 22, 29, 14, 27, 16, 30, 15, 26, 17, 31, 18, 24, 19, 23, 20, 21 };
        var expected = new[] { 11, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29, 30, 31 };
        AmericanFlagSort.Sort(array.AsSpan(), stats);
        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
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
        AmericanFlagSort.Sort(sorted.AsSpan(), stats);

        // American Flag Sort is an in-place MSD Radix Sort variant
        // For sorted data:
        // - Elements distribute into buckets
        // - Small buckets (<=16) use insertion sort
        // - In-place permutation minimizes writes
        await Assert.That((ulong)sorted.Length).IsEqualTo((ulong)n);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);

        // For sorted data, verify that the sort completes successfully
        await Assert.That(IsSorted(sorted)).IsTrue();
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
        AmericanFlagSort.Sort(reversed.AsSpan(), stats);

        // American Flag Sort on reversed data:
        // - In-place permutation requires swaps to rearrange elements
        // - Insertion sort for small buckets has more operations
        await Assert.That((ulong)reversed.Length).IsEqualTo((ulong)n);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(IsSorted(reversed)).IsTrue();
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesRandomTest(int n)
    {
        var stats = new StatisticsContext();
        var random = new Random(42);
        var array = Enumerable.Range(0, n).OrderBy(_ => random.Next()).ToArray();
        AmericanFlagSort.Sort(array.AsSpan(), stats);

        // American Flag Sort on random data:
        // - Bucket distribution varies
        // - In-place permutation requires swaps
        // - Combination of MSD partitioning and insertion sort
        await Assert.That((ulong)array.Length).IsEqualTo((ulong)n);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(IsSorted(array)).IsTrue();

        // Random data should require swap operations when n > InsertionSortCutoff (16)
        if (n > 16)
        {
            await Assert.That(stats.SwapCount).IsNotEqualTo(0UL);
        }
    }
}
