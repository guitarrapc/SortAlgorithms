using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

/// <summary>
/// Generates worst-case patterns specifically designed for middle-pivot QuickSort implementations.
/// These patterns create highly unbalanced partitions, leading to O(n²) behavior.
/// </summary>
/// <remarks>
/// Middle-pivot QuickSort (selecting pivot at (left + right) / 2) performs poorly on:
/// 1. Alternating patterns (sawtooth)
/// 2. Sorted arrays with repeated median values
/// 3. Zigzag patterns that create unbalanced partitions
/// </remarks>
public static class MockQuickSortWorstCaseData
{
    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        // Pattern 1: Sawtooth - alternating low/high values
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AntiQuickSort,
            Samples = ArrayPatterns.GenerateQuickSortAdversary(100),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AntiQuickSort,
            Samples = ArrayPatterns.GenerateQuickSortAdversary(1000),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AntiQuickSort,
            Samples = ArrayPatterns.GenerateQuickSortAdversary(10000),
        };

        // Pattern 2: Pipe organ - creates poor middle pivot choices
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AntiQuickSort,
            Samples = ArrayPatterns.GeneratePipeOrgan(100),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AntiQuickSort,
            Samples = ArrayPatterns.GeneratePipeOrgan(1000),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AntiQuickSort,
            Samples = ArrayPatterns.GeneratePipeOrgan(10000),
        };

        // Pattern 3: Interleaved halves - splits poorly with middle pivot
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AntiQuickSort,
            Samples = ArrayPatterns.GenerateEvensReversedOddsInOrder(100),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AntiQuickSort,
            Samples = ArrayPatterns.GenerateEvensReversedOddsInOrder(1000),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.AntiQuickSort,
            Samples = ArrayPatterns.GenerateEvensReversedOddsInOrder(10000),
        };
    }
}
