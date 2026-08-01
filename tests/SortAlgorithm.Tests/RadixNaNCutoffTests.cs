using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

/// <summary>
/// 基数ソートは NaN をキー（0 = 非 NaN のどれより小さい）で先頭に置く。しかし挿入ソートのカットオフに
/// 落ちた範囲は桁ではなく <c>TComparer</c> で並べ替えられるため、両者の順序が食い違うと
/// 「カットオフに落ちたかどうか」で答えが変わる。
///
/// 実際に食い違っていた: 浮動小数点オーバーロードが <c>ComparableComparer</c> を渡しており、
/// <c>SortSpan</c> が NullContext 経路でそれを生の IEEE 演算子に特殊化するため NaN が順序不能になり、
/// 挿入ソートが NaN を動かせなかった。しかもこの経路でしか壊れないので、観測コンテキストを付けた
/// 呼び出しや Debug ビルドでは正しく出ていた。
///
/// <see cref="MockNanRandomData"/> の最小サンプルは n=100 で、MSD のカットオフ 48 も
/// AmericanFlagSort の 64 も超えているため、このクラスは既存テストの穴だった。
/// ここでは「NaN を含む範囲がカットオフに落ちる」真クラスを、
/// (a) 配列全体がカットオフ以下、(b) 深いバケットが NaN と非 NaN を抱えたままカットオフに落ちる、
/// の両方で固定する。
/// </summary>
public class RadixNaNCutoffTests
{
    public sealed record NaNSortCase(string Name, Action<double[]> Sort, Action<double[], ISortContext> SortWithContext)
    {
        public override string ToString() => Name;
    }

    /// <summary>挿入ソートのカットオフを持つ基数ソート。持たない LSD 系は構造上この穴に落ちない。</summary>
    public static IEnumerable<Func<NaNSortCase>> SortsWithCutoff()
    {
        yield return () => new NaNSortCase("RadixMSD4Sort",
            a => RadixMSD4Sort.Sort(a.AsSpan()),
            (a, c) => RadixMSD4Sort.Sort(a.AsSpan(), new StatisticsContext()));
        yield return () => new NaNSortCase("RadixMSD10Sort",
            a => RadixMSD10Sort.Sort(a.AsSpan()),
            (a, c) => RadixMSD10Sort.Sort(a.AsSpan(), new StatisticsContext()));
        yield return () => new NaNSortCase("AmericanFlagSort",
            a => AmericanFlagSort.Sort(a.AsSpan()),
            (a, c) => AmericanFlagSort.Sort(a.AsSpan(), new StatisticsContext()));
    }

    /// <summary>NaN を 1/3 混ぜた配列。値は決定的。</summary>
    private static double[] WithNaN(int n)
    {
        var random = new Random(42);
        var a = new double[n];
        for (var i = 0; i < n; i++) a[i] = i % 3 == 0 ? double.NaN : random.NextDouble() * 200.0 - 100.0;
        return a;
    }

    /// <summary>NaN 位置の一致と、非 NaN 要素の値の一致を見る（NaN == NaN は false なので位置で比較する）。</summary>
    private static void AssertSameAs(double[] expected, double[] actual, string because)
    {
        if (expected.Length != actual.Length) throw new Exception($"{because}: length differs");
        for (var i = 0; i < expected.Length; i++)
        {
            if (double.IsNaN(expected[i]) != double.IsNaN(actual[i]))
                throw new Exception($"{because}: index {i} NaN-ness differs (expected NaN={double.IsNaN(expected[i])})");
            if (!double.IsNaN(expected[i]) && expected[i] != actual[i])
                throw new Exception($"{because}: index {i} expected {expected[i]} but was {actual[i]}");
        }
    }

    /// <summary>
    /// 12/40 は配列全体がどのカットオフ（MSD 48 / AmericanFlag 64）より小さく挿入ソートに直行する。
    /// 60 は MSD のカットオフだけを跨ぎ、深いバケットが NaN を抱えたまま落ちる。65 は両方を跨ぐ。
    /// 200 と 4096 はカットオフの影響が出ない大きさ。
    /// </summary>
    private static readonly int[] SizesAroundEveryCutoff = [12, 40, 60, 65, 200, 4096];

    [Test]
    [MethodDataSource(nameof(SortsWithCutoff))]
    public async Task NaNSortsFirstWhateverTheCutoffDoes(NaNSortCase testCase)
    {
        foreach (var n in SizesAroundEveryCutoff)
        {
            var array = WithNaN(n);
            var expected = array.ToArray();
            Array.Sort(expected);

            testCase.Sort(array);

            AssertSameAs(expected, array, $"{testCase.Name} n={n}");
            await Assert.That(array.TakeWhile(double.IsNaN).Count()).IsEqualTo(expected.Count(double.IsNaN));
        }
    }

    /// <summary>
    /// 観測コンテキストの有無で答えが変わらないこと。<c>SortSpan</c> が比較子を生の IEEE 演算子に
    /// 特殊化するのは NullContext 経路だけなので、両者が割れると特殊化と桁パスの不一致を意味する。
    /// </summary>
    [Test]
    [MethodDataSource(nameof(SortsWithCutoff))]
    public async Task ObservationContextDoesNotChangeTheResult(NaNSortCase testCase)
    {
        foreach (var n in SizesAroundEveryCutoff)
        {
            var withoutContext = WithNaN(n);
            var withContext = withoutContext.ToArray();

            testCase.Sort(withoutContext);
            testCase.SortWithContext(withContext, new StatisticsContext());

            AssertSameAs(withoutContext, withContext, $"{testCase.Name} n={n}");
            await Assert.That(withContext.Length).IsEqualTo(withoutContext.Length);
        }
    }

    /// <summary>float と Half も同じ機構（<c>SortSpan</c> は 3 型とも特殊化する）なので同じ穴に落ちうる。</summary>
    [Test]
    [Arguments(12)]
    [Arguments(40)]
    [Arguments(60)]
    public async Task FloatAndHalfNaNAlsoSortFirstAtCutoffSizes(int n)
    {
        var random = new Random(42);

        var floats = new float[n];
        for (var i = 0; i < n; i++) floats[i] = i % 3 == 0 ? float.NaN : (float)(random.NextDouble() * 200.0 - 100.0);
        var expectedFloats = floats.ToArray();
        Array.Sort(expectedFloats);
        RadixMSD4Sort.Sort(floats.AsSpan());
        for (var i = 0; i < n; i++)
        {
            await Assert.That(float.IsNaN(floats[i])).IsEqualTo(float.IsNaN(expectedFloats[i]));
            if (!float.IsNaN(expectedFloats[i])) await Assert.That(floats[i]).IsEqualTo(expectedFloats[i]);
        }

        var halves = new Half[n];
        for (var i = 0; i < n; i++) halves[i] = i % 3 == 0 ? Half.NaN : (Half)(random.NextDouble() * 200.0 - 100.0);
        var expectedHalves = halves.ToArray();
        Array.Sort(expectedHalves);
        RadixMSD10Sort.Sort(halves.AsSpan());
        for (var i = 0; i < n; i++)
        {
            await Assert.That(Half.IsNaN(halves[i])).IsEqualTo(Half.IsNaN(expectedHalves[i]));
            if (!Half.IsNaN(expectedHalves[i])) await Assert.That(halves[i]).IsEqualTo(expectedHalves[i]);
        }
    }

    /// <summary>
    /// NaN だけの配列がカットオフ以下でも壊れないこと（全要素が tie なので入力順のまま返る）。
    /// </summary>
    [Test]
    [MethodDataSource(nameof(SortsWithCutoff))]
    public async Task AllNaNBelowCutoffIsLeftAlone(NaNSortCase testCase)
    {
        foreach (var n in SizesAroundEveryCutoff)
        {
            var array = Enumerable.Repeat(double.NaN, n).ToArray();

            testCase.Sort(array);

            await Assert.That(array.All(double.IsNaN)).IsTrue();
            await Assert.That(array.Length).IsEqualTo(n);
        }
    }
}
