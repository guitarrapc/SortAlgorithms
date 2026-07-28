using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

/// <summary>
/// マージ系アルゴリズムがマージフェーズを observer に通知することを保証するテスト。
///
/// 通知が無いと、可視化側は葉の InsertionSort しか観測できず、直前の挿入フェーズのラベルが
/// マージ中も貼り付いたままになる（実際に SpinSort / SpinSortVariant /
/// RotateMergeSortRecursive / FlatStableSort でこの状態だった）。
/// 「ソート結果が正しい」だけではこの欠落を検出できないため、通知内容を直接検証する。
/// </summary>
public class MergePhaseNotificationTests
{
    /// <summary>決定的なシャッフル配列（値 0..n-1）。</summary>
    private static int[] MakeArray(int n)
    {
        var arr = Enumerable.Range(0, n).ToArray();
        var rng = new Random(42);
        for (var i = n - 1; i > 0; i--)
        {
            var j = rng.Next(i + 1);
            (arr[i], arr[j]) = (arr[j], arr[i]);
        }
        return arr;
    }

    private static List<(SortPhase Phase, int P1, int P2, int P3)> RecordPhases(int[] array, Action<int[], VisualizationContext> sort)
    {
        var phases = new List<(SortPhase, int, int, int)>();
        var context = new VisualizationContext(onPhase: (p, a, b, c) => phases.Add((p, a, b, c)));
        sort(array, context);
        return phases;
    }

    /// <summary>マージが必ず走るサイズと、その呼び出し方をまとめたケース。</summary>
    public sealed record MergeSortCase(string Name, Action<int[], VisualizationContext> Sort, int Size)
    {
        public override string ToString() => $"{Name}(n={Size})";
    }

    public static IEnumerable<Func<MergeSortCase>> MergeSortCases()
    {
        yield return () => new MergeSortCase("SpinSort", static (a, c) => SpinSort.Sort(a.AsSpan(), c), 80);
        yield return () => new MergeSortCase("SpinSortVariant", static (a, c) => SpinSortVariant.Sort(a.AsSpan(), c), 80);
        yield return () => new MergeSortCase("RotateMergeSortRecursive", static (a, c) => RotateMergeSortRecursive.Sort(a.AsSpan(), c), 48);
        yield return () => new MergeSortCase("FlatStableSort", static (a, c) => FlatStableSort.Sort(a.AsSpan(), c), 48);
        yield return () => new MergeSortCase("RotateMergeSort", static (a, c) => RotateMergeSort.Sort(a.AsSpan(), c), 48);
    }

    [Test]
    [MethodDataSource(nameof(MergeSortCases))]
    public async Task EmitsMergePhase(MergeSortCase testCase)
    {
        var array = MakeArray(testCase.Size);
        var expected = array.ToArray();
        Array.Sort(expected);

        var phases = RecordPhases(array, testCase.Sort);

        await Assert.That(array).IsEquivalentTo(expected)
            .Because($"{testCase.Name}: 通知を追加してもソート結果は変わらないこと");

        var mergePhases = phases
            .Where(p => p.Phase is SortPhase.MergeSortMerge or SortPhase.MergePass)
            .ToList();

        await Assert.That(mergePhases).IsNotEmpty()
            .Because($"{testCase.Name}: n={testCase.Size} でマージが走るのにマージフェーズが通知されていません。"
                   + $" 通知されたフェーズ: [{string.Join(", ", phases.Select(p => p.Phase).Distinct())}]");
    }

    /// <summary>
    /// MergeSortMerge の 3 引数は [p1..p2] と [p2+1..p3] の 2 区間を表す。
    /// 表示テキストがそのまま区間として読まれるため、順序が崩れていないことを検証する。
    /// </summary>
    [Test]
    [MethodDataSource(nameof(MergeSortCases))]
    public async Task MergeSortMergeBoundsAreOrdered(MergeSortCase testCase)
    {
        var phases = RecordPhases(MakeArray(testCase.Size), testCase.Sort);

        var malformed = phases
            .Where(p => p.Phase == SortPhase.MergeSortMerge)
            .Where(p => !(p.P1 <= p.P2 && p.P2 < p.P3))
            .Select(p => $"[{p.P1}..{p.P2}] + [{p.P2 + 1}..{p.P3}]")
            .ToList();

        await Assert.That(malformed).IsEmpty()
            .Because($"{testCase.Name}: MergeSortMerge は p1 <= p2 < p3 を満たす必要があります: {string.Join(", ", malformed)}");
    }
}
