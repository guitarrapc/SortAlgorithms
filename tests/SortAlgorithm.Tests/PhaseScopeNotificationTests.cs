using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

/// <summary>
/// ハイブリッドアルゴリズムが宣言したスコープ相が、委譲先コアのディテール相に潰されないことを保証するテスト。
///
/// 共有コア（InsertionSort / BinaryInsertionSort / HeapSort の SortCore）は呼び出し元が
/// 「この範囲を挿入ソートに渡す」と宣言した直後に自分の進捗相を発行する。可視化側が現在相を
/// 1 枠しか持たないと、この宣言は 1 オペレーションも表示されないまま消える
/// （実際に SymMergeSort の MergeInitSort と introsort 系の Hybrid* がこの状態だった）。
///
/// ソート結果は正しいままなので結果ベースのテストでは検出できない。
/// <see cref="SortPhaseExtensions.IsDetailPhase"/> による 2 枠の意味論をここで直接検証する。
/// </summary>
public class PhaseScopeNotificationTests
{
    /// <summary>記録したイベント。Phase 以外は「観測可能な 1 オペレーション」として数える。</summary>
    private readonly record struct Event(SortPhase Phase, bool IsOperation);

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

    private static List<Event> RecordEvents(int[] array, Action<int[], VisualizationContext> sort)
    {
        var events = new List<Event>();
        void Op() => events.Add(new Event(SortPhase.None, IsOperation: true));

        var context = new VisualizationContext(
            onCompare: (_, _, _, _, _) => Op(),
            onSwap: (_, _, _) => Op(),
            onIndexRead: (_, _) => Op(),
            onIndexWrite: (_, _, _) => Op(),
            onRangeCopy: (_, _, _, _, _, _) => Op(),
            onPhase: (p, _, _, _) => events.Add(new Event(p, IsOperation: false)));

        sort(array, context);
        return events;
    }

    /// <summary>ハイブリッド経路が必ず走るサイズと、その呼び出し方をまとめたケース。</summary>
    public sealed record PhaseCase(string Name, Action<int[], VisualizationContext> Sort, int Size)
    {
        public override string ToString() => $"{Name}(n={Size})";
    }

    public static IEnumerable<Func<PhaseCase>> PhaseCases()
    {
        // 委譲先が InsertionSort.SortCore のもの
        yield return () => new PhaseCase("IntroSort", static (a, c) => IntroSort.Sort(a.AsSpan(), c), 200);
        yield return () => new PhaseCase("StdSort", static (a, c) => StdSort.Sort(a.AsSpan(), c), 200);
        yield return () => new PhaseCase("PDQSort", static (a, c) => PDQSort.Sort(a.AsSpan(), c), 200);
        yield return () => new PhaseCase("PDQSortBranchless", static (a, c) => PDQSortBranchless.Sort(a.AsSpan(), c), 200);
        yield return () => new PhaseCase("BlockQuickSort", static (a, c) => BlockQuickSort.Sort(a.AsSpan(), c), 200);
        yield return () => new PhaseCase("DualPivotQuickSort", static (a, c) => DualPivotQuickSort.Sort(a.AsSpan(), c), 200);
        yield return () => new PhaseCase("QuickSort3way", static (a, c) => QuickSort3way.Sort(a.AsSpan(), c), 200);
        yield return () => new PhaseCase("SymMergeSort", static (a, c) => SymMergeSort.Sort(a.AsSpan(), c), 200);
        yield return () => new PhaseCase("RotateMergeSort", static (a, c) => RotateMergeSort.Sort(a.AsSpan(), c), 200);
        yield return () => new PhaseCase("SpinSort", static (a, c) => SpinSort.Sort(a.AsSpan(), c), 200);
        yield return () => new PhaseCase("StdStableSort", static (a, c) => StdStableSort.Sort(a.AsSpan(), c), 200);
        yield return () => new PhaseCase("LibrarySort", static (a, c) => LibrarySort.Sort(a.AsSpan(), c), 200);
        yield return () => new PhaseCase("PowerSort", static (a, c) => PowerSort.Sort(a.AsSpan(), c), 200);
        // 委譲先が BinaryInsertionSort.SortCore のもの
        yield return () => new PhaseCase("TimSort", static (a, c) => TimSort.Sort(a.AsSpan(), c), 200);
    }

    /// <summary>
    /// 2 枠（スコープ / ディテール）で再生したとき、宣言されたスコープ相はいずれも
    /// 最低 1 オペレーションのあいだ現在相であり続けなければならない。
    ///
    /// 新しい共有コアが <see cref="SortPhaseExtensions.IsDetailPhase"/> に登録されないまま
    /// 相を発行しはじめると、それはスコープ相として扱われて呼び出し元を上書きするため、
    /// ここで 0 オペレーションのスコープ相として検出される。
    /// </summary>
    [Test]
    [MethodDataSource(nameof(PhaseCases))]
    public async Task ScopePhaseSurvivesDelegatedCore(PhaseCase testCase)
    {
        var array = MakeArray(testCase.Size);
        var expected = array.ToArray();
        Array.Sort(expected);

        var events = RecordEvents(array, testCase.Sort);

        await Assert.That(array).IsEquivalentTo(expected)
            .Because($"{testCase.Name}: 相の観測を変えてもソート結果は変わらないこと");

        // スコープ枠だけを追跡する。ディテール相はスコープ枠に触れない = 潰さない。
        var currentScope = SortPhase.None;
        var opsUnderCurrentScope = 0;
        var starved = new List<SortPhase>();

        foreach (var e in events)
        {
            if (e.IsOperation)
            {
                opsUnderCurrentScope++;
                continue;
            }

            if (!e.Phase.IsScopePhase()) continue;

            if (currentScope != SortPhase.None && opsUnderCurrentScope == 0)
                starved.Add(currentScope);

            currentScope = e.Phase;
            opsUnderCurrentScope = 0;
        }

        await Assert.That(starved.Distinct().ToList()).IsEmpty()
            .Because($"""
                {testCase.Name}: 宣言されたのに 1 オペレーションも表示されないスコープ相があります。
                委譲先コアの相が IsDetailPhase に登録されていない可能性があります:
                {string.Join(", ", starved.Distinct())}
                """);
    }

    /// <summary>
    /// 委譲が起きるアルゴリズムでは、スコープ相とディテール相が同時に成立する瞬間が存在する。
    /// これが 1 度も起きないなら、2 枠に分けた意味が無い（= 委譲先が相を出していないか、
    /// 呼び出し元がスコープを宣言していない）。
    /// </summary>
    [Test]
    [MethodDataSource(nameof(PhaseCases))]
    public async Task DelegatedCoreReportsDetailUnderScope(PhaseCase testCase)
    {
        var events = RecordEvents(MakeArray(testCase.Size), testCase.Sort);

        var currentScope = SortPhase.None;
        var pairs = new List<(SortPhase Scope, SortPhase Detail)>();

        foreach (var e in events)
        {
            if (e.IsOperation) continue;
            if (e.Phase.IsScopePhase()) currentScope = e.Phase;
            else if (e.Phase.IsDetailPhase() && currentScope != SortPhase.None)
                pairs.Add((currentScope, e.Phase));
        }

        await Assert.That(pairs).IsNotEmpty()
            .Because($"""
                {testCase.Name}: スコープ相の下でディテール相が発行される瞬間がありません。
                観測された相: [{string.Join(", ", events.Where(e => !e.IsOperation).Select(e => e.Phase).Distinct())}]
                """);
    }
}
