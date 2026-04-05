using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

/// <summary>
/// 重複値を含む降順配列（few-unique を逆順ソートしたもの）。
/// 安定ソートの merge パスで「逆順ローテーション」fast path と重複処理が同時に走るケースをテストする。
/// <br/>
/// Descending array containing duplicate values (few-unique sorted in reverse order).
/// Tests the "reverse order rotate" fast path combined with duplicate handling in stable merge passes.
/// </summary>
public static class MockReversedWithDuplicatesData
{
    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        var random = new Random(42);
        yield return () => new InputSample<int>
        {
            InputType = InputType.Reversed,
            Samples = ArrayPatterns.GenerateFewUnique(100, random).OrderByDescending(x => x).ToArray(),
        };
        yield return () => new InputSample<int>
        {
            InputType = InputType.Reversed,
            Samples = ArrayPatterns.GenerateFewUnique(1000, random).OrderByDescending(x => x).ToArray(),
        };
        yield return () => new InputSample<int>
        {
            InputType = InputType.Reversed,
            Samples = ArrayPatterns.GenerateFewUnique(10000, random).OrderByDescending(x => x).ToArray(),
        };
    }
}
