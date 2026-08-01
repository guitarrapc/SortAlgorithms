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
    /// バケツが複数の異なるキーを受け持つ入力を作る。バケツ数は min(n, 値域) なので、値域 == n の順列では
    /// バケツ幅が 1 になり、各バケツは等値要素しか持たない —— バケツ内ソートが働くのは値域 &gt; n のとき、
    /// つまり CountingSort / PigeonholeSort が値域を理由に拒否する、bucket sort 本来の担当入力に限られる。
    ///
    /// 値は (0,1), (4,5), (8,9), ... と 2 個ずつ隣接する。値域は 2n-2 なのでバケツ幅は 2 になり、
    /// 隣接ペアがちょうど同じバケツに入って、非空バケツが n/2 個・各 2 要素という決定的な形になる。
    /// </summary>
    private static int[] PairedSparseKeys(int n)
        => [.. Enumerable.Range(0, n).Select(i => (i / 2) * 4 + (i % 2))];

    /// <summary>
    /// アルゴリズムと同じ規則でバケツを割り当て、非空バケツ数を数える。
    /// バケツ相の演算数を決めるのはバケツ数ではなく非空バケツ数で、空のバケツは何もしない。
    /// </summary>
    private static int NonEmptyBucketsFor(int[] data)
    {
        var min = data.Min();
        long range = (long)data.Max() - min + 1;
        var bucketCount = (int)Math.Min(data.Length, range);
        var bucketSize = Math.Max(1, (range + bucketCount - 1) / bucketCount);
        return data.Select(v => Math.Min((int)((v - (long)min) / bucketSize), bucketCount - 1)).Distinct().Count();
    }

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
    private static async Task AssertReadsFollowFromCompares(int[] data, StatisticsContext stats)
    {
        var n = data.Length;
        var nonEmpty = NonEmptyBucketsFor(data);
        var (fixedReads, _, fixedCompares) = FixedCost(n);
        var bucketCompares = stats.CompareCount - fixedCompares;
        var expectedReads = fixedReads + (ulong)(n - nonEmpty) + bucketCompares;

        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads)
            .Because($"n={n}: 読み取り {stats.IndexReadCount} は固定費 {fixedReads} + 挿入対象 {n - nonEmpty} + バケツ比較 {bucketCompares} と一致すべき");
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = PairedSparseKeys(n);
        BucketSortInteger.Sort(sorted.AsSpan(), stats);

        var nonEmpty = NonEmptyBucketsFor(sorted);
        var (fixedReads, fixedWrites, fixedCompares) = FixedCost(n);

        // ソート済みならバケツ内も昇順なので、SortCore は要素ごとに 1 回だけ比較して 1 度も書かない
        // （定位置の要素に対する書き戻しを省く最適化）。
        var expectedCompares = fixedCompares + (ulong)(n - nonEmpty);
        // 挿入対象ごとに退避の 1 読み + 比較の相手 1 読み = 2(n - 非空バケツ数)
        var expectedReads = fixedReads + (ulong)(2 * (n - nonEmpty));

        await Assert.That(nonEmpty).IsLessThan(n)
            .Because("バケツ内ソートが働く入力であること（非空バケツ数 < n なら少なくとも 1 つのバケツに 2 要素以上ある）");
        await Assert.That(stats.IndexWriteCount).IsEqualTo(fixedWrites)
            .Because("ソート済みバケツでは 1 要素も書き込まれない");
        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        await Assert.That(stats.CompareCount).IsGreaterThan(fixedCompares)
            .Because("バケツ内の比較が観測されていること（以前は raw Span 経由で 1 件も出ていなかった）");
        await AssertReadsFollowFromCompares(sorted, stats);
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = PairedSparseKeys(n).Reverse().ToArray();
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

        await AssertReadsFollowFromCompares(reversed, stats);
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
        var shuffleRandom = new Random(seed);
        var random = PairedSparseKeys(n).OrderBy(_ => shuffleRandom.Next()).ToArray();
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

        await AssertReadsFollowFromCompares(random, stats);
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

        // 値域 > n の入力。バケツ幅が 2 になり、隣接ペアが同じバケツに入ってバケツ内ソートが実際に走る。
        var data = PairedSparseKeys(n).Reverse().ToArray();
        BucketSortInteger.Sort(data.AsSpan(), context);

        await Assert.That(data).IsEquivalentTo(PairedSparseKeys(n));

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
