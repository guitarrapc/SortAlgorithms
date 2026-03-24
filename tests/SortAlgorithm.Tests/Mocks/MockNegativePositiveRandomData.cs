using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

public static class MockNegativePositiveRandomData
{
    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        var random = new Random(42);

        yield return () => new InputSample<int>()
        {
            InputType = InputType.MixRandom,
            Samples = ArrayPatterns.GenerateNegativePositiveRandom(100, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.MixRandom,
            Samples = ArrayPatterns.GenerateNegativePositiveRandom(1000, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.MixRandom,
            Samples = ArrayPatterns.GenerateNegativePositiveRandom(10000, random),
        };
    }
}
