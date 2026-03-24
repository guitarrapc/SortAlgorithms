using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

public static class MockSameValuesData
{
    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        var random = new Random(42);

        yield return () => new InputSample<int>()
        {
            InputType = InputType.SameValues,
            Samples = ArrayPatterns.GenerateFewUnique(100, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.SameValues,
            Samples = ArrayPatterns.GenerateFewUnique(1000, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.SameValues,
            Samples = ArrayPatterns.GenerateFewUnique(10000, random),
        };
    }
}
