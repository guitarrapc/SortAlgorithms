using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class BottomupHeapSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => BottomupHeapSort.Sort(span, context);

    // Every sift moves elements through a hole, so even sorted input is written, but nothing is swapped.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        BottomupHeapSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        BottomupHeapSort.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // Sort a single element range [2, 3)
        BottomupHeapSort.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        await Assert.That(array).IsEquivalentTo([5, 3, 8, 1, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        // Sort only the first 5 elements [0, 5)
        BottomupHeapSort.Sort(array.AsSpan(), 0, 5, stats);

        // Expected: first 5 sorted, last 4 unchanged
        await Assert.That(array).IsEquivalentTo([1, 3, 5, 7, 9, 2, 4, 6, 8], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        // Sort only the last 4 elements [5, 9)
        BottomupHeapSort.Sort(array.AsSpan(), 5, 9, stats);

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
        BottomupHeapSort.Sort(sorted.AsSpan(), stats);

        // Empirical observations for sorted data:
        // n=10:  Compare=33,  Write=48
        // n=20:  Compare=88,  Write=113
        // n=50:  Compare=286, Write=346
        // n=100: Compare=688, Write=801
        await AssertBottomUpCounts(stats, n);
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
        BottomupHeapSort.Sort(reversed.AsSpan(), stats);

        // Empirical observations for reversed data:
        // n=10:  Compare=36,  Write=55
        // n=20:  Compare=94,  Write=129
        // n=50:  Compare=314, Write=397
        // n=100: Compare=724, Write=887
        await AssertBottomUpCounts(stats, n);
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
        BottomupHeapSort.Sort(random.AsSpan(), stats);

        // Empirical observations for random data:
        // n=10:  Compare=32-34,   Write=46-49
        // n=20:  Compare=92,      Write=121-122
        // n=50:  Compare=289-297, Write=349-354
        // n=100: Compare=693-705, Write=805-824
        await AssertBottomUpCounts(stats, n);
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesAllEqualTest(int n)
    {
        var stats = new StatisticsContext();
        var equal = new int[n];
        BottomupHeapSort.Sort(equal.AsSpan(), stats);

        // Every sift descends to a leaf before it compares the sifted element, so equal keys
        // cost as much as distinct ones. HeapSort's top-down sift stops at once here (3n - 6 comparisons).
        // n=10:  Compare=33
        // n=100: Compare=711
        await AssertBottomUpCounts(stats, n);
    }

    // One comparison per level on the way down and a short climb give about n log n comparisons;
    // top-down sift-down needs about twice as many. Elements move through a hole, so nothing is swapped.
    static async Task AssertBottomUpCounts(StatisticsContext stats, int n)
    {
        var logN = Math.Log(n + 1, 2);
        var minCompares = (ulong)(n * logN * 0.8);
        var maxCompares = (ulong)(n * logN * 1.2);

        // n - 1 root-to-end writes plus one write per level of every descent
        var minWrites = (ulong)(n * logN);
        var maxWrites = (ulong)(n * logN * 2.0);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minCompares * 2).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minCompares * 2}");
    }
}
