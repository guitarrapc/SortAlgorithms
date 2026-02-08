using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

public static class MockPowerOfTwoRandomData
{
    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        var random = new Random(42);

        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = ArrayPatterns.GenerateRandom(16, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = ArrayPatterns.GenerateRandom(64, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = ArrayPatterns.GenerateRandom(256, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = ArrayPatterns.GenerateRandom(512, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = ArrayPatterns.GenerateRandom(1024, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = ArrayPatterns.GenerateRandom(2048, random),
        };
    }
}
