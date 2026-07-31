using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

/// <summary>
/// バケツ内のソートは以前 raw <see cref="Span{T}"/> と比較子を直接使う private ヘルパで行われており、
/// バケツ内の読み書きと比較が観測ストリームから丸ごと落ちていた。現在は <c>InsertionSort.SortCore</c> に
/// 委譲しており、ここの期待値はその「見えるようになった分」を含む。
///
/// 期待値の導出に使った実測値は sandbox/DotnetFiles/BucketSortIntegerCounts.cs で再現できる。
/// </summary>
[InheritsTests]
public class BucketSortIntegerTests : IntegerSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => BucketSortInteger.Sort(span, context);

    // Distribution + per-bucket insertion sort: compares and writes always occur; no swaps.
    protected override CountExpectation SortedInputCompares => CountExpectation.NonZero;
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    /// <summary>
    /// アルゴリズムと同じヒューリスティックでバケツ数を求める。
    /// 本テストの入力はいずれも 0..n-1 の順列（range == n &gt; bucketCount）なので、
    /// range によるバケツ数の切り詰めは起こらず、全バケツが非空になる。
    /// </summary>
    private static int BucketCountFor(int n) => Math.Max(2, Math.Min(1000, (int)Math.Sqrt(n)));

    /// <summary>
    /// バケツ相を除いた固定費。
    /// 読み取り: min/max の初期化 2 + 走査 (n-1) + 配布 1 パス目 n + 2 パス目 n + 書き戻し n = 4n+1
    /// 書き込み: 一時バッファへの配布 n + 書き戻し n = 2n
    /// 比較    : 走査で 1 要素あたり 2 回 = 2(n-1)、加えて min と max の一致判定 1 回 = 2n-1
    /// </summary>
    private static (ulong Reads, ulong Writes, ulong Compares) FixedCost(int n)
        => ((ulong)(4 * n + 1), (ulong)(2 * n), (ulong)(2 * n - 1));

    /// <summary>
    /// バケツ相の読み取り回数は、比較回数から幾何に依存せず決まる。
    /// SortCore は挿入対象ごとに tmp を 1 回読み、比較ごとに相手を 1 回読むので、
    /// バケツ読み取り = (挿入対象の総数) + (バケツ比較数)。
    /// 挿入対象の総数は Σ(m_i - 1) = n - 非空バケツ数。
    ///
    /// この関係が崩れるのは、バケツ相の操作が観測から漏れているか二重計上されているとき。
    /// </summary>
    private static async Task AssertReadsFollowFromCompares(int n, StatisticsContext stats)
    {
        var (fixedReads, _, fixedCompares) = FixedCost(n);
        var bucketCompares = stats.CompareCount - fixedCompares;
        var expectedReads = fixedReads + (ulong)(n - BucketCountFor(n)) + bucketCompares;

        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads)
            .Because($"n={n}: 読み取り {stats.IndexReadCount} は固定費 {fixedReads} + 挿入対象 {n - BucketCountFor(n)} + バケツ比較 {bucketCompares} と一致すべき");
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        BucketSortInteger.Sort(sorted.AsSpan(), stats);

        var bucketCount = BucketCountFor(n);
        var (_, fixedWrites, fixedCompares) = FixedCost(n);

        // ソート済みならバケツ内も昇順なので、SortCore は要素ごとに 1 回だけ比較して 1 度も書かない
        // （定位置の要素に対する書き戻しを省く最適化）。
        var expectedCompares = fixedCompares + (ulong)(n - bucketCount);
        var expectedReads = (ulong)(6 * n + 1 - 2 * bucketCount);

        await Assert.That(stats.IndexWriteCount).IsEqualTo(fixedWrites)
            .Because("ソート済みバケツでは 1 要素も書き込まれない");
        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        await Assert.That(stats.CompareCount).IsGreaterThan(fixedCompares)
            .Because("バケツ内の比較が観測されていること（以前は raw Span 経由で 1 件も出ていなかった）");
        await AssertReadsFollowFromCompares(n, stats);
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        BucketSortInteger.Sort(reversed.AsSpan(), stats);

        var (_, fixedWrites, fixedCompares) = FixedCost(n);

        // 各バケツは完全な逆順になる。サイズ m のバケツで比較 m(m-1)/2、書き込み m(m-1)/2 + (m-1)。
        // 上限は全要素が 1 バケツに落ちた最悪ケースで押さえる。
        var maxBucketCompares = (ulong)(n * (n - 1) / 2);
        var maxBucketWrites = maxBucketCompares + (ulong)(n - 1);

        await Assert.That(stats.IndexWriteCount).IsGreaterThan(fixedWrites)
            .Because("逆順バケツは必ずシフトするので、バケツ内の書き込みが観測されること");
        await Assert.That(stats.IndexWriteCount).IsLessThanOrEqualTo(fixedWrites + maxBucketWrites);
        await Assert.That(stats.CompareCount).IsGreaterThan(fixedCompares);
        await Assert.That(stats.CompareCount).IsLessThanOrEqualTo(fixedCompares + maxBucketCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        await AssertReadsFollowFromCompares(n, stats);
    }

    [Test]
    [Arguments(10, 42)]
    [Arguments(10, 1234)]
    [Arguments(20, 42)]
    [Arguments(20, 1234)]
    [Arguments(50, 42)]
    [Arguments(50, 1234)]
    [Arguments(100, 42)]
    [Arguments(100, 1234)]
    public async Task TheoreticalValuesRandomTest(int n, int seed)
    {
        var stats = new StatisticsContext();
        var random = TestHelpers.ShuffledRange(n, seed);
        BucketSortInteger.Sort(random.AsSpan(), stats);

        var (_, fixedWrites, fixedCompares) = FixedCost(n);
        var maxBucketCompares = (ulong)(n * (n - 1) / 2);
        var maxBucketWrites = maxBucketCompares + (ulong)(n - 1);

        await Assert.That(stats.IndexWriteCount).IsGreaterThanOrEqualTo(fixedWrites);
        await Assert.That(stats.IndexWriteCount).IsLessThanOrEqualTo(fixedWrites + maxBucketWrites);
        await Assert.That(stats.CompareCount).IsGreaterThan(fixedCompares)
            .Because("バケツ内の比較が観測されていること");
        await Assert.That(stats.CompareCount).IsLessThanOrEqualTo(fixedCompares + maxBucketCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        await AssertReadsFollowFromCompares(n, stats);
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesAllSameTest(int n)
    {
        var stats = new StatisticsContext();
        var allSame = Enumerable.Repeat(42, n).ToArray();
        BucketSortInteger.Sort(allSame.AsSpan(), stats);

        // min == max と分かった時点で打ち切るため、配布もバケツソートも走らない。
        var expectedReads = (ulong)n + 1;
        var expectedWrites = 0UL;
        var expectedCompares = (ulong)n * 2 - 1;

        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    /// <summary>
    /// バケツ内の操作は一時バッファ (BUFFER_TEMP) 上で行われ、そのバッファ座標で報告される。
    /// 以前は raw Span で処理していたためバケツ内の操作が 1 件も報告されず、
    /// 一時バッファは「書き込まれてから読み戻されるだけの箱」に見えていた。
    /// </summary>
    [Test]
    public async Task BucketSortReportsPerBucketOperationsOnTheTempBufferTest()
    {
        const int n = 64;
        var compares = new List<(int I, int J, int BufferI, int BufferJ)>();
        var context = new VisualizationContext(
            onCompare: (i, j, _, bi, bj) => compares.Add((i, j, bi, bj)));

        var data = Enumerable.Range(0, n).Reverse().ToArray();
        BucketSortInteger.Sort(data.AsSpan(), context);

        await Assert.That(data).IsEquivalentTo(Enumerable.Range(0, n).ToArray());

        // 一時バッファ上の比較 = バケツ内ソート。走査相はバッファ上に無い値どうしの比較なので (-1,-1)。
        var onTemp = compares.Where(c => c.BufferI == 1 || c.BufferJ == 1).ToList();
        await Assert.That(onTemp).IsNotEmpty()
            .Because("バケツ内の比較が一時バッファの座標で報告されていること");
        await Assert.That(onTemp.All(c => c.I >= 0 && c.I < n)).IsTrue()
            .Because($"一時バッファ内の絶対位置であること: [{string.Join(",", onTemp.Select(c => c.I).Distinct().Order())}]");
        await Assert.That(onTemp.All(c => c.BufferI == 1 && c.J == -1 && c.BufferJ == -1)).IsTrue()
            .Because("挿入ソートは「バッファ上の要素 vs 退避中の値」を比較する");
    }
}
