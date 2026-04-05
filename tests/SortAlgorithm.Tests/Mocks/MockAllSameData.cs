namespace SortAlgorithm.Tests;

/// <summary>
/// 全要素が同じ値の配列。
/// buffer1 の抽出に失敗する（ユニーク値ゼロ）ため、in-place fallback が必要なアルゴリズムで重要。
/// <br/>
/// All elements are identical. Important for algorithms that need an in-place fallback
/// when buffer extraction fails due to zero unique values.
/// </summary>
public static class MockAllSameData
{
    public static IEnumerable<Func<InputSample<int>>> Generate()
    {
        yield return () => new InputSample<int>
        {
            InputType = InputType.AllIdentical,
            Samples = Enumerable.Repeat(42, 100).ToArray(),
        };
        yield return () => new InputSample<int>
        {
            InputType = InputType.AllIdentical,
            Samples = Enumerable.Repeat(42, 1000).ToArray(),
        };
        yield return () => new InputSample<int>
        {
            InputType = InputType.AllIdentical,
            Samples = Enumerable.Repeat(42, 10000).ToArray(),
        };
    }
}
