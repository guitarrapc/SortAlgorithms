using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

/// <summary>
/// IntKeyランダムデータ（JIT最適化の検証用）
/// </summary>
public static class MockIntKeyRandomData
{
    public static IEnumerable<Func<InputSample<IntKey>>> Generate()
    {
        var random = new Random(42);

        yield return () => new InputSample<IntKey>()
        {
            InputType = InputType.IntKeyRandom,
            Samples = ArrayPatterns.GenerateRandomIntKey(100, random),
        };
        yield return () => new InputSample<IntKey>()
        {
            InputType = InputType.IntKeyRandom,
            Samples = ArrayPatterns.GenerateRandomIntKey(1000, random),
        };
        yield return () => new InputSample<IntKey>()
        {
            InputType = InputType.IntKeyRandom,
            Samples = ArrayPatterns.GenerateRandomIntKey(10000, random),
        };
    }
}
