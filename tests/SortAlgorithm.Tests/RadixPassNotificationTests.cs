using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

/// <summary>
/// 基数ソートが <see cref="SortPhase.RadixPass"/> の引数契約
/// （param1 = 現在の桁、param2 = 総桁数、<c>SortPhase.cs</c> 参照）を守ることを保証するテスト。
///
/// MSD 系 3 種は param2 に総桁数ではなく param1 と同じ値を渡していた。ソート結果は正しいため
/// 結果検証では検出できず、可視化側には「8 桁中 5 桁目」ではなく「5 桁中 5 桁目」と見える。
/// 進捗率が常に 100% になり、桁の進行が読めなくなるので、通知内容を直接検証する。
/// </summary>
public class RadixPassNotificationTests
{
    /// <summary>決定的なシャッフル配列。全桁に仕事が発生するよう int の全域から取る。</summary>
    private static int[] MakeArray(int n)
    {
        var rng = new Random(42);
        var arr = new int[n];
        for (var i = 0; i < n; i++) arr[i] = rng.Next(int.MinValue, int.MaxValue);
        return arr;
    }

    /// <param name="HasRangeScan">
    /// 桁パスに入る前にキー範囲（min/max）を 1 回走査するか。
    /// 走査するものは <see cref="SortPhase.KeyRangeScan"/> でそれを通知しなければならない。
    /// 現状は全実装が走査を持つ（桁数をキー幅ではなく max − min の幅から決めるため）が、
    /// 走査を持たない実装が加わったときに通知の有無を取り違えないよう区別を残している。
    /// </param>
    public sealed record RadixSortCase(string Name, Action<int[], VisualizationContext> Sort, bool HasRangeScan)
    {
        public override string ToString() => Name;
    }

    public static IEnumerable<Func<RadixSortCase>> RadixSortCases()
    {
        // MSD 系: 再帰の各ノードが RadixPass を通知する
        yield return () => new RadixSortCase("AmericanFlagSort", static (a, c) => AmericanFlagSort.Sort(a.AsSpan(), c), HasRangeScan: true);
        yield return () => new RadixSortCase("RadixMSD4Sort", static (a, c) => RadixMSD4Sort.Sort(a.AsSpan(), c), HasRangeScan: true);
        yield return () => new RadixSortCase("RadixMSD10Sort", static (a, c) => RadixMSD10Sort.Sort(a.AsSpan(), c), HasRangeScan: true);
        // LSD 系: 元から契約を満たしている。回帰の基準として同じ検証にかける
        yield return () => new RadixSortCase("RadixLSD4Sort", static (a, c) => RadixLSD4Sort.Sort(a.AsSpan(), c), HasRangeScan: true);
        yield return () => new RadixSortCase("RadixLSD10Sort", static (a, c) => RadixLSD10Sort.Sort(a.AsSpan(), c), HasRangeScan: true);
        yield return () => new RadixSortCase("RadixLSD256Sort", static (a, c) => RadixLSD256Sort.Sort(a.AsSpan(), c), HasRangeScan: true);
    }

    /// <summary>
    /// 事前のキー範囲走査は要素を 1 つも動かさないまま n 回 read するため、通知が無いと可視化側には
    /// 「直前のフェーズのラベルが貼られたまま n ステップ進む」状態に見える。
    /// 走査を持つ実装は最初の RadixPass より前に KeyRangeScan を 1 回だけ出すこと。
    /// </summary>
    [Test]
    [MethodDataSource(nameof(RadixSortCases))]
    public async Task RangeScanIsAnnouncedBeforeTheFirstDigitPass(RadixSortCase testCase)
    {
        var array = MakeArray(2000);
        var expected = array.ToArray();
        Array.Sort(expected);

        var phases = new List<SortPhase>();
        testCase.Sort(array, new VisualizationContext(onPhase: (phase, _, _, _) =>
        {
            if (phase is SortPhase.KeyRangeScan or SortPhase.RadixPass) phases.Add(phase);
        }));

        await Assert.That(array).IsEquivalentTo(expected);

        var scanCount = phases.Count(p => p == SortPhase.KeyRangeScan);
        if (!testCase.HasRangeScan)
        {
            await Assert.That(scanCount).IsEqualTo(0);
            return;
        }

        // 走査は 1 回だけ、かつ必ず最初の桁パスより前
        await Assert.That(scanCount).IsEqualTo(1);
        await Assert.That(phases[0]).IsEqualTo(SortPhase.KeyRangeScan);
    }

    [Test]
    [MethodDataSource(nameof(RadixSortCases))]
    public async Task RadixPassReportsTotalDigitsInParam2(RadixSortCase testCase)
    {
        // 桁レベルが 2 段以上走るサイズ。最も粗く分割するのは AmericanFlagSort（256 バケット・
        // カットオフ 64）で、1 段目のバケットが 64 を超えるには n > 256*64 = 16384 が要る。
        var array = MakeArray(50_000);
        var expected = array.ToArray();
        Array.Sort(expected);

        var passes = new List<(int Digit, int TotalDigits)>();
        var context = new VisualizationContext(onPhase: (phase, p1, p2, _) =>
        {
            if (phase == SortPhase.RadixPass) passes.Add((p1, p2));
        });

        testCase.Sort(array, context);

        await Assert.That(array).IsEquivalentTo(expected);
        await Assert.That(passes).IsNotEmpty();

        // 総桁数は 1 回のソート中で変化しない
        var totals = passes.Select(x => x.TotalDigits).Distinct().ToArray();
        await Assert.That(totals.Length).IsEqualTo(1);

        var totalDigits = totals[0];
        var digits = passes.Select(x => x.Digit).ToArray();

        // 桁位置は 0-based で総桁数未満。param2 に param1 を渡す回帰はここで落ちる
        await Assert.That(digits.Min()).IsGreaterThanOrEqualTo(0);
        await Assert.That(digits.Max()).IsLessThan(totalDigits);

        // LSD は 0 から、MSD は totalDigits-1 から始まるが、どちらも最上位桁は必ず 1 度通知される
        await Assert.That(digits.Max()).IsEqualTo(totalDigits - 1);

        // 複数桁を実際に処理している（param2 が param1 に追従しているだけではないことの担保）
        await Assert.That(digits.Distinct().Count()).IsGreaterThan(1);
    }
}
