using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

public static class MockNearlySortedData
{
    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        var random = new Random(42);
        yield return () => new InputSample<int>()
        {
            InputType = InputType.NearlySorted,
            Samples = ArrayPatterns.GenerateNearlySorted(100, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.NearlySorted,
            Samples = ArrayPatterns.GenerateNearlySorted(1000, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.NearlySorted,
            Samples = ArrayPatterns.GenerateNearlySorted(10000, random),
        };
    }
}
