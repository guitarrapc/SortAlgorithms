using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

public static class MockNegativeRandomData
{

    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        var random = new Random(42);

        yield return () => new InputSample<int>()
        {
            InputType = InputType.NegativeRandom,
            Samples = ArrayPatterns.GenerateNegativeeRandom(100, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.NegativeRandom,
            Samples = ArrayPatterns.GenerateNegativeeRandom(1000, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.NegativeRandom,
            Samples = ArrayPatterns.GenerateNegativeeRandom(10000, random),
        };
    }
}
