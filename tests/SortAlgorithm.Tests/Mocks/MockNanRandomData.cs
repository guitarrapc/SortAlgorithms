using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

/// <summary>
/// NaN を含む浮動小数点数のテストデータを生成します。
/// IntroSort と PDQSort の NaN 最適化をテストするために使用します。
/// </summary>
public static class MockNanRandomData
{
    /// <summary>
    /// float 型の NaN を含むテストデータを生成します。
    /// </summary>
    public static IEnumerable<Func<InputSample<float>>> GenerateFloat()
    {
        var random = new Random(42);

        // Pattern 1: Random with 10% NaN (small)
        yield return () => new InputSample<float>()
        {
            InputType = InputType.RandomWithNaN,
            Samples = ArrayPatterns.GenerateRandomFloatWithNaN(100, random, 0.1),
        };

        // Pattern 2: Random with 10% NaN (medium)
        yield return () => new InputSample<float>()
        {
            InputType = InputType.RandomWithNaN,
            Samples = ArrayPatterns.GenerateRandomFloatWithNaN(1000, random, 0.1),
        };

        // Pattern 3: Random with 10% NaN (large)
        yield return () => new InputSample<float>()
        {
            InputType = InputType.RandomWithNaN,
            Samples = ArrayPatterns.GenerateRandomFloatWithNaN(10000, random, 0.1),
        };

        // Pattern 4: Random with 30% NaN (high NaN ratio)
        yield return () => new InputSample<float>()
        {
            InputType = InputType.RandomWithHighNaN,
            Samples = ArrayPatterns.GenerateRandomFloatWithNaN(1000, random, 0.3),
        };

        // Pattern 5: Sorted with NaN at beginning
        yield return () => new InputSample<float>()
        {
            InputType = InputType.SortedWithNaN,
            Samples = ArrayPatterns.GenerateSortedFloatWithNaN(1000, 10),
        };

        // Pattern 6: All NaN
        yield return () => new InputSample<float>()
        {
            InputType = InputType.AllNaN,
            Samples = ArrayPatterns.GenerateAllNaN(100),
        };

        // Pattern 7: No NaN (for optimization comparison)
        yield return () => new InputSample<float>()
        {
            InputType = InputType.RandomNoNaN,
            Samples = ArrayPatterns.GenerateRandomFloatNoNaN(1000, random),
        };
    }

    /// <summary>
    /// double 型の NaN を含むテストデータを生成します。
    /// </summary>
    public static IEnumerable<Func<InputSample<double>>> GenerateDouble()
    {
        var random = new Random(42);

        // Pattern 1: Random with 10% NaN (small)
        yield return () => new InputSample<double>()
        {
            InputType = InputType.RandomWithNaN,
            Samples = ArrayPatterns.GenerateRandomDoubleWithNaN(100, random, 0.1),
        };

        // Pattern 2: Random with 10% NaN (medium)
        yield return () => new InputSample<double>()
        {
            InputType = InputType.RandomWithNaN,
            Samples = ArrayPatterns.GenerateRandomDoubleWithNaN(1000, random, 0.1),
        };

        // Pattern 3: Random with 10% NaN (large)
        yield return () => new InputSample<double>()
        {
            InputType = InputType.RandomWithNaN,
            Samples = ArrayPatterns.GenerateRandomDoubleWithNaN(10000, random, 0.1),
        };
    }

    /// <summary>
    /// Half 型の NaN を含むテストデータを生成します。
    /// </summary>
    public static IEnumerable<Func<InputSample<Half>>> GenerateHalf()
    {
        var random = new Random(42);

        // Pattern 1: Random with 10% NaN (small)
        yield return () => new InputSample<Half>()
        {
            InputType = InputType.RandomWithNaN,
            Samples = ArrayPatterns.GenerateRandomHalfWithNaN(100, random, 0.1),
        };

        // Pattern 2: Random with 10% NaN (medium)
        yield return () => new InputSample<Half>()
        {
            InputType = InputType.RandomWithNaN,
            Samples = ArrayPatterns.GenerateRandomHalfWithNaN(1000, random, 0.1),
        };
    }
}
