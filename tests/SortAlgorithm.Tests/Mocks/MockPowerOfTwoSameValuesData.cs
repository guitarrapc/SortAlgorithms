namespace SortAlgorithm.Tests;

public static class MockPowerOfTwoSameValuesData
{

    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = Enumerable.Repeat(42, 16).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = Enumerable.Repeat(42, 64).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = Enumerable.Repeat(42, 256).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = Enumerable.Repeat(42, 512).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = Enumerable.Repeat(42, 1024).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = Enumerable.Repeat(42, 2048).ToArray()
        };
    }
}
