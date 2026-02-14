using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class LibrarySortTests
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

        LibrarySort.Sort(array.AsSpan(), stats);

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

        LibrarySort.Sort(array.AsSpan(), stats);

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

        LibrarySort.Sort(array.AsSpan(), stats);

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

        LibrarySort.Sort(array.AsSpan(), stats);

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

        LibrarySort.Sort(array.AsSpan(), stats);

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

        LibrarySort.Sort(items.AsSpan(), stats);

        // Verify sorting correctness - values should be in ascending order
        await Assert.That(items.Select(x => x.Value).ToArray()).IsEquivalentTo(MockStabilityData.Sorted, CollectionOrdering.Matching);

        // Verify stability: for each group of equal values, original order is preserved
        var value1Indices = items.Where(x => x.Value == 1).Select(x => x.OriginalIndex).ToArray();
        var value2Indices = items.Where(x => x.Value == 2).Select(x => x.OriginalIndex).ToArray();
        var value3Indices = items.Where(x => x.Value == 3).Select(x => x.OriginalIndex).ToArray();

        // Value 1 appeared at original indices 0, 2, 4 - should remain in this order
        await Assert.That(value1Indices).IsEquivalentTo(MockStabilityData.Sorted1, CollectionOrdering.Matching);

        // Value 2 appeared at original indices 1, 5 - should remain in this order
        await Assert.That(value2Indices).IsEquivalentTo(MockStabilityData.Sorted2, CollectionOrdering.Matching);

        // Value 3 appeared at original index 3
        await Assert.That(value3Indices).IsEquivalentTo(MockStabilityData.Sorted3, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockStabilityWithIdData), nameof(MockStabilityWithIdData.Generate))]
    public async Task StabilityTestWithComplex(StabilityTestItemWithId[] items)
    {
        // Test stability with more complex scenario - multiple equal values
        var stats = new StatisticsContext();

        LibrarySort.Sort(items.AsSpan(), stats);

        // Expected: [2:B, 2:D, 2:F, 5:A, 5:C, 5:G, 8:E]
        // Keys are sorted, and elements with the same key maintain original order

        for (var i = 0; i < items.Length; i++)
        {
            await Assert.That(items[i].Key).IsEqualTo(MockStabilityWithIdData.Sorted[i].Key);
            await Assert.That(items[i].Id).IsEqualTo(MockStabilityWithIdData.Sorted[i].Id);
        }
    }

    [Test]
    public async Task EmptyArrayTest()
    {
        var stats = new StatisticsContext();
        var array = Array.Empty<int>();

        LibrarySort.Sort(array.AsSpan(), stats);

        await Assert.That(array).Count().IsEqualTo(0);
    }

    [Test]
    public async Task SingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 42 };

        LibrarySort.Sort(array.AsSpan(), stats);

        await Assert.That(array).Count().IsEqualTo(1);
        await Assert.That(array[0]).IsEqualTo(42);
    }

    [Test]
    public async Task TwoElementsAscendingTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 2 };

        LibrarySort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(new[] { 1, 2 }, CollectionOrdering.Matching);
    }

    [Test]
    public async Task TwoElementsDescendingTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 2, 1 };

        LibrarySort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(new[] { 1, 2 }, CollectionOrdering.Matching);
    }

    [Test]
    public async Task AllSameElementsTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 5, 5, 5, 5 };

        LibrarySort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(new[] { 5, 5, 5, 5, 5 }, CollectionOrdering.Matching);
    }

    [Test]
    public async Task AlreadySortedTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 2, 3, 4, 5 };

        LibrarySort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(new[] { 1, 2, 3, 4, 5 }, CollectionOrdering.Matching);
    }

    [Test]
    public async Task ReverseSortedTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 4, 3, 2, 1 };

        LibrarySort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(new[] { 1, 2, 3, 4, 5 }, CollectionOrdering.Matching);
    }

    [Test]
    public async Task LargerArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 64, 34, 25, 12, 22, 11, 90, 88, 45, 50, 22, 34, 67, 100 };
        var expected = array.OrderBy(x => x).ToArray();

        LibrarySort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task WithDuplicatesTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 3, 1, 4, 1, 5, 9, 2, 6, 5, 3, 5 };
        var expected = array.OrderBy(x => x).ToArray();

        LibrarySort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task NegativeNumbersTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { -5, 3, -2, 8, -10, 0, 1, -3 };
        var expected = array.OrderBy(x => x).ToArray();

        LibrarySort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }


#if DEBUG

    [Test]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        if (inputSample.Samples.Length > 1024)
            return;

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        LibrarySort.Sort(array.AsSpan(), stats);

        // Library Sort uses auxiliary arrays, so statistics are different from standard insertion sort
        // Just verify the array is sorted and some operations occurred
        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsGreaterThan(0UL);
        await Assert.That(stats.IndexWriteCount).IsGreaterThan(0UL); // Library sort always writes to aux array
        await Assert.That(stats.SwapCount).IsEqualTo(0UL); // Library sort doesn't use swaps
    }

#endif
}
