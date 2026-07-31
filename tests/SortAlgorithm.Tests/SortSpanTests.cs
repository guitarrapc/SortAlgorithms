using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class SortSpanTests
{
    [Test]
    public async Task CopyTo_ShouldCopyRangeToAnotherSortSpan()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        var destination = new int[5];
        var context = new StatisticsContext();

        var sourceSpan = new SortSpan<int, Comparer<int>, StatisticsContext>(source.AsSpan(), context, Comparer<int>.Default, 0);
        var destSpan = new SortSpan<int, Comparer<int>, StatisticsContext>(destination.AsSpan(), context, Comparer<int>.Default, 1);

        // Act
        sourceSpan.CopyTo(1, destSpan, 0, 3); // Copy [2, 3, 4] to destination[0..3]

        // Assert
        await Assert.That(destination[0]).IsEqualTo(2);
        await Assert.That(destination[1]).IsEqualTo(3);
        await Assert.That(destination[2]).IsEqualTo(4);
        await Assert.That(destination[3]).IsEqualTo(0); // Not copied
        await Assert.That(destination[4]).IsEqualTo(0); // Not copied
    }

    [Test]
    public async Task CopyTo_ShouldTrackStatistics()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        var destination = new int[5];
        var context = new StatisticsContext();

        var sourceSpan = new SortSpan<int, Comparer<int>, StatisticsContext>(source.AsSpan(), context, Comparer<int>.Default, 0);
        var destSpan = new SortSpan<int, Comparer<int>, StatisticsContext>(destination.AsSpan(), context, Comparer<int>.Default, 1);

        // Act
        sourceSpan.CopyTo(0, destSpan, 0, 3); // Copy 3 elements

        // Assert - Should count as 3 reads + 3 writes
        await Assert.That(context.IndexReadCount).IsEqualTo(3UL);
        await Assert.That(context.IndexWriteCount).IsEqualTo(3UL);
    }

    [Test]
    public async Task CopyTo_ShouldCopyToRegularSpan()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5 };
        var destination = new int[5];
        var context = new StatisticsContext();

        var sourceSpan = new SortSpan<int, Comparer<int>, StatisticsContext>(source.AsSpan(), context, Comparer<int>.Default, 0);

        // Act
        sourceSpan.CopyTo(2, destination.AsSpan(), 1, 2); // Copy [3, 4] to destination[1..3]

        // Assert
        await Assert.That(destination[0]).IsEqualTo(0); // Not copied
        await Assert.That(destination[1]).IsEqualTo(3);
        await Assert.That(destination[2]).IsEqualTo(4);
        await Assert.That(destination[3]).IsEqualTo(0); // Not copied
    }

    [Test]
    public async Task CopyTo_VerifyBetterThanLoopWrite()
    {
        // Arrange
        var source = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        var destination1 = new int[10];
        var destination2 = new int[10];
        var contextCopyTo = new StatisticsContext();
        var contextLoop = new StatisticsContext();

        var sourceSpan1 = new SortSpan<int, Comparer<int>, StatisticsContext>(source.AsSpan(), contextCopyTo, Comparer<int>.Default, 0);
        var destSpan1 = new SortSpan<int, Comparer<int>, StatisticsContext>(destination1.AsSpan(), contextCopyTo, Comparer<int>.Default, 1);

        var sourceSpan2 = new SortSpan<int, Comparer<int>, StatisticsContext>(source.AsSpan(), contextLoop, Comparer<int>.Default, 0);
        var destSpan2 = new SortSpan<int, Comparer<int>, StatisticsContext>(destination2.AsSpan(), contextLoop, Comparer<int>.Default, 1);

        // Act - using CopyTo
        sourceSpan1.CopyTo(0, destSpan1, 0, 10);

        // Act - using loop with Read/Write
        for (int i = 0; i < 10; i++)
        {
            destSpan2.Write(i, sourceSpan2.Read(i));
        }

        // Assert - Both should produce the same result
        await Assert.That(destination2).IsEquivalentTo(destination1, CollectionOrdering.Matching);

        // Assert - CopyTo should have the same statistics as loop
        // (Both are counted as reads + writes, but CopyTo is more efficient in tracking)
        await Assert.That(contextCopyTo.IndexReadCount).IsEqualTo(10UL);
        await Assert.That(contextCopyTo.IndexWriteCount).IsEqualTo(10UL);
        await Assert.That(contextLoop.IndexReadCount).IsEqualTo(10UL);
        await Assert.That(contextLoop.IndexWriteCount).IsEqualTo(10UL);
    }

    [Test]
    public async Task RawSpan_ShouldReturnUnderlyingSpanUnderNullContext()
    {
        // RawSpan exists for NullContext-gated fast paths (no observer to bypass).
        // SortSpan is a ref struct, so extract values before awaiting.
        var source = new[] { 1, 2, 3 };
        var span = new SortSpan<int, ComparableComparer<int>, NullContext>(source.AsSpan(), NullContext.Default, new ComparableComparer<int>(), 0);
        var length = span.RawSpan.Length;
        var second = span.RawSpan[1];

        await Assert.That(length).IsEqualTo(3);
        await Assert.That(second).IsEqualTo(2);
    }

    [Test]
    public async Task RawSpan_ShouldThrowUnderObservingContext()
    {
        // Accessing RawSpan with an observing context would silently lose element
        // operations, so the getter enforces the NullContext-only contract at runtime.
        var thrown = false;
        try
        {
            var source = new[] { 1, 2, 3 };
            var context = new StatisticsContext();
            var span = new SortSpan<int, ComparableComparer<int>, StatisticsContext>(source.AsSpan(), context, new ComparableComparer<int>(), 0);
            _ = span.RawSpan;
        }
        catch (InvalidOperationException)
        {
            thrown = true;
        }

        await Assert.That(thrown).IsTrue().Because("RawSpan must throw for observing contexts");
    }

    // ------------------------------------------------------------------
    // Mixed index/value and cross-buffer comparisons.
    //
    // An algorithm holding one operand in a local still knows where the other one lives. These
    // overloads exist so that knowledge reaches the context: reaching for the value-based overloads
    // with an inline Read reports a comparison that names neither operand (-1, -1), which a consumer
    // cannot place on any array.
    // ------------------------------------------------------------------

    private readonly record struct CompareEvent(int I, int J, int Result, int BufferI, int BufferJ);

    /// <summary>比較イベントと読み取りイベントを記録するコンテキスト。</summary>
    private static VisualizationContext Recording(List<CompareEvent> compares, List<(int Index, int BufferId)> reads)
        => new(
            onCompare: (i, j, r, bi, bj) => compares.Add(new CompareEvent(i, j, r, bi, bj)),
            onIndexRead: (i, b) => reads.Add((i, b)));

    /// <summary>降順比較子。プリミティブ特殊化がカスタム比較子を迂回しないことの検証用。</summary>
    private readonly struct DescendingIntComparer : IComparer<int>
    {
        public int Compare(int x, int y) => y.CompareTo(x);
    }

    /// <summary>
    /// index が左のオーバーロードは、左オペランドに実インデックスとバッファ ID を、
    /// 右オペランド（バッファ上に無い値）に -1 を報告する。
    /// </summary>
    [Test]
    public async Task IsElementGreaterThan_IndexOnLeft_ReportsIndexForTheOperandThatHasOne()
    {
        var compares = new List<CompareEvent>();
        var reads = new List<(int Index, int BufferId)>();
        var source = new[] { 10, 20, 30 };
        var span = new SortSpan<int, ComparableComparer<int>, VisualizationContext>(
            source.AsSpan(), Recording(compares, reads), new ComparableComparer<int>(), 7);

        var result = span.IsElementGreaterThan(2, 25);

        await Assert.That(result).IsTrue().Because("30 > 25");
        await Assert.That(reads).IsEquivalentTo(new List<(int, int)> { (2, 7) });
        await Assert.That(compares).HasCount(1);
        await Assert.That(compares[0].I).IsEqualTo(2);
        await Assert.That(compares[0].BufferI).IsEqualTo(7);
        await Assert.That(compares[0].J).IsEqualTo(-1).Because("値はバッファ上に無い");
        await Assert.That(compares[0].BufferJ).IsEqualTo(-1);
        await Assert.That(compares[0].Result).IsGreaterThan(0);
    }

    /// <summary>value が左のオーバーロードはオペランド順を保つので、報告される符号が算法の見た符号と一致する。</summary>
    [Test]
    public async Task IsValueLessThan_ValueOnLeft_PreservesOperandOrderAndSign()
    {
        var compares = new List<CompareEvent>();
        var reads = new List<(int Index, int BufferId)>();
        var source = new[] { 10, 20, 30 };
        var span = new SortSpan<int, ComparableComparer<int>, VisualizationContext>(
            source.AsSpan(), Recording(compares, reads), new ComparableComparer<int>(), 0);

        var result = span.IsValueLessThan(5, 1);

        await Assert.That(result).IsTrue().Because("5 < 20");
        await Assert.That(compares).HasCount(1);
        await Assert.That(compares[0].I).IsEqualTo(-1);
        await Assert.That(compares[0].BufferI).IsEqualTo(-1);
        await Assert.That(compares[0].J).IsEqualTo(1);
        await Assert.That(compares[0].Result).IsLessThan(0).Because("符号は Compare(value, span[j]) の向き");
    }

    /// <summary>スライスに対しては Offset を加えた絶対インデックスとスライスのバッファ ID で報告する。</summary>
    [Test]
    public async Task MixedComparisons_ReportSliceCoordinates()
    {
        var compares = new List<CompareEvent>();
        var reads = new List<(int Index, int BufferId)>();
        var source = new[] { 0, 0, 0, 10, 20, 30 };
        var outer = new SortSpan<int, ComparableComparer<int>, VisualizationContext>(
            source.AsSpan(), Recording(compares, reads), new ComparableComparer<int>(), 0);
        var sliced = outer.Slice(3, 3, 42);

        sliced.IsElementGreaterThan(1, 5);

        await Assert.That(reads).IsEquivalentTo(new List<(int, int)> { (4, 42) });
        await Assert.That(compares[0].I).IsEqualTo(4).Because("Offset 3 + index 1");
        await Assert.That(compares[0].BufferI).IsEqualTo(42);
    }

    /// <summary>out 版は読み取った要素を返し、読み取りイベントは 1 回だけ。</summary>
    [Test]
    public async Task IsElementGreaterThan_OutVariant_YieldsElementWithASingleRead()
    {
        var compares = new List<CompareEvent>();
        var reads = new List<(int Index, int BufferId)>();
        var source = new[] { 10, 20, 30 };
        var span = new SortSpan<int, ComparableComparer<int>, VisualizationContext>(
            source.AsSpan(), Recording(compares, reads), new ComparableComparer<int>(), 0);

        var result = span.IsElementGreaterThan(1, 15, out var element);

        await Assert.That(result).IsTrue();
        await Assert.That(element).IsEqualTo(20);
        await Assert.That(reads).HasCount(1).Because("要素を返すために読み直してはならない");
        await Assert.That(compares).HasCount(1);
        await Assert.That(compares[0].I).IsEqualTo(1);
    }

    /// <summary>
    /// 新オーバーロードは、置き換え対象である <c>IsGreaterThan(Read(i), value)</c> と
    /// 読み取り数・比較数が完全に一致しなければならない。差が出れば統計の互換性が壊れる。
    /// </summary>
    [Test]
    public async Task MixedComparisons_MatchOperationCountsOfTheInlineReadForm()
    {
        var source = new[] { 10, 20, 30, 40 };

        var oldStats = new StatisticsContext();
        RunInlineReadForm(source, oldStats);

        var newStats = new StatisticsContext();
        RunMixedForm(source, newStats);

        await Assert.That(newStats.IndexReadCount).IsEqualTo(oldStats.IndexReadCount);
        await Assert.That(newStats.CompareCount).IsEqualTo(oldStats.CompareCount);

        static void RunInlineReadForm(int[] source, StatisticsContext context)
        {
            var s = new SortSpan<int, ComparableComparer<int>, StatisticsContext>(source.AsSpan(), context, new ComparableComparer<int>(), 0);
            for (var i = 0; i < source.Length; i++) _ = s.IsGreaterThan(s.Read(i), 25);
        }

        static void RunMixedForm(int[] source, StatisticsContext context)
        {
            var s = new SortSpan<int, ComparableComparer<int>, StatisticsContext>(source.AsSpan(), context, new ComparableComparer<int>(), 0);
            for (var i = 0; i < source.Length; i++) _ = s.IsElementGreaterThan(i, 25);
        }
    }

    /// <summary>
    /// クロスバッファ比較は両オペランドのインデックスとバッファ ID を報告する。
    /// マージ系ではインデックスだけではどちらの配列か決まらない。
    /// </summary>
    [Test]
    public async Task IsLessAcross_ReportsBothIndicesWithTheirOwnBufferIds()
    {
        var compares = new List<CompareEvent>();
        var reads = new List<(int Index, int BufferId)>();
        var context = Recording(compares, reads);
        var main = new[] { 10, 20, 30 };
        var aux = new[] { 5, 25, 35 };
        var mainSpan = new SortSpan<int, ComparableComparer<int>, VisualizationContext>(main.AsSpan(), context, new ComparableComparer<int>(), 0);
        var auxSpan = new SortSpan<int, ComparableComparer<int>, VisualizationContext>(aux.AsSpan(), context, new ComparableComparer<int>(), 1);

        var result = mainSpan.IsLessAcross(1, auxSpan, 1);

        await Assert.That(result).IsTrue().Because("main[1]=20 < aux[1]=25");
        await Assert.That(reads).IsEquivalentTo(new List<(int, int)> { (1, 0), (1, 1) })
            .Because("両バッファの読み取りが報告されること");
        await Assert.That(compares).HasCount(1);
        await Assert.That(compares[0].I).IsEqualTo(1);
        await Assert.That(compares[0].BufferI).IsEqualTo(0);
        await Assert.That(compares[0].J).IsEqualTo(1);
        await Assert.That(compares[0].BufferJ).IsEqualTo(1).Because("インデックスだけではバッファを区別できない");
    }

    /// <summary>
    /// プリミティブ特殊化はカスタム比較子を迂回してはならない。
    /// 降順比較子では全オーバーロードの真偽が既定比較子と反転する。
    /// </summary>
    [Test]
    public async Task MixedComparisons_HonorACustomComparerUnderNullContext()
    {
        var source = new[] { 10, 20, 30 };
        var aux = new[] { 20 };

        var (asc, desc) = Evaluate(source, aux);

        // 昇順: span[2]=30 > 15 は true、15 < span[1]=20 は true、main[2]=30 < aux[0]=20 は false
        await Assert.That(asc).IsEquivalentTo(new List<bool> { true, true, false });
        // 降順比較子では 3 つとも反転する
        await Assert.That(desc).IsEquivalentTo(new List<bool> { false, false, true })
            .Because("プリミティブ特殊化が比較子を迂回すると昇順と同じ結果になる");

        static (List<bool> Ascending, List<bool> Descending) Evaluate(int[] source, int[] aux)
        {
            var ascending = new List<bool>();
            {
                var s = new SortSpan<int, ComparableComparer<int>, NullContext>(source.AsSpan(), NullContext.Default, new ComparableComparer<int>(), 0);
                var a = new SortSpan<int, ComparableComparer<int>, NullContext>(aux.AsSpan(), NullContext.Default, new ComparableComparer<int>(), 1);
                ascending.Add(s.IsElementGreaterThan(2, 15));
                ascending.Add(s.IsValueLessThan(15, 1));
                ascending.Add(s.IsLessAcross(2, a, 0));
            }

            var descending = new List<bool>();
            {
                var s = new SortSpan<int, DescendingIntComparer, NullContext>(source.AsSpan(), NullContext.Default, new DescendingIntComparer(), 0);
                var a = new SortSpan<int, DescendingIntComparer, NullContext>(aux.AsSpan(), NullContext.Default, new DescendingIntComparer(), 1);
                descending.Add(s.IsElementGreaterThan(2, 15));
                descending.Add(s.IsValueLessThan(15, 1));
                descending.Add(s.IsLessAcross(2, a, 0));
            }

            return (ascending, descending);
        }
    }

    /// <summary>
    /// 観測コンテキストでも同じ真偽でなければならない（観測経路は比較子を直接呼ぶため、
    /// プリミティブ経路との食い違いがここで出る）。
    /// </summary>
    [Test]
    public async Task MixedComparisons_AgreeBetweenObservedAndNullContextPaths()
    {
        var source = new[] { 10, 20, 30 };

        for (var i = 0; i < source.Length; i++)
        {
            foreach (var value in new[] { 5, 10, 20, 25, 35 })
            {
                var (fast, observed) = Evaluate(source, i, value);
                await Assert.That(observed).IsEquivalentTo(fast)
                    .Because($"index {i} vs value {value}: 観測経路と NullContext 経路の結果が一致すること");
            }
        }

        static (List<bool> Fast, List<bool> Observed) Evaluate(int[] source, int i, int value)
        {
            var fast = new List<bool>();
            {
                var s = new SortSpan<int, ComparableComparer<int>, NullContext>(source.AsSpan(), NullContext.Default, new ComparableComparer<int>(), 0);
                fast.Add(s.IsElementLessThan(i, value));
                fast.Add(s.IsElementLessOrEqual(i, value));
                fast.Add(s.IsElementGreaterThan(i, value));
                fast.Add(s.IsElementGreaterOrEqual(i, value));
                fast.Add(s.IsValueLessThan(value, i));
                fast.Add(s.IsValueLessOrEqual(value, i));
                fast.Add(s.IsValueGreaterThan(value, i));
                fast.Add(s.IsValueGreaterOrEqual(value, i));
                fast.Add(s.IsElementGreaterThan(i, value, out _));
            }

            var observed = new List<bool>();
            {
                var context = new StatisticsContext();
                var s = new SortSpan<int, ComparableComparer<int>, StatisticsContext>(source.AsSpan(), context, new ComparableComparer<int>(), 0);
                observed.Add(s.IsElementLessThan(i, value));
                observed.Add(s.IsElementLessOrEqual(i, value));
                observed.Add(s.IsElementGreaterThan(i, value));
                observed.Add(s.IsElementGreaterOrEqual(i, value));
                observed.Add(s.IsValueLessThan(value, i));
                observed.Add(s.IsValueLessOrEqual(value, i));
                observed.Add(s.IsValueGreaterThan(value, i));
                observed.Add(s.IsValueGreaterOrEqual(value, i));
                observed.Add(s.IsElementGreaterThan(i, value, out _));
            }

            return (fast, observed);
        }
    }
}
