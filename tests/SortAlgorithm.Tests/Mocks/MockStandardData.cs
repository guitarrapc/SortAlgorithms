namespace SortAlgorithm.Tests;

/// <summary>
/// Aggregates the 15 standard int input patterns used by every SortResultOrderTest.
/// Use this single data source instead of stacking 15 MethodDataSource attributes.
/// </summary>
public static class MockStandardData
{
    public static IEnumerable<Func<InputSample<int>>> Generate() =>
        MockRandomData.Generate()
        .Concat(MockNegativePositiveRandomData.Generate())
        .Concat(MockNegativeRandomData.Generate())
        .Concat(MockReversedData.Generate())
        .Concat(MockReversedWithDuplicatesData.Generate())
        .Concat(MockPipeorganData.Generate())
        .Concat(MockNearlySortedData.Generate())
        .Concat(MockAllSameData.Generate())
        .Concat(MockSameValuesData.Generate())
        .Concat(MockQuickSortWorstCaseData.Generate())
        .Concat(MockTwoDistinctValuesData.Generate())
        .Concat(MockHalfZeroHalfOneData.Generate())
        .Concat(MockValleyRandomData.Generate())
        .Concat(MockHighlySkewedData.Generate())
        .Concat(MockHalfRotationData.Generate());
}
