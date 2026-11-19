using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class WeakHeapSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => WeakHeapSort.Sort(span, context);

    // The build swaps whenever a node beats its distinguished ancestor, and the final swap is unconditional.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.NonZero;

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        WeakHeapSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        WeakHeapSort.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // Sort a single element range [2, 3)
        WeakHeapSort.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        await Assert.That(array).IsEquivalentTo([5, 3, 8, 1, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        // Sort only the first 5 elements [0, 5)
        WeakHeapSort.Sort(array.AsSpan(), 0, 5, stats);

        // Expected: first 5 sorted, last 4 unchanged
        await Assert.That(array).IsEquivalentTo([1, 3, 5, 7, 9, 2, 4, 6, 8], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        // Sort only the last 4 elements [5, 9)
        WeakHeapSort.Sort(array.AsSpan(), 5, 9, stats);

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
        WeakHeapSort.Sort(sorted.AsSpan(), stats);

        await AssertWeakHeapCounts(stats, n);
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
        WeakHeapSort.Sort(reversed.AsSpan(), stats);

        await AssertWeakHeapCounts(stats, n);
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
        WeakHeapSort.Sort(random.AsSpan(), stats);

        await AssertWeakHeapCounts(stats, n);
    }

    [Test]
    [Arguments(10)]
    [Arguments(100)]
    public async Task TheoreticalValuesAllEqualTest(int n)
    {
        var stats = new StatisticsContext();
        var equal = new int[n];
        WeakHeapSort.Sort(equal.AsSpan(), stats);

        // No merge finds a larger element, so the build swaps nothing and no extraction merge moves anything.
        // What remains is the max and the root written once per extraction, and the final swap.
        await AssertWeakHeapCounts(stats, n);
        await Assert.That(stats.SwapCount).IsEqualTo(1UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo((ulong)(2 * (n - 2) + 2));
    }

    // The build merges every node with its distinguished ancestor: n - 1 comparisons and at most one swap each.
    // Extracting from a heap of m elements merges the root with every node of the distinguished path, whose length
    // lies between ⌈log2(m + 1)⌉ - 1 and ⌈log2 m⌉. Extraction writes the max and the root once each, plus one write
    // per merge that moves an element, and never swaps. The final swap of the last two elements is unconditional.
    static async Task AssertWeakHeapCounts(StatisticsContext stats, int n)
    {
        ulong minPath = 0, maxPath = 0;
        for (var m = 2; m < n; m++)
        {
            minPath += (ulong)(CeilLog2(m + 1) - 1);
            maxPath += (ulong)CeilLog2(m);
        }

        await Assert.That(stats.CompareCount).IsBetween((ulong)(n - 1) + minPath, (ulong)(n - 1) + maxPath);
        await Assert.That(stats.SwapCount).IsBetween(1UL, (ulong)n);

        var extractionMerges = stats.CompareCount - (ulong)(n - 1);
        var fixedWrites = 2 * stats.SwapCount + 2 * (ulong)(n - 2);
        await Assert.That(stats.IndexWriteCount).IsBetween(fixedWrites, fixedWrites + extractionMerges);
    }

    static int CeilLog2(int x) => x <= 1 ? 0 : 32 - int.LeadingZeroCount(x - 1);
}
