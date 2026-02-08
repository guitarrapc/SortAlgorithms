using SortAlgorithm.Utils;

namespace SandboxBenchmark;

/// <summary>
/// Data patterns for benchmark testing.
/// </summary>
public enum DataPattern
{
    Random,
    Sorted,
    Reversed,
    AntiQuicksort,
}

public static class BenchmarkData
{
    public static int[] GenerateIntArray(int size, DataPattern pattern)
    {
        var random = new Random(42);
        return pattern switch
        {
            DataPattern.Random => ArrayPatterns.GenerateRandom(size, random),
            DataPattern.Sorted => ArrayPatterns.GenerateSorted(size),
            DataPattern.Reversed => ArrayPatterns.GenerateReversed(size),
            DataPattern.AntiQuicksort => ArrayPatterns.GenerateQuickSortAdversary(size),
            _ => throw new ArgumentException($"Unknown pattern: {pattern}")
        };
    }

    public static string[] GenerateStringArray(int size, DataPattern pattern)
    {
        var random = new Random(42);

        var baseArray = pattern switch
        {
            DataPattern.Random => ArrayPatterns.GenerateRandom(size, random).Select(i => $"String_{i:D6}").ToArray(),
            DataPattern.Sorted => ArrayPatterns.GenerateSorted(size).Select(i => $"String_{i:D6}").ToArray(),
            DataPattern.Reversed => ArrayPatterns.GenerateReversed(size).Select(i => $"String_{i:D6}").ToArray(),
            DataPattern.AntiQuicksort => ArrayPatterns.GenerateQuickSortAdversary(size).Select(i => $"String_{i:D6}").ToArray(),
            _ => throw new ArgumentException($"Unknown pattern: {pattern}")
        };
        return baseArray;
    }
}
