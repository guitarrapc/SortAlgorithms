using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

/// <summary>
/// Generates worst-case patterns specifically designed for middle-pivot QuickSort implementations.
/// These patterns create highly unbalanced partitions, leading to O(n²) behavior.
/// </summary>
/// <remarks>
/// Middle-pivot QuickSort (selecting pivot at (left + right) / 2) performs poorly on:
/// 1. The killer adversary, which is quadratic for QuickSort, QuickSortMedian3 and QuickSort3way
/// 2. Pipe organ patterns
/// 3. Zigzag patterns that create unbalanced partitions
///
/// <para>
/// The arrays are built once and handed out as copies. Every test mutates its sample in place, so
/// a fresh array per invocation is required - but building one is no longer free: the killer
/// adversary is derived by running QuickSort against it, so it costs the quadratic work it
/// provokes (~90 ms at 10,000 elements). Regenerating that once per test dominated the suite.
/// </para>
/// </remarks>
public static class MockQuickSortWorstCaseData
{
    // Pattern 1: Killer adversary - forces the middle-pivot family into O(n²)
    private static readonly int[] Adversary100 = ArrayPatterns.GenerateQuickSortAdversary(100);
    private static readonly int[] Adversary1000 = ArrayPatterns.GenerateQuickSortAdversary(1000);
    private static readonly int[] Adversary10000 = ArrayPatterns.GenerateQuickSortAdversary(10000);

    // Pattern 2: Pipe organ - creates poor middle pivot choices
    private static readonly int[] PipeOrgan100 = ArrayPatterns.GeneratePipeOrgan(100);
    private static readonly int[] PipeOrgan1000 = ArrayPatterns.GeneratePipeOrgan(1000);
    private static readonly int[] PipeOrgan10000 = ArrayPatterns.GeneratePipeOrgan(10000);

    // Pattern 3: Interleaved halves - splits poorly with middle pivot
    private static readonly int[] Interleaved100 = ArrayPatterns.GenerateEvensReversedOddsInOrder(100);
    private static readonly int[] Interleaved1000 = ArrayPatterns.GenerateEvensReversedOddsInOrder(1000);
    private static readonly int[] Interleaved10000 = ArrayPatterns.GenerateEvensReversedOddsInOrder(10000);

    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        int[][] patterns =
        [
            Adversary100, Adversary1000, Adversary10000,
            PipeOrgan100, PipeOrgan1000, PipeOrgan10000,
            Interleaved100, Interleaved1000, Interleaved10000,
        ];

        foreach (var pattern in patterns)
        {
            var source = pattern;
            yield return () => new InputSample<int>()
            {
                InputType = InputType.AntiQuickSort,
                Samples = (int[])source.Clone(),
            };
        }
    }
}
