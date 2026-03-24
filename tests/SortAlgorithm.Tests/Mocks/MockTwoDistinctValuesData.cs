namespace SortAlgorithm.Tests;

/// <summary>
/// Test data with only two distinct values distributed randomly.
/// Based on BlockQuickSort paper benchmark: "random 0-1 values".
/// This tests partitioning behavior with binary data.
/// </summary>
public static class MockTwoDistinctValuesData
{

    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        var random = new Random(42);

        // Small array - random 0-1
        yield return () => new InputSample<int>()
        {
            InputType = InputType.TwoDistinctValues,
            Samples = Enumerable.Range(0, 100).Select(_ => random.Next(2)).ToArray()
        };

        // Medium array - random 0-1
        yield return () => new InputSample<int>()
        {
            InputType = InputType.TwoDistinctValues,
            Samples = Enumerable.Range(0, 500).Select(_ => random.Next(2)).ToArray()
        };

        // Large array - random 0-1
        yield return () => new InputSample<int>()
        {
            InputType = InputType.TwoDistinctValues,
            Samples = Enumerable.Range(0, 1000).Select(_ => random.Next(2)).ToArray()
        };

        // Very large array - random 0-1
        yield return () => new InputSample<int>()
        {
            InputType = InputType.TwoDistinctValues,
            Samples = Enumerable.Range(0, 5000).Select(_ => random.Next(2)).ToArray()
        };

        // Extra large array - random 0-1
        yield return () => new InputSample<int>()
        {
            InputType = InputType.TwoDistinctValues,
            Samples = Enumerable.Range(0, 10000).Select(_ => random.Next(2)).ToArray()
        };
    }

}
