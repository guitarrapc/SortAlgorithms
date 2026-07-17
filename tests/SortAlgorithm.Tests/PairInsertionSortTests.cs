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

    // Pair insertion reads pairs upfront and writes both elements back even on sorted input; never swaps.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    public async Task OddElementsTest()
    {
        // Odd length exercises the single-element tail after pair processing
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 2 };

        PairInsertionSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(new[] { 1, 2, 3, 5, 8 }, CollectionOrdering.Matching);
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
        PairInsertionSort.Sort(sorted.AsSpan(), stats);

        // Pair Insertion Sort on sorted data: best case O(n)
        // Process pairs (i, i+1) for i = 1, 3, 5, ...
        // For each pair on sorted data:
        //   1. Compare a with b: 1 comparison
        //   2. Compare j with a (where j = i-1): 1 comparison (no shift needed)
        //   3. Compare j with b (where j = i): 1 comparison (no shift needed)
        //   4. Write a to its position: 1 write
        //   5. Write b to its position: 1 write
        // Number of pairs: floor(n/2) - 1 (excluding first element, which is trivially sorted)
        // If n is odd, last element needs 1 comparison + 0 writes (already in position)

        var numPairs = (n - 1) / 2;  // Pairs starting from index 1
        var hasOdd = (n - 1) % 2 == 1;

        // Each pair: 1 (a vs b) + 1 (j vs a) + 1 (j vs b) = 3 comparisons
        // Odd element: 1 comparison
        var expectedCompares = (ulong)(numPairs * 3 + (hasOdd ? 1 : 0));

        // Each pair writes both elements even if already sorted (read them upfront)
        // Odd element: no write if already sorted
        var expectedWrites = (ulong)(numPairs * 2);

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
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
        PairInsertionSort.Sort(reversed.AsSpan(), stats);

        // Pair Insertion Sort on reversed data: worst case O(n^2)
        // Due to pair processing overhead, slightly more comparisons than standard insertion sort
        // Empirical observation: approximately n*(n-1)/2 + small overhead from pair comparisons
        var minCompares = (ulong)(n * (n - 1) / 2);
        var maxCompares = (ulong)(n * (n - 1) / 2 + n);  // Allow some overhead

        await Assert.That(stats.CompareCount >= minCompares).IsTrue()
            .Because($"CompareCount ({stats.CompareCount}) should be >= {minCompares}");
        await Assert.That(stats.CompareCount <= maxCompares).IsTrue()
            .Because($"CompareCount ({stats.CompareCount}) should be <= {maxCompares}");
        await Assert.That(stats.IndexWriteCount > 0UL).IsTrue();
        await Assert.That(stats.SwapCount).IsEqualTo(0UL); // Insertion sort uses shifts, not swaps

        // Verify array is sorted
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

        // Insertion Sort on random data: average case O(n^2)
        // - Average comparisons: approximately n^2/4
        // - Comparisons range from best case (n-1) to worst case (n(n-1)/2)
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n * (n - 1) / 2);

        // Writes vary based on how many elements need to be shifted
        var minWrites = 0UL; // Best case (already sorted by chance)
        var maxWrites = (ulong)((n - 1) * (n + 2) / 2); // Worst case (reversed)

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL); // Insertion sort uses shifts, not swaps
    }
}
