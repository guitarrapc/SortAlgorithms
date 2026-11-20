using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class DoubleSelectionSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => DoubleSelectionSort.Sort(span, context);

    // O(n^2) comparisons: skip large inputs in data-driven tests.
    protected override int MaxOrderTestSize => 512;

    // Sorted input keeps min at left and max at right each pass: no writes, no swaps.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        DoubleSelectionSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        DoubleSelectionSort.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        // Sort a single element range [2, 3)
        DoubleSelectionSort.Sort(array.AsSpan(), 2, 3, stats);

        // Array should be unchanged (single element is already sorted)
        await Assert.That(array).IsEquivalentTo([5, 3, 8, 1, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        // Sort only the first 5 elements [0, 5)
        DoubleSelectionSort.Sort(array.AsSpan(), 0, 5, stats);

        // Expected: first 5 sorted, last 4 unchanged
        await Assert.That(array).IsEquivalentTo([1, 3, 5, 7, 9, 2, 4, 6, 8], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        // Sort only the last 4 elements [5, 9)
        DoubleSelectionSort.Sort(array.AsSpan(), 5, 9, stats);

        // Expected: first 5 unchanged, last 4 sorted
        await Assert.That(array).IsEquivalentTo([1, 3, 5, 7, 9, 2, 4, 6, 8], CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseMinMaxAtBoundariesTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 2, 3, 4, 1 }; // max at left, min at right

        DoubleSelectionSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseMaxAtLeftTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 1, 2, 3, 4 }; // max at left

        DoubleSelectionSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5], CollectionOrdering.Matching);
    }

    [Test]
    public async Task EdgeCaseMinAtRightTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 2, 3, 4, 5, 1 }; // min at right

        DoubleSelectionSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5], CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(5)]
    [Arguments(6)]
    public async Task EdgeCaseAllEqualElementsTest(int n)
    {
        var stats = new StatisticsContext();
        var array = Enumerable.Repeat(5, n).ToArray();

        DoubleSelectionSort.Sort(array.AsSpan(), stats);

        // The minimum is the first smallest element and the maximum the last largest,
        // so every region of equal keys already has both at its ends.
        await Assert.That(array).IsEquivalentTo(Enumerable.Repeat(5, n).ToArray(), CollectionOrdering.Matching);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsEqualTo(ExpectedCompares(n));
    }

    [Test]
    public async Task MaximumIsLastLargestTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 2, 3 };

        DoubleSelectionSort.Sort(array.AsSpan(), stats);

        // Pass 1 keeps the 3 that already ends the region. Taking the first 3 would exchange two equal keys.
        // Pass 2 exchanges the remaining [3, 2].
        await Assert.That(array).IsEquivalentTo([1, 2, 3, 3], CollectionOrdering.Matching);
        await Assert.That(stats.SwapCount).IsEqualTo(1UL);
    }

    [Test]
    public async Task MinimumIsFirstSmallestTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 2, 1, 3, 1, 4 };

        DoubleSelectionSort.Sort(array.AsSpan(), stats);

        // Pass 1 takes the 1 at index 1, not the one at index 3, and moves it to the front (1 swap).
        // Pass 2 over [2, 3, 1] then places both extremes (2 swaps).
        await Assert.That(array).IsEquivalentTo([1, 1, 2, 3, 4], CollectionOrdering.Matching);
        await Assert.That(stats.SwapCount).IsEqualTo(3UL);
    }

    [Test]
    [Arguments(10)]
    [Arguments(11)]
    [Arguments(20)]
    [Arguments(51)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        DoubleSelectionSort.Sort(sorted.AsSpan(), stats);

        // Each pass finds the minimum at left and the maximum at right, so nothing moves.
        await Assert.That(stats.CompareCount).IsEqualTo(ExpectedCompares(n));
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo(ExpectedScanReads(n));
    }

    [Test]
    [Arguments(10)]
    [Arguments(11)]
    [Arguments(20)]
    [Arguments(51)]
    [Arguments(100)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        DoubleSelectionSort.Sort(reversed.AsSpan(), stats);

        // Each pass finds the maximum at left and the minimum at right; one swap places both
        // and leaves the inner region reversed for the next pass.
        var expectedSwaps = (ulong)(n / 2);

        await Assert.That(stats.CompareCount).IsEqualTo(ExpectedCompares(n));
        await Assert.That(stats.SwapCount).IsEqualTo(expectedSwaps);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(2 * expectedSwaps);
        await Assert.That(stats.IndexReadCount).IsEqualTo(ExpectedScanReads(n) + 2 * expectedSwaps);
    }

    [Test]
    [Arguments(10, 42)]
    [Arguments(10, 1234)]
    [Arguments(11, 42)]
    [Arguments(20, 42)]
    [Arguments(20, 1234)]
    [Arguments(50, 42)]
    [Arguments(50, 1234)]
    [Arguments(51, 1234)]
    [Arguments(100, 42)]
    [Arguments(100, 1234)]
    public async Task TheoreticalValuesRandomTest(int n, int seed)
    {
        var stats = new StatisticsContext();
        var random = TestHelpers.ShuffledRange(n, seed);
        DoubleSelectionSort.Sort(random.AsSpan(), stats);

        // The comparison count does not depend on the input; each of the n/2 passes swaps at most twice.
        await Assert.That(stats.CompareCount).IsEqualTo(ExpectedCompares(n));
        await Assert.That(stats.SwapCount).IsBetween(0UL, (ulong)(2 * (n / 2)));
        await Assert.That(stats.IndexReadCount).IsEqualTo(ExpectedScanReads(n) + 2 * stats.SwapCount);
    }

    // Pass regions shrink by two. A pair costs three comparisons: odd k takes 3(k-1)/2, even k takes 3k/2 - 2.
    static ulong ExpectedCompares(int n)
    {
        var total = 0UL;
        for (var k = n; k >= 2; k -= 2)
        {
            total += (ulong)(k % 2 == 1 ? 3 * (k - 1) / 2 : 3 * k / 2 - 2);
        }
        return total;
    }

    // For int, a pair comparison reads both elements and each candidate comparison reads its element again.
    // An odd region also reads its first element once (2k - 1 reads), an even one its first pair (2k - 2).
    static ulong ExpectedScanReads(int n)
    {
        var total = 0UL;
        for (var k = n; k >= 2; k -= 2)
        {
            total += (ulong)(k % 2 == 1 ? 2 * k - 1 : 2 * k - 2);
        }
        return total;
    }
}
