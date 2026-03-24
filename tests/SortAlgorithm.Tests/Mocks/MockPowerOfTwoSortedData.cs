namespace SortAlgorithm.Tests;

public static class MockPowerOfTwoSortedData
{

    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Sorted,
            Samples = Enumerable.Range(0, 16).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Sorted,
            Samples = Enumerable.Range(0, 64).ToArray()
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Sorted,
            Samples = Enumerable.Range(0, 256).ToArray()
        };
    }
}
