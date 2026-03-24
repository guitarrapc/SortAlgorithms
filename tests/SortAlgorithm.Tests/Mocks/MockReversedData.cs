using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

public static class MockReversedData
{

    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Reversed,
            Samples = ArrayPatterns.GenerateReversed(100),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Reversed,
            Samples = ArrayPatterns.GenerateReversed(1000),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Reversed,
            Samples = ArrayPatterns.GenerateReversed(10000),
        };
    }
}
