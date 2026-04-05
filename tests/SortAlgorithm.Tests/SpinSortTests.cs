using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class SpinSortTests
{
    [Test]
    [MethodDataSource(typeof(MockRandomData), nameof(MockRandomData.Generate))]
    [MethodDataSource(typeof(MockNegativePositiveRandomData), nameof(MockNegativePositiveRandomData.Generate))]
    [MethodDataSource(typeof(MockNegativeRandomData), nameof(MockNegativeRandomData.Generate))]
    [MethodDataSource(typeof(MockReversedData), nameof(MockReversedData.Generate))]
    [MethodDataSource(typeof(MockReversedWithDuplicatesData), nameof(MockReversedWithDuplicatesData.Generate))]
    [MethodDataSource(typeof(MockPipeorganData), nameof(MockPipeorganData.Generate))]
    [MethodDataSource(typeof(MockNearlySortedData), nameof(MockNearlySortedData.Generate))]
    [MethodDataSource(typeof(MockAllSameData), nameof(MockAllSameData.Generate))]
    [MethodDataSource(typeof(MockSameValuesData), nameof(MockSameValuesData.Generate))]
    [MethodDataSource(typeof(MockQuickSortWorstCaseData), nameof(MockQuickSortWorstCaseData.Generate))]
    [MethodDataSource(typeof(MockTwoDistinctValuesData), nameof(MockTwoDistinctValuesData.Generate))]
    [MethodDataSource(typeof(MockHalfZeroHalfOneData), nameof(MockHalfZeroHalfOneData.Generate))]
    [MethodDataSource(typeof(MockValleyRandomData), nameof(MockValleyRandomData.Generate))]
    [MethodDataSource(typeof(MockHighlySkewedData), nameof(MockHighlySkewedData.Generate))]
    public async Task SortResultOrderTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        SpinSort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateHalf))]
    public async Task SortHalfResultOrderTest(IInputSample<Half> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        SpinSort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateFloat))]
    public async Task SortFloatResultOrderTest(IInputSample<float> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        SpinSort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateDouble))]
    public async Task SortDoubleResultOrderTest(IInputSample<double> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        SpinSort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockIntKeyRandomData), nameof(MockIntKeyRandomData.Generate))]
    public async Task SortIntStructResultOrderTest(IInputSample<Utils.IntKey> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        SpinSort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockStabilityData), nameof(MockStabilityData.Generate))]
    public async Task StabilityTest(StabilityTestItem[] items)
    {
        var stats = new StatisticsContext();

        SpinSort.Sort(items.AsSpan(), stats);

        await Assert.That(items.Select(x => x.Value).ToArray()).IsEquivalentTo(MockStabilityData.Sorted, CollectionOrdering.Matching);

        var value1Indices = items.Where(x => x.Value == 1).Select(x => x.OriginalIndex).ToArray();
        var value2Indices = items.Where(x => x.Value == 2).Select(x => x.OriginalIndex).ToArray();
        var value3Indices = items.Where(x => x.Value == 3).Select(x => x.OriginalIndex).ToArray();

        await Assert.That(value1Indices).IsEquivalentTo(MockStabilityData.Sorted1, CollectionOrdering.Matching);
        await Assert.That(value2Indices).IsEquivalentTo(MockStabilityData.Sorted2, CollectionOrdering.Matching);
        await Assert.That(value3Indices).IsEquivalentTo(MockStabilityData.Sorted3, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockStabilityWithIdData), nameof(MockStabilityWithIdData.Generate))]
    public async Task StabilityTestWithComplex(StabilityTestItemWithId[] items)
    {
        var stats = new StatisticsContext();

        SpinSort.Sort(items.AsSpan(), stats);

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
        var stats = new StatisticsContext();

        SpinSort.Sort(items.AsSpan(), stats);

        foreach (var item in items) await Assert.That(item.Value).IsEqualTo(1);

        await Assert.That(items.Select(x => x.OriginalIndex).ToArray()).IsEquivalentTo(MockStabilityAllEqualsData.Sorted, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        SpinSort.Sort(array.AsSpan(), stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(10)]
    [Arguments(36)]
    [Arguments(72)]
    [Arguments(73)]
    [Arguments(100)]
    [Arguments(200)]
    [Arguments(500)]
    [Arguments(1000)]
    [Arguments(2000)]
    public async Task SortSmallAndBoundaryTest(int n)
    {
        var stats = new StatisticsContext();
        var rng = new Random(42);
        var array = Enumerable.Range(0, n).Select(_ => rng.Next(0, n)).ToArray();
        var expected = array.ToArray();
        Array.Sort(expected);

        SpinSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }
}
