using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

/// <summary>
/// Test data with valley-shaped pattern and many duplicates.
/// This creates approximately √n duplicates of each value on average.
/// </summary>
public static class MockValleyRandomData
{
    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        yield return () => new InputSample<int>()
        {
            InputType = InputType.ManyDuplicatesSqrtRange,
            Samples = ArrayPatterns.GenerateValleyShape(100),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.ManyDuplicatesSqrtRange,
            Samples = ArrayPatterns.GenerateValleyShape(1000),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.ManyDuplicatesSqrtRange,
            Samples = ArrayPatterns.GenerateValleyShape(1000),
        };
    }
}
