namespace SortAlgorithm.Tests;

public static class MockPowerOfTwoReversedData
{

    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Reversed,
            Samples = Enumerable.Range(0, 16).Reverse().ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Reversed,
            Samples = Enumerable.Range(0, 64).Reverse().ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Reversed,
            Samples = Enumerable.Range(0, 256).Reverse().ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Reversed,
            Samples = Enumerable.Range(0, 512).Reverse().ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Reversed,
            Samples = Enumerable.Range(0, 1024).Reverse().ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Reversed,
            Samples = Enumerable.Range(0, 2048).Reverse().ToArray()
        };
    }
}
