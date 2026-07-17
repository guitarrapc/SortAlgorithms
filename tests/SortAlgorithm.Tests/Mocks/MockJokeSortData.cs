using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

/// <summary>
/// Small deterministic inputs for factorial/exponential-time joke sorts
/// (BogoSort, SlowSort, StoogeSort). The standard mocks start at 100 elements,
/// which these algorithms cannot finish in reasonable time, so sizes here stay
/// at or below 10 to keep the mock-driven tests actually running.
/// </summary>
public static class MockJokeSortData
{
    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        var random = new Random(42);

        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = ArrayPatterns.GenerateRandom(2, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = ArrayPatterns.GenerateRandom(5, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Random,
            Samples = ArrayPatterns.GenerateRandom(8, random),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Reversed,
            Samples = ArrayPatterns.GenerateReversed(8),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Sorted,
            Samples = ArrayPatterns.GenerateSorted(8),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AllIdentical,
            Samples = ArrayPatterns.GenerateAllEqual(8),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.TwoDistinctValues,
            Samples = ArrayPatterns.GenerateFewUnique(8, 2, random),
        };
    }

    public static IEnumerable<Func<InputSample<int>>> GenerateSorted()
    {
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Sorted,
            Samples = ArrayPatterns.GenerateSorted(5),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Sorted,
            Samples = ArrayPatterns.GenerateSorted(10),
        };
    }
}
