using SortAlgorithm.Utils;

namespace SortAlgorithm.Benchmark;

/// <summary>
/// Data patterns for benchmark testing.
/// </summary>
public enum DataPattern
{
    Random,
    SingleElementMoved,
    Sorted,
    Reversed,
    PipeOrgan,
    AntiQuicksort,
    /// <summary>
    /// 重複多数（40種類程度のユニーク値が n 個の要素に分散）。
    /// 等価要素の扱い（3-way partition、安定性、分布ソートのバケット長）で性質が変わるソートを区別します。
    /// <br/>
    /// Duplicate-heavy (about 40 distinct values spread over n elements).
    /// Separates sorts whose behavior depends on how equal elements are handled
    /// (3-way partitioning, stability, distribution-sort bucket lengths).
    /// </summary>
    ManyDuplicates,
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
            DataPattern.SingleElementMoved => ArrayPatterns.GenerateSingleElementMoved(size, random),
            DataPattern.PipeOrgan => ArrayPatterns.GeneratePipeOrgan(size),
            DataPattern.AntiQuicksort => ArrayPatterns.GenerateQuickSortAdversary(size),
            DataPattern.ManyDuplicates => ArrayPatterns.GenerateManyDuplicates(size, random),
            _ => throw new ArgumentException($"Unknown pattern: {pattern}")
        };
    }

    public static IntKey[] GenerateIntKeyArray(int size, DataPattern pattern)
    {
        var random = new Random(42);
        return pattern switch
        {
            DataPattern.Random => ArrayPatterns.GenerateRandomIntKey(size, random),
            DataPattern.Sorted => ArrayPatterns.GenerateSortedIntKey(size),
            DataPattern.Reversed => ArrayPatterns.GenerateReversedIntKey(size),
            DataPattern.SingleElementMoved => ArrayPatterns.GenerateSingleElementMovedIntKey(size, random),
            DataPattern.PipeOrgan => ArrayPatterns.GeneratePipeOrganIntKey(size),
            DataPattern.AntiQuicksort => ArrayPatterns.GenerateQuickSortAdversaryIntKey(size),
            DataPattern.ManyDuplicates => ArrayPatterns.GenerateManyDuplicatesIntKey(size, random),
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
            DataPattern.SingleElementMoved => ArrayPatterns.GenerateSingleElementMoved(size, random).Select(i => $"String_{i:D6}").ToArray(),
            DataPattern.PipeOrgan => ArrayPatterns.GeneratePipeOrgan(size).Select(i => $"String_{i:D6}").ToArray(),
            DataPattern.AntiQuicksort => ArrayPatterns.GenerateQuickSortAdversary(size).Select(i => $"String_{i:D6}").ToArray(),
            DataPattern.ManyDuplicates => ArrayPatterns.GenerateManyDuplicates(size, random).Select(i => $"String_{i:D6}").ToArray(),
            _ => throw new ArgumentException($"Unknown pattern: {pattern}")
        };
        return baseArray;
    }
}
