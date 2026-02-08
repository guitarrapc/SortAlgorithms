using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

public static class MockPowerOfTwoNegativePositiveRandomData
{

    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        var random = new Random(42);

        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = ArrayPatterns.GenerateNegativePositiveRandom(16, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = ArrayPatterns.GenerateNegativePositiveRandom(64, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = ArrayPatterns.GenerateNegativePositiveRandom(256, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = ArrayPatterns.GenerateNegativePositiveRandom(512, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = ArrayPatterns.GenerateNegativePositiveRandom(1024, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = ArrayPatterns.GenerateNegativePositiveRandom(2048, random),
        };
    }
}
