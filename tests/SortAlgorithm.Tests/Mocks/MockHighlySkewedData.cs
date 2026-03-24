using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

/// <summary>
/// Test data with highly skewed distribution where most elements have the same value.
/// Approximately 90% of elements are the same value, rest are unique.
/// This tests behavior when pivot selection encounters many duplicates.
/// </summary>
public static class MockHighlySkewedData
{
    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        var random = new Random(42);

        yield return () => new InputSample<int>()
        {
            InputType = InputType.HighlySkewed,
            Samples = ArrayPatterns.GenerateSkewedDuplicates(100, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.HighlySkewed,
            Samples = ArrayPatterns.GenerateSkewedDuplicates(1000, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.HighlySkewed,
            Samples = ArrayPatterns.GenerateSkewedDuplicates(10000, random),
        };
    }
}
