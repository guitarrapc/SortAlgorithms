using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

public static class MockPipeorganData
{

    public static IEnumerable<Func<InputSample<int>>> Generate()
    {

        yield return () => new InputSample<int>()
        {
            InputType = InputType.Mountain,
            Samples = ArrayPatterns.GeneratePipeOrgan(100),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Mountain,
            Samples = ArrayPatterns.GeneratePipeOrgan(1000),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Mountain,
            Samples = ArrayPatterns.GeneratePipeOrgan(10000),
        };
    }
}
