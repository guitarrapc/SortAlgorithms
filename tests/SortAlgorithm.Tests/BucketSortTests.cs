using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class BucketSortTests
{
    [Test]
    [MethodDataSource(typeof(MockRandomData), nameof(MockRandomData.Generate))]
    [MethodDataSource(typeof(MockNegativePositiveRandomData), nameof(MockNegativePositiveRandomData.Generate))]
    [MethodDataSource(typeof(MockNegativeRandomData), nameof(MockNegativeRandomData.Generate))]
    [MethodDataSource(typeof(MockReversedData), nameof(MockReversedData.Generate))]
    [MethodDataSource(typeof(MockReversedWithDuplicatesData), nameof(MockReversedWithDuplicatesData.Generate))]
    [MethodDataSource(typeof(MockPipeorganData), nameof(MockPipeorganData.Generate))]
    [MethodDataSource(typeof(MockNearlySortedData), nameof(MockNearlySortedData.Generate))]
    [MethodDataSource(typeof(MockAllSameData), nameof(MockAllSameData.Generate))]
    [MethodDataSource(typeof(MockSameValuesData), nameof(MockSameValuesData.Generate))]
    [MethodDataSource(typeof(MockQuickSortWorstCaseData), nameof(MockQuickSortWorstCaseData.Generate))]
    [MethodDataSource(typeof(MockTwoDistinctValuesData), nameof(MockTwoDistinctValuesData.Generate))]
    [MethodDataSource(typeof(MockHalfZeroHalfOneData), nameof(MockHalfZeroHalfOneData.Generate))]
    [MethodDataSource(typeof(MockValleyRandomData), nameof(MockValleyRandomData.Generate))]
    [MethodDataSource(typeof(MockHighlySkewedData), nameof(MockHighlySkewedData.Generate))]
    public async Task SortResultOrderTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();


        BucketSort.SortBy(array.AsSpan(), x => x, stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockStabilityData), nameof(MockStabilityData.Generate))]
    public async Task StabilityTest(StabilityTestItem[] items)
    {
        // Test stability: equal elements should maintain relative order
        var stats = new StatisticsContext();

        BucketSort.SortBy(items.AsSpan(), x => x.Value, stats);

        // Verify sorting correctness - values should be in ascending order
        await Assert.That(items.Select(x => x.Value).ToArray()).IsEquivalentTo(MockStabilityData.Sorted, CollectionOrdering.Matching);

        // Verify stability: for each group of equal values, original order is preserved
        var value1Indices = items.Where(x => x.Value == 1).Select(x => x.OriginalIndex).ToArray();
        var value2Indices = items.Where(x => x.Value == 2).Select(x => x.OriginalIndex).ToArray();
        var value3Indices = items.Where(x => x.Value == 3).Select(x => x.OriginalIndex).ToArray();

        // Value 1 appeared at original indices 0, 2, 4 - should remain in this order
        await Assert.That(value1Indices).IsEquivalentTo(MockStabilityData.Sorted1, CollectionOrdering.Matching);

        // Value 2 appeared at original indices 1, 5 - should remain in this order
        await Assert.That(value2Indices).IsEquivalentTo(MockStabilityData.Sorted2, CollectionOrdering.Matching);

        // Value 3 appeared at original index 3
        await Assert.That(value3Indices).IsEquivalentTo(MockStabilityData.Sorted3, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockStabilityWithIdData), nameof(MockStabilityWithIdData.Generate))]
    public async Task StabilityTestWithComplex(StabilityTestItemWithId[] items)
    {
        // Test stability with more complex scenario - multiple equal values
        var stats = new StatisticsContext();

        BucketSort.SortBy(items.AsSpan(), x => x.Key, stats);

        // Expected: [2:B, 2:D, 2:F, 5:A, 5:C, 5:G, 8:E]
        // Keys are sorted, and elements with the same key maintain original order

        for (var i = 0; i < items.Length; i++)
        {
            await Assert.That(items[i].Key).IsEqualTo(MockStabilityWithIdData.Sorted[i].Key);
            await Assert.That(items[i].Id).IsEqualTo(MockStabilityWithIdData.Sorted[i].Id);
        }
    }

    [Test]
    [MethodDataSource(typeof(MockStabilityAllEqualsData), nameof(MockStabilityAllEqualsData.Generate))]
    public async Task StabilityTestWithAllEqual(StabilityTestItem[] items)
    {
        // Edge case: all elements have the same value
        // They should remain in original order
        var stats = new StatisticsContext();

        BucketSort.SortBy(items.AsSpan(), x => x.Value, stats);

        // All values are 1
        foreach (var item in items) await Assert.That(item.Value).IsEqualTo(1);

        // Original order should be preserved: 0, 1, 2, 3, 4
        await Assert.That(items.Select(x => x.OriginalIndex).ToArray()).IsEquivalentTo(MockStabilityAllEqualsData.Sorted, CollectionOrdering.Matching);
    }


    [Test]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BucketSort.SortBy(array.AsSpan(), x => x, stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        // 比較はバケツ内ソートからしか出ない（min/max 走査は生の int 比較で観測対象外）。この入力は
        // 値域 == n の順列なので、バケツ数 min(n, 値域) はバケツ幅を 1 にし、どのバケツも等値要素しか
        // 持たない。比較が起きるのは値域 > n の入力に限られる（TheoreticalValues* を参照）。
        await Assert.That(stats.CompareCount).IsEqualTo(0UL)
            .Because("バケツ幅 1 では並べ替える相手がバケツ内に存在しない");
    }

    /// <summary>
    /// バケツが複数の異なるキーを受け持つ入力。値は (0,1), (4,5), (8,9), ... と 2 個ずつ隣接し、
    /// 値域 2n-2 に対してバケツ幅 2 になるので、隣接ペアがちょうど同じバケツに入る。
    /// バケツ内ソートが働くのは値域 &gt; n のときだけで、これは CountingSort / PigeonholeSort が
    /// 値域を理由に拒否する、bucket sort 本来の担当入力でもある。
    /// </summary>
    private static int[] PairedSparseKeys(int n)
        => [.. Enumerable.Range(0, n).Select(i => (i / 2) * 4 + (i % 2))];

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = PairedSparseKeys(n);
        BucketSort.SortBy(sorted.AsSpan(), x => x, stats);

        // 固定費（バケツ相を除く）:
        // 1. min/max とキー抽出   : n 読み
        // 2. 配布の 2 パス目      : n 読み + n 書き（一時バッファへ）
        // 3. 書き戻し (temp→main) : n 読み + n 書き（OnRangeCopy）
        var fixedReads = (ulong)(3 * n);
        var fixedWrites = (ulong)(2 * n);

        // PairedSparseKeys は非空バケツ n/2 個・各 2 要素になるので、挿入対象は n/2 個。
        var insertionTargets = n / 2;

        // 昇順に入るバケツでは、挿入対象ごとに比較 1 回・読み 2 回（退避と比較相手）で、
        // 定位置の要素への書き戻しは省かれるため書き込みは 0。
        await Assert.That(stats.CompareCount).IsEqualTo((ulong)insertionTargets)
            .Because("バケツ内の比較が観測されていること");
        await Assert.That(stats.IndexWriteCount).IsEqualTo(fixedWrites)
            .Because("ソート済みバケツでは 1 要素も書き込まれない");
        await Assert.That(stats.IndexReadCount).IsEqualTo(fixedReads + (ulong)(2 * insertionTargets));
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    /// <summary>
    /// 値域 == n の順列ではバケツ数 min(n, 値域) がバケツ幅を 1 にするので、どのバケツも等値要素しか
    /// 持たず、バケツ内ソートは 1 度も比較しない。分配と書き戻しだけが残る形を厳密に押さえる。
    /// </summary>
    [Test]
    [Arguments(10)]
    [Arguments(100)]
    public async Task TheoreticalValuesDenseKeysTest(int n)
    {
        var stats = new StatisticsContext();
        var dense = Enumerable.Range(0, n).ToArray();
        BucketSort.SortBy(dense.AsSpan(), x => x, stats);

        await Assert.That(stats.IndexReadCount).IsEqualTo((ulong)(3 * n));
        await Assert.That(stats.IndexWriteCount).IsEqualTo((ulong)(2 * n));
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
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
        BucketSort.SortBy(reversed.AsSpan(), x => x, stats);

        // BucketSort on reversed data (with internal buffer tracking):
        // - Distribution is same as sorted (independent of order)
        // - Within each bucket, elements are reversed
        // - InsertionSort worst case: many shifts
        //
        // Actual observations:
        // n=10:  writes=30 (3n)
        // n=20:  writes=76 (3.8n)
        // n=50:  writes=262 (5.24n)
        // n=100: writes=640 (6.4n)

        // 逆順に入るバケツは必ずシフトするので、固定費 2n を超える書き込みが観測される。
        // 上限は全要素が 1 バケツに落ちた最悪ケース（bucket sort の O(n²) の姿）で押さえる。
        var fixedReads = (ulong)(3 * n);
        var fixedWrites = (ulong)(2 * n);
        var maxBucketCompares = (ulong)(n * (n - 1) / 2);

        await Assert.That(stats.IndexReadCount).IsGreaterThan(fixedReads);
        await Assert.That(stats.IndexWriteCount).IsGreaterThan(fixedWrites)
            .Because("逆順バケツは必ずシフトするので、バケツ内の書き込みが観測されること");
        await Assert.That(stats.IndexWriteCount).IsLessThanOrEqualTo(fixedWrites + maxBucketCompares + (ulong)(n - 1));
        await Assert.That(stats.CompareCount).IsGreaterThan(0UL);
        await Assert.That(stats.CompareCount).IsLessThanOrEqualTo(maxBucketCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
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
        BucketSort.SortBy(random.AsSpan(), x => x, stats);

        // 入力順によってバケツ内のシフト量は変わるが、固定費と最悪ケースの間には必ず収まる。
        var fixedReads = (ulong)(3 * n);
        var fixedWrites = (ulong)(2 * n);
        var maxBucketCompares = (ulong)(n * (n - 1) / 2);

        await Assert.That(stats.IndexReadCount).IsGreaterThan(fixedReads);
        await Assert.That(stats.IndexWriteCount).IsGreaterThanOrEqualTo(fixedWrites);
        await Assert.That(stats.IndexWriteCount).IsLessThanOrEqualTo(fixedWrites + maxBucketCompares + (ulong)(n - 1));
        await Assert.That(stats.CompareCount).IsGreaterThan(0UL)
            .Because("バケツ内の比較が観測されていること");
        await Assert.That(stats.CompareCount).IsLessThanOrEqualTo(maxBucketCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
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
        BucketSort.SortBy(allSame.AsSpan(), x => x, stats);

        // All elements are the same:
        // - min == max, early return after first pass
        // - No distribution or sorting needed

        // IndexReadCount: only for finding min/max
        var expectedReads = (ulong)n;

        // IndexWriteCount: 0 (early return)
        var expectedWrites = 0UL;

        // CompareCount: 0 (no sorting needed)
        var expectedCompares = 0UL;

        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    [Test]
    public async Task SortByWithoutComparableElementTest()
    {
        // SortBy orders strictly by the extracted key: the element type does not
        // need IComparable, and equal keys retain input order (stable).
        var records = new (int Key, string Name)[] { (3, "c"), (-5, "a"), (0, "b"), (-5, "a2"), (3, "c2") };
        BucketSort.SortBy(records.AsSpan(), x => x.Key);

        await Assert.That(records.Select(x => x.Key).ToArray())
            .IsEquivalentTo([-5, -5, 0, 3, 3], CollectionOrdering.Matching);
        await Assert.That(records.Select(x => x.Name).ToArray())
            .IsEquivalentTo(["a", "a2", "b", "c", "c2"], CollectionOrdering.Matching);
    }

    private readonly struct DescendingIntComparer : IComparer<int>
    {
        public int Compare(int x, int y) => y.CompareTo(x);
    }

    [Test]
    public async Task SortWithComparerAndBucketHintTest()
    {
        // The comparer defines the final order; the key selector is only a bucket-distribution
        // hint and must be order-consistent with the comparer (negated key for descending order).
        var stats = new StatisticsContext();
        var random = new Random(42);
        var array = Enumerable.Range(0, 500).Select(_ => random.Next(-1000, 1000)).ToArray();
        var expected = array.OrderByDescending(x => x).ToArray();

        BucketSort.Sort(array.AsSpan(), x => -x, new DescendingIntComparer(), stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    /// <summary>
    /// 要素が「書き込まれていないバッファーから現れる」ストリームになっていないこと。
    ///
    /// バケツは temp バッファーの隣接区間であって別配列ではない。以前はバケツごとに専用のバッファー ID
    /// (100+i) を振っていたため、消費側からは「temp に配った要素が 100 番台のどこかでソートされ、
    /// 誰も書いていない temp から最終コピーが行われる」ように見えていた。同じ記憶域が操作によって
    /// 別の識別子を名乗るのは、バッファー識別が持つべき意味に反する。
    /// </summary>
    [Test]
    public async Task ElementOperationsStayWithinTheBuffersTheAlgorithmCopiesBetweenTest()
    {
        var buffersTouched = new HashSet<int>();
        var buffersWritten = new HashSet<int>();
        var copySources = new HashSet<int>();
        var copyDestinations = new HashSet<int>();

        var context = new VisualizationContext(
            onCompare: (_, _, _, bi, bj) => { if (bi >= 0) buffersTouched.Add(bi); if (bj >= 0) buffersTouched.Add(bj); },
            onIndexRead: (_, b) => buffersTouched.Add(b),
            onIndexWrite: (_, b, _) => { buffersTouched.Add(b); buffersWritten.Add(b); },
            onRangeCopy: (_, _, _, src, dst, _) => { copySources.Add(src); copyDestinations.Add(dst); buffersTouched.Add(src); buffersWritten.Add(dst); });

        var random = new Random(42);
        var array = Enumerable.Range(0, 300).Select(_ => random.Next(0, 1000)).ToArray();
        var expected = array.OrderBy(x => x).ToArray();

        BucketSort.SortBy(array.AsSpan(), x => x, context);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching)
            .Because("観測を変えてもソート結果は変わらないこと");

        // メイン配列と一時バッファーの 2 つだけ。バケツごとの識別子は存在しない。
        await Assert.That(buffersTouched.Order().ToList()).IsEquivalentTo(new List<int> { 0, 1 })
            .Because($"観測されたバッファー: [{string.Join(",", buffersTouched.Order())}]");

        // 最終コピーは temp から main へ。書き込まれた補助バッファーは、そのコピー元と一致しなければ
        // 「誰も読まないバッファーに書いた」ことになる。
        await Assert.That(copySources).IsEquivalentTo(new HashSet<int> { 1 });
        await Assert.That(copyDestinations).IsEquivalentTo(new HashSet<int> { 0 });
        var auxWritten = buffersWritten.Where(b => b != 0).ToHashSet();
        await Assert.That(auxWritten).IsEquivalentTo(copySources)
            .Because($"書き込まれた補助バッファー [{string.Join(",", auxWritten.Order())}] と、最終コピーの読み出し元 [{string.Join(",", copySources.Order())}] が一致すること");
    }

    /// <summary>
    /// バケツ内ソートの比較が一時バッファーの座標で報告されること。
    /// 専用 ID を振っていた頃は、これらが temp とは別のバッファー上の出来事として流れていた。
    /// </summary>
    [Test]
    public async Task PerBucketSortIsReportedOnTheTempBufferTest()
    {
        var comparesOnTemp = 0;
        var context = new VisualizationContext(
            onCompare: (_, _, _, bi, bj) => { if (bi == 1 || bj == 1) comparesOnTemp++; });

        var random = new Random(42);
        var array = Enumerable.Range(0, 300).Select(_ => random.Next(0, 1000)).ToArray();

        BucketSort.SortBy(array.AsSpan(), x => x, context);

        await Assert.That(comparesOnTemp).IsGreaterThan(0)
            .Because("バケツ内の挿入ソートは一時バッファー上の操作として観測されること");
    }
}
