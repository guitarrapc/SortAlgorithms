using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

public static class MockRandomData
{
    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        var random = new Random(42);

        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = ArrayPatterns.GenerateRandom(100, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = ArrayPatterns.GenerateRandom(1000, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = ArrayPatterns.GenerateRandom(10000, random),
        };
    }
}
