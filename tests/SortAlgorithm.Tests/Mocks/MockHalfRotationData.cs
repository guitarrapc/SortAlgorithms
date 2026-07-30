using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

/// <summary>
/// Test data rotated by half: two ascending runs whose second run is entirely below the first
/// (<c>[n/2+1..n, 1..n/2]</c>).
///
/// Merge sorts split this into runs whose lengths are unequal in a way that drives split-point
/// helpers to a degenerate result (one side of the second sub-merge becomes empty). Glidesort
/// read one element past the run in exactly that case, which only surfaced as an exception in
/// DEBUG builds because <see cref="SortSpan{T, TComparer, TContext}"/> drops bounds checks in
/// RELEASE. Keep this pattern in the standard set so every algorithm is exercised by it.
/// </summary>
public static class MockHalfRotationData
{
    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        // 128 is the smallest size that reproduced the Glidesort out-of-bounds read.
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Mountain,
            Samples = ArrayPatterns.GenerateHalfRotation(128),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Mountain,
            Samples = ArrayPatterns.GenerateHalfRotation(1000),
        };
        // Odd size takes the cyclic-rotation branch of the generator.
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Mountain,
            Samples = ArrayPatterns.GenerateHalfRotation(1001),
        };
        yield return () => new InputSample<int>()
        {
            InputType = InputType.Mountain,
            Samples = ArrayPatterns.GenerateHalfRotation(10000),
        };
    }
}
