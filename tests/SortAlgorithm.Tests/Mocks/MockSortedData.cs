using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

public static class MockSortedData
{

    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Sorted,
            Samples = ArrayPatterns.GenerateSorted(100),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Sorted,
            Samples = ArrayPatterns.GenerateSorted(1000),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Sorted,
            Samples = ArrayPatterns.GenerateSorted(10000),
        };
    }
}
