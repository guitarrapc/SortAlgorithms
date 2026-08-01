using SortAlgorithm.Algorithms;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

/// <summary>
/// <c>-0.0</c> と <c>+0.0</c> は IEEE 754 比較でも <see cref="IComparable{T}"/> でも等しい。
/// つまり tie であり、安定ソートは入力順を保たなければならない。
///
/// 基数ソートは要素ではなくキーで順序を決めるため、キーが両者を分離していると
/// 「自分が等しいと宣言した要素を並べ替える安定ソート」になる。さらに MSD 系と
/// American Flag は挿入ソートのカットオフを持つので、分離していると同じ多重集合に対する
/// 答えがカットオフを跨いだ瞬間に変わる（回帰前の実測: n=8 は入力順、n=64 は -0 が先頭に集まる）。
///
/// ここではキー側の同一性と、カットオフの両側での挙動の両方を固定する。
/// </summary>
public class NegativeZeroTieTests
{
    // 8 はライブラリ内のどのカットオフ（MSD 48 / AmericanFlag 64）より小さく、128 はどれより大きい。
    // 2048 は SpreadSort の min_sort_size = 1000 を超え、分配経路に実際に入る唯一のサイズ。
    private static readonly int[] SizesAcrossEveryCutoff = [8, 128, 2048];

    public sealed record DoubleSortCase(string Name, Action<double[]> Sort)
    {
        public override string ToString() => Name;
    }

    /// <summary>キーで順序を決め、かつ安定と宣言しているソート。tie は入力順で出なければならない。</summary>
    public static IEnumerable<Func<DoubleSortCase>> StableKeyOrderedSorts()
    {
        yield return () => new DoubleSortCase("RadixLSD4Sort", a => RadixLSD4Sort.Sort(a.AsSpan()));
        yield return () => new DoubleSortCase("RadixLSD10Sort", a => RadixLSD10Sort.Sort(a.AsSpan()));
        yield return () => new DoubleSortCase("RadixLSD256Sort", a => RadixLSD256Sort.Sort(a.AsSpan()));
        yield return () => new DoubleSortCase("RadixMSD4Sort", a => RadixMSD4Sort.Sort(a.AsSpan()));
        yield return () => new DoubleSortCase("RadixMSD10Sort", a => RadixMSD10Sort.Sort(a.AsSpan()));
    }

    /// <summary>キーで順序を決めるが不安定なソート。tie 順は未規定だが、値を書き換えてはならない。</summary>
    public static IEnumerable<Func<DoubleSortCase>> UnstableKeyOrderedSorts()
    {
        yield return () => new DoubleSortCase("AmericanFlagSort", a => AmericanFlagSort.Sort(a.AsSpan()));
        yield return () => new DoubleSortCase("SpreadSort", a => SpreadSort.Sort(a.AsSpan()));
    }

    /// <summary>+0.0 と -0.0 だけを交互に並べた配列。安定ソートはこれをそのまま返さなければならない。</summary>
    private static double[] AlternatingZeros(int n)
    {
        var a = new double[n];
        for (var i = 0; i < n; i++) a[i] = i % 2 == 0 ? +0.0 : -0.0;
        return a;
    }

    private static string SignPattern(double[] a) => string.Concat(a.Select(x => double.IsNegative(x) ? '-' : '+'));

    [Test]
    [MethodDataSource(nameof(StableKeyOrderedSorts))]
    public async Task StableSortKeepsInputOrderOfTiedZeros(DoubleSortCase testCase)
    {
        foreach (var n in SizesAcrossEveryCutoff)
        {
            var array = AlternatingZeros(n);
            var expected = SignPattern(array);

            testCase.Sort(array);

            // 全要素が tie なので、安定ソートの出力は入力そのもの
            await Assert.That(SignPattern(array)).IsEqualTo(expected);
        }
    }

    /// <summary>
    /// カットオフを跨いでも答えが変わらないこと。回帰前はここが n=8 と n=128 で食い違っていた。
    /// </summary>
    [Test]
    [MethodDataSource(nameof(StableKeyOrderedSorts))]
    public async Task StableSortAnswerDoesNotDependOnSize(DoubleSortCase testCase)
    {
        var patterns = new List<string>();
        foreach (var n in SizesAcrossEveryCutoff)
        {
            var array = AlternatingZeros(n);
            testCase.Sort(array);
            // 先頭 8 要素だけ見れば、-0 が前方に集められたか入力順のままかは判別できる
            patterns.Add(SignPattern(array)[..8]);
        }

        await Assert.That(patterns.Distinct().Count()).IsEqualTo(1);
    }

    [Test]
    [MethodDataSource(nameof(UnstableKeyOrderedSorts))]
    public async Task UnstableSortPreservesZeroSignsEvenThoughTieOrderIsUnspecified(DoubleSortCase testCase)
    {
        foreach (var n in SizesAcrossEveryCutoff)
        {
            var array = AlternatingZeros(n);
            var negativeZeros = array.Count(double.IsNegative);

            testCase.Sort(array);

            // 並び順は問わないが、-0.0 が +0.0 に書き換わっていてはならない
            await Assert.That(array.Count(double.IsNegative)).IsEqualTo(negativeZeros);
            await Assert.That(array.All(x => x == 0.0)).IsTrue();
        }
    }

    /// <summary>
    /// 実データに埋めた tie。ゼロ群の前後が正しく並び、ゼロ群自身の入力順も保たれること。
    /// </summary>
    [Test]
    [MethodDataSource(nameof(StableKeyOrderedSorts))]
    public async Task TiedZerosSurroundedByRealValuesLandInOneRun(DoubleSortCase testCase)
    {
        const int n = 512;
        var array = new double[n];
        for (var i = 0; i < n; i++)
        {
            array[i] = (i % 4) switch
            {
                0 => -1.5,
                1 => +0.0,
                2 => -0.0,
                _ => 2.5,
            };
        }

        testCase.Sort(array);

        var quarter = n / 4;
        await Assert.That(array.Take(quarter).All(x => x == -1.5)).IsTrue();
        await Assert.That(array.Skip(3 * quarter).All(x => x == 2.5)).IsTrue();

        // ゼロ群は連続した 1 つのランになり、その中は入力順（+0, -0, +0, -0, ...）
        var zeros = array.Skip(quarter).Take(2 * quarter).ToArray();
        await Assert.That(zeros.All(x => x == 0.0)).IsTrue();
        await Assert.That(SignPattern(zeros)).IsEqualTo(string.Concat(Enumerable.Repeat("+-", quarter)));
    }

    [Test]
    public async Task FloatAndHalfTiesBehaveTheSameAsDouble()
    {
        const int n = 128;

        var floats = new float[n];
        for (var i = 0; i < n; i++) floats[i] = i % 2 == 0 ? +0.0f : -0.0f;
        RadixMSD4Sort.Sort(floats.AsSpan());
        await Assert.That(string.Concat(floats.Select(x => float.IsNegative(x) ? '-' : '+')))
            .IsEqualTo(string.Concat(Enumerable.Repeat("+-", n / 2)));

        var positiveZero = BitConverter.UInt16BitsToHalf(0x0000);
        var negativeZero = BitConverter.UInt16BitsToHalf(0x8000);
        var halves = new Half[n];
        for (var i = 0; i < n; i++) halves[i] = i % 2 == 0 ? positiveZero : negativeZero;
        RadixMSD4Sort.Sort(halves.AsSpan());
        await Assert.That(string.Concat(halves.Select(x => Half.IsNegative(x) ? '-' : '+')))
            .IsEqualTo(string.Concat(Enumerable.Repeat("+-", n / 2)));
    }

    /// <summary>
    /// キー側の契約: 等しい要素は等しいキーに写らなければならない
    /// (<see cref="IRadixKeySelector{T}"/> の Monotonicity 節)。tie を分離しないことと、
    /// 他の値の順序が壊れていないことを同時に見る。
    /// </summary>
    [Test]
    public async Task EqualElementsMapToEqualKeys()
    {
        await Assert.That(default(DoubleRadixKey).GetKey(-0.0)).IsEqualTo(default(DoubleRadixKey).GetKey(+0.0));
        await Assert.That(default(SingleRadixKey).GetKey(-0.0f)).IsEqualTo(default(SingleRadixKey).GetKey(+0.0f));
        await Assert.That(default(HalfRadixKey).GetKey(BitConverter.UInt16BitsToHalf(0x8000)))
            .IsEqualTo(default(HalfRadixKey).GetKey(BitConverter.UInt16BitsToHalf(0x0000)));

        // NaN は依然としてキー 0 で単独最小、それ以外は狭義単調
        var key = default(DoubleRadixKey);
        await Assert.That(key.GetKey(double.NaN)).IsEqualTo(0UL);
        double[] ascending = [double.NegativeInfinity, -1.5, -double.Epsilon, -0.0, double.Epsilon, 1.5, double.PositiveInfinity];
        for (var i = 1; i < ascending.Length; i++)
        {
            var previous = key.GetKey(ascending[i - 1]);
            var current = key.GetKey(ascending[i]);
            await Assert.That(previous).IsLessThan(current);
            // NaN のキー 0 は非 NaN のどれとも衝突しない
            await Assert.That(current).IsNotEqualTo(0UL);
        }
    }

    /// <summary>
    /// 順序自体の回帰。ゼロを含む配列が <c>Array.Sort</c> と同じ値列になること
    /// （<c>Array.Sort</c> も -0.0/+0.0 を等しいと見なすので、値の列としては一致する）。
    /// </summary>
    [Test]
    [MethodDataSource(nameof(StableKeyOrderedSorts))]
    public async Task ValueSequenceMatchesArraySort(DoubleSortCase testCase)
    {
        var random = new Random(42);
        var array = new double[1000];
        for (var i = 0; i < array.Length; i++)
        {
            array[i] = (i % 7) switch
            {
                0 => +0.0,
                1 => -0.0,
                _ => random.NextDouble() * 200.0 - 100.0,
            };
        }
        var expected = array.ToArray();
        Array.Sort(expected);

        testCase.Sort(array);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }
}
