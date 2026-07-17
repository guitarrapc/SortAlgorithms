using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class SmoothSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => SmoothSort.Sort(span, context);

    // SmoothSort on already-sorted input is O(n) with no element movement: no writes, no swaps.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        SmoothSort.Sort(sorted.AsSpan(), stats);

        // Smooth Sort achieves O(n) for sorted data
        // It should perform comparisons proportional to n (not n log n)
        // Best case: approximately n comparisons
        // Each comparison involves reading elements
        var minCompares = (ulong)n;
        var maxCompares = (ulong)(n * Math.Log(n, 2) * 2); // Allow some overhead

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexReadCount >= stats.CompareCount).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= CompareCount ({stats.CompareCount})");

        // Sorted data should have minimal operations
        await Assert.That(stats.CompareCount < (ulong)(n * n / 2)).IsTrue().Because($"Sorted data should be O(n), not O(n²). CompareCount: {stats.CompareCount}, n²/2: {n * n / 2}");
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
        SmoothSort.Sort(reversed.AsSpan(), stats);

        // Smooth Sort has O(n log n) worst case
        // For reversed data, it should perform more comparisons than sorted
        var minCompares = (ulong)(n * Math.Log(n, 2));
        var maxCompares = (ulong)(n * Math.Log(n, 2) * 4); // Allow overhead for Leonardo heap operations

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexReadCount >= stats.CompareCount).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= CompareCount ({stats.CompareCount})");
        await Assert.That(stats.IndexWriteCount >= stats.SwapCount * 2).IsTrue().Because($"IndexWriteCount ({stats.IndexWriteCount}) should be >= SwapCount * 2 ({stats.SwapCount * 2})");
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
        SmoothSort.Sort(random.AsSpan(), stats);

        // Smooth Sort has O(n log n) average case
        // Random data should fall between best (O(n)) and worst (O(n log n)) cases
        var minCompares = (ulong)n; // Best case
        var maxCompares = (ulong)(n * Math.Log(n, 2) * 4); // Worst case with overhead

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexReadCount >= stats.CompareCount).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= CompareCount ({stats.CompareCount})");
    }
}
