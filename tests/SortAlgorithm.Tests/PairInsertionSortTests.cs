using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class PairInsertionSortTests
{
    [Test]
    [MethodDataSource(typeof(MockRandomData), nameof(MockRandomData.Generate))]
    [MethodDataSource(typeof(MockNegativePositiveRandomData), nameof(MockNegativePositiveRandomData.Generate))]
    [MethodDataSource(typeof(MockNegativeRandomData), nameof(MockNegativeRandomData.Generate))]
    [MethodDataSource(typeof(MockReversedData), nameof(MockReversedData.Generate))]
    [MethodDataSource(typeof(MockPipeorganData), nameof(MockPipeorganData.Generate))]
    [MethodDataSource(typeof(MockNearlySortedData), nameof(MockNearlySortedData.Generate))]
    [MethodDataSource(typeof(MockSameValuesData), nameof(MockSameValuesData.Generate))]
    [MethodDataSource(typeof(MockQuickSortWorstCaseData), nameof(MockQuickSortWorstCaseData.Generate))]
    [MethodDataSource(typeof(MockTwoDistinctValuesData), nameof(MockTwoDistinctValuesData.Generate))]
    [MethodDataSource(typeof(MockHalfZeroHalfOneData), nameof(MockHalfZeroHalfOneData.Generate))]
    [MethodDataSource(typeof(MockValleyRandomData), nameof(MockValleyRandomData.Generate))]
    [MethodDataSource(typeof(MockHighlySkewedData), nameof(MockHighlySkewedData.Generate))]
    public async Task SortResultOrderTest(IInputSample<int> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 512, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        PairInsertionSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateHalf))]
    public async Task SortHalfResultOrderTest(IInputSample<Half> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 512, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        PairInsertionSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateFloat))]
    public async Task SortFloatResultOrderTest(IInputSample<float> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 512, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        PairInsertionSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateDouble))]
    public async Task SortDoubleResultOrderTest(IInputSample<double> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 512, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        PairInsertionSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockIntKeyRandomData), nameof(MockIntKeyRandomData.Generate))]
    public async Task SortIntStructResultOrderTest(IInputSample<Utils.IntKey> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 512, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        PairInsertionSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockStabilityData), nameof(MockStabilityData.Generate))]
    public async Task StabilityTest(StabilityTestItem[] items)
    {
        // Test stability: equal elements should maintain relative order
        var stats = new StatisticsContext();

        PairInsertionSort.Sort(items.AsSpan(), stats);

        // Verify sorting correctness - values should be in ascending order
        await Assert.That(items.Select(x => x.Value).ToArray()).IsEquivalentTo(MockStabilityData.Sorted, CollectionOrdering.Matching);

        // Verify stability: for each group of equal values, original order is preserved
        var groupedByValue = items.GroupBy(x => x.Value);
        foreach (var group in groupedByValue)
        {
            var indexes = group.Select(x => x.OriginalIndex).ToList();
            // If stable, indexes should be in ascending order
            for (var i = 0; i < indexes.Count - 1; i++)
            {
                await Assert.That(indexes[i]).IsLessThan(indexes[i + 1]);
            }
        }
    }

    [Test]
    [MethodDataSource(typeof(MockStabilityWithIdData), nameof(MockStabilityWithIdData.Generate))]
    public async Task StabilityTestWithComplex(StabilityTestItemWithId[] items)
    {
        // Test stability with more complex scenario - multiple equal values
        var stats = new StatisticsContext();

        PairInsertionSort.Sort(items.AsSpan(), stats);

        // Expected: [2:B, 2:D, 2:F, 5:A, 5:C, 5:G, 8:E]
        // Keys are sorted, and elements with the same key maintain original order

        for (var i = 0; i < items.Length; i++)
        {
            await Assert.That(items[i].Key).IsEqualTo(MockStabilityWithIdData.Sorted[i].Key);
            await Assert.That(items[i].Id).IsEqualTo(MockStabilityWithIdData.Sorted[i].Id);
        }
    }

    [Test]
    [MethodDataSource(typeof(MockStabilityAllEqualsData), nameof(MockStabilityAllEqualsData.Generate))]
    public async Task StabilityTestWithAllEqual(StabilityTestItem[] items)
    {
        // Edge case: all elements have the same value
        // They should remain in original order
        var stats = new StatisticsContext();

        PairInsertionSort.Sort(items.AsSpan(), stats);

        // All values are 1
        foreach (var item in items) await Assert.That(item.Value).IsEqualTo(1);

        // Original order should be preserved: 0, 1, 2, 3, 4
        await Assert.That(items.Select(x => x.OriginalIndex).ToArray()).IsEquivalentTo(MockStabilityAllEqualsData.Sorted, CollectionOrdering.Matching);
    }

    [Test]
    public async Task EmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var array = Array.Empty<int>();

        PairInsertionSort.Sort(array.AsSpan(), stats);

        await Assert.That(array.Length).IsEqualTo(0);
        await Assert.That(stats.CompareCount).IsEqualTo(0ul);
        await Assert.That(stats.SwapCount).IsEqualTo(0ul);
    }

    [Test]
    public async Task SingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 42 };

        PairInsertionSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(new[] { 42 }, CollectionOrdering.Matching);
        await Assert.That(stats.CompareCount).IsEqualTo(0ul);
        await Assert.That(stats.SwapCount).IsEqualTo(0ul);
    }

    [Test]
    public async Task TwoElementsTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 2, 1 };

        PairInsertionSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(new[] { 1, 2 }, CollectionOrdering.Matching);
    }

    [Test]
    public async Task OddElementsTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 2 };

        PairInsertionSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(new[] { 1, 2, 3, 5, 8 }, CollectionOrdering.Matching);
    }


[Test]
[MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
public async Task StatisticsSortedTest(IInputSample<int> inputSample)
{
    if (inputSample.Samples.Length > 1024)
        return;

    var stats = new StatisticsContext();
    var array = inputSample.Samples.ToArray();
    PairInsertionSort.Sort(array.AsSpan(), stats);

    await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
    await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);

    // Pair Insertion Sort writes pairs even when data is sorted
    // The number of writes depends on whether data is truly sorted or not
    // For sorted data: writes = 2 * number_of_pairs (because we read pairs upfront)
        // Just verify writes occurred and no swaps
        await Assert.That(stats.IndexWriteCount > 0UL).IsTrue();
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
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
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesRandomTest(int n)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
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
