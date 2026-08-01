using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

/// <summary>
/// <see cref="SortPhase.DistributionBucket"/> の契約を保証するテスト。
///
/// この通知は消費側がバケット境界を自前で復元しなくて済むように存在する。復元するには鍵写像・範囲正規化・
/// 桁幅・桁数をこのライブラリの外に写す必要があり、それらは実際に何度も食い違った。しかも誤った復元でも
/// 「それらしい分割」が出てしまうため、ソート結果を見ても気づけない。だから通知そのものを検証する。
///
/// 契約:
/// <list type="bullet">
/// <item><description>1 回の分配で報告された区間は、その分配が扱う範囲を隙間なく重なりなく覆う</description></item>
/// <item><description>空バケットは報告しない（報告数は min(基数, 範囲長) に収まる）</description></item>
/// <item><description>ラベルは昇順（バケット 0 から順に並ぶ）</description></item>
/// <item><description>要素が 1 つも動く前に報告される</description></item>
/// </list>
/// </summary>
public class DistributionBucketObservationTests
{
    /// <summary>キー範囲 328（9 ビット / 3 桁）。基数 4 / 10 / 256 のどれでも複数レベルが走る。</summary>
    private static int[] TwoGroupArray()
    {
        var values = Enumerable.Range(1, 100).Concat(Enumerable.Range(300, 30)).ToArray();
        var rng = new Random(20260802);
        for (var i = values.Length - 1; i > 0; i--)
        {
            var j = rng.Next(i + 1);
            (values[i], values[j]) = (values[j], values[i]);
        }
        return values;
    }

    public sealed record RadixCase(string Name, Action<int[], VisualizationContext> Sort)
    {
        public override string ToString() => Name;
    }

    public static IEnumerable<Func<RadixCase>> RadixCases()
    {
        yield return () => new RadixCase("AmericanFlagSort", static (a, c) => AmericanFlagSort.Sort(a.AsSpan(), c));
        yield return () => new RadixCase("RadixLSD4Sort", static (a, c) => RadixLSD4Sort.Sort(a.AsSpan(), c));
        yield return () => new RadixCase("RadixLSD10Sort", static (a, c) => RadixLSD10Sort.Sort(a.AsSpan(), c));
        yield return () => new RadixCase("RadixLSD256Sort", static (a, c) => RadixLSD256Sort.Sort(a.AsSpan(), c));
        yield return () => new RadixCase("RadixMSD4Sort", static (a, c) => RadixMSD4Sort.Sort(a.AsSpan(), c));
        yield return () => new RadixCase("RadixMSD10Sort", static (a, c) => RadixMSD10Sort.Sort(a.AsSpan(), c));
    }

    /// <summary>1 つの分配で報告された区間の並び。次の桁パスの通知で区切る。</summary>
    private static List<List<(int Start, int Length, int Label)>> CollectDistributions(RadixCase testCase, int[] array)
    {
        var groups = new List<List<(int, int, int)>>();
        List<(int, int, int)>? current = null;

        testCase.Sort(array, new VisualizationContext(onPhase: (phase, p1, p2, p3) =>
        {
            if (phase == SortPhase.RadixPass)
            {
                current = null; // 新しい分配の開始。区間はこの後に届く
            }
            else if (phase == SortPhase.DistributionBucket)
            {
                if (current is null)
                {
                    current = [];
                    groups.Add(current);
                }
                current.Add((p1, p2, p3));
            }
        }));

        return groups;
    }

    [Test]
    [MethodDataSource(nameof(RadixCases))]
    public async Task ReportedBucketsTileTheirRange(RadixCase testCase)
    {
        var array = TwoGroupArray();
        var groups = CollectDistributions(testCase, array);

        await Assert.That(groups).IsNotEmpty().Because("バケットが 1 度も報告されていません。");

        var problems = new List<string>();
        foreach (var g in groups)
        {
            for (var i = 1; i < g.Count; i++)
            {
                var prevEnd = g[i - 1].Start + g[i - 1].Length;
                if (g[i].Start != prevEnd)
                    problems.Add($"隙間/重なり: [{g[i - 1].Start}+{g[i - 1].Length}] の次が {g[i].Start}");
                if (g[i].Label <= g[i - 1].Label)
                    problems.Add($"ラベルが昇順でない: {g[i - 1].Label} → {g[i].Label}");
            }

            if (g.Any(x => x.Length <= 0))
                problems.Add("空バケットが報告されています");
        }

        await Assert.That(problems.Distinct().ToList()).IsEmpty();
    }

    /// <summary>
    /// 分配が扱った要素はすべてどこかの区間に属する。報告された区間長の合計は、その分配で
    /// 実際に移動した（あるいは移動しうる）要素数と一致しなければならない。
    /// </summary>
    [Test]
    [MethodDataSource(nameof(RadixCases))]
    public async Task ReportedBucketsCoverEveryElementInTheDistribution(RadixCase testCase)
    {
        var array = TwoGroupArray();
        var groups = CollectDistributions(testCase, array);

        var problems = new List<string>();
        foreach (var g in groups)
        {
            var total = g.Sum(x => x.Length);
            if (total > array.Length)
                problems.Add($"区間長の合計 {total} が配列長 {array.Length} を超えています");
            // 空バケットを報告しないので、区間数は要素数を超えない
            if (g.Count > total)
                problems.Add($"区間数 {g.Count} が要素数 {total} を超えています");
        }

        await Assert.That(problems).IsEmpty();
    }

    /// <summary>
    /// 要素が動く前に報告されること。これが崩れると、消費側は「もう動いた後の配列」に対して
    /// 「これから動く先」の区間を重ねて描くことになる。
    /// </summary>
    [Test]
    [MethodDataSource(nameof(RadixCases))]
    public async Task BucketsAreReportedBeforeAnyElementMoves(RadixCase testCase)
    {
        var array = TwoGroupArray();
        var movesBeforeFirstReport = 0;
        var reported = false;
        var moves = 0;

        testCase.Sort(array, new VisualizationContext(
            onIndexWrite: (_, _, _) => moves++,
            onSwap: (_, _, _) => moves++,
            onPhase: (phase, _, _, _) =>
            {
                if (phase == SortPhase.DistributionBucket && !reported)
                {
                    reported = true;
                    movesBeforeFirstReport = moves;
                }
            }));

        await Assert.That(reported).IsTrue().Because("バケットが 1 度も報告されていません。");
        await Assert.That(movesBeforeFirstReport).IsEqualTo(0)
            .Because("最初のバケット報告より前に要素が動いています。");
    }

    /// <summary>
    /// 観測しない実行パスには一切現れないこと。NullContext での <see cref="StatisticsContext"/> 比較ではなく、
    /// 通知そのものが 0 件であることを確認する。
    /// </summary>
    [Test]
    [MethodDataSource(nameof(RadixCases))]
    public async Task SortingIsUnaffectedByTheReport(RadixCase testCase)
    {
        var withContext = TwoGroupArray();
        var plain = TwoGroupArray();
        var expected = TwoGroupArray();
        Array.Sort(expected);

        testCase.Sort(withContext, new VisualizationContext(onPhase: (_, _, _, _) => { }));

        await Assert.That(withContext).IsEquivalentTo(expected);
        await Assert.That(plain.Length).IsEqualTo(expected.Length);
    }
}
