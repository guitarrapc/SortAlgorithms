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

    /// <summary>A context that records compare and read events.</summary>
    private static VisualizationContext Recording(List<CompareEvent> compares, List<(int Index, int BufferId)> reads)
        => new(
            onCompare: (i, j, r, bi, bj) => compares.Add(new CompareEvent(i, j, r, bi, bj)),
            onIndexRead: (i, b) => reads.Add((i, b)));

    /// <summary>A descending comparer, used to verify that the primitive specialization does not bypass a custom comparer.</summary>
    private readonly struct DescendingIntComparer : IComparer<int>
    {
        public int Compare(int x, int y) => y.CompareTo(x);
    }

    /// <summary>
    /// The index-on-the-left overloads report the real index and buffer id for the left operand, and -1 for
    /// the right operand, which is a value that lives in no buffer.
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
        await Assert.That(compares[0].J).IsEqualTo(-1).Because("the value lives in no buffer");
        await Assert.That(compares[0].BufferJ).IsEqualTo(-1);
        await Assert.That(compares[0].Result).IsGreaterThan(0);
    }

    /// <summary>The value-on-the-left overloads keep the operand order, so the reported sign matches the sign the algorithm saw.</summary>
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
        await Assert.That(compares[0].Result).IsLessThan(0).Because("the sign follows Compare(value, span[j])");
    }

    /// <summary>A slice reports the absolute index (its offset plus the local index) and the slice's own buffer id.</summary>
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

    /// <summary>The out variant hands back the element it read, and announces exactly one read.</summary>
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
        await Assert.That(reads).HasCount(1).Because("returning the element must not cost a second read");
        await Assert.That(compares).HasCount(1);
        await Assert.That(compares[0].I).IsEqualTo(1);
    }

    /// <summary>
    /// The new overloads must match the read and compare counts of the <c>IsGreaterThan(Read(i), value)</c>
    /// form they replace, exactly. A difference would break the comparability of the published statistics.
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
    /// A cross-buffer comparison reports the index and buffer id of both operands. In the merge family an
    /// index alone does not say which array the element came from.
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
            .Because("the read from each buffer must be reported");
        await Assert.That(compares).HasCount(1);
        await Assert.That(compares[0].I).IsEqualTo(1);
        await Assert.That(compares[0].BufferI).IsEqualTo(0);
        await Assert.That(compares[0].J).IsEqualTo(1);
        await Assert.That(compares[0].BufferJ).IsEqualTo(1).Because("an index alone cannot tell the two buffers apart");
    }

    /// <summary>
    /// The primitive specialization must not bypass a custom comparer. Under a descending comparer every
    /// overload returns the opposite of what it returns under the default comparer.
    /// </summary>
    [Test]
    public async Task MixedComparisons_HonorACustomComparerUnderNullContext()
    {
        var source = new[] { 10, 20, 30 };
        var aux = new[] { 20 };

        var (asc, desc) = Evaluate(source, aux);

        // Ascending: span[2]=30 > 15 is true, 15 < span[1]=20 is true, main[2]=30 < aux[0]=20 is false
        await Assert.That(asc).IsEquivalentTo(new List<bool> { true, true, false });
        // A descending comparer flips all three
        await Assert.That(desc).IsEquivalentTo(new List<bool> { false, false, true })
            .Because("a specialization that bypassed the comparer would give the ascending result here");

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
    /// An observing context must agree on every result: the observed path calls the comparer directly, so a
    /// disagreement with the primitive path shows up here.
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
                    .Because($"index {i} vs value {value}: the observed and NullContext paths must agree");
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

    // ------------------------------------------------------------------
    // out overloads, for callers that still need the value they just compared.
    // The span performs the read and hands the value back, so the reported location is the one the span
    // itself produced and cannot be wrong.
    // ------------------------------------------------------------------

    /// <summary>The out overloads report both indices and read each operand exactly once.</summary>
    [Test]
    public async Task IsLessAt_OutVariant_ReportsBothIndicesAndReadsEachOnce()
    {
        var compares = new List<CompareEvent>();
        var reads = new List<(int Index, int BufferId)>();
        var source = new[] { 10, 20, 30 };
        var span = new SortSpan<int, ComparableComparer<int>, VisualizationContext>(
            source.AsSpan(), Recording(compares, reads), new ComparableComparer<int>(), 4);

        var result = span.IsLessAt(0, 2, out var a0, out var a2);

        await Assert.That(result).IsTrue().Because("10 < 30");
        await Assert.That(a0).IsEqualTo(10);
        await Assert.That(a2).IsEqualTo(30);
        await Assert.That(reads).IsEquivalentTo(new List<(int, int)> { (0, 4), (2, 4) })
            .Because("returning the values must not cost a second read");
        await Assert.That(compares).HasCount(1);
        await Assert.That((compares[0].I, compares[0].J, compares[0].BufferI, compares[0].BufferJ))
            .IsEqualTo((0, 2, 4, 4));
    }

    /// <summary>The cross-buffer out overloads keep each operand's buffer id.</summary>
    [Test]
    public async Task IsLessOrEqualAcross_OutVariant_KeepsEachOperandsBuffer()
    {
        var compares = new List<CompareEvent>();
        var reads = new List<(int Index, int BufferId)>();
        var context = Recording(compares, reads);
        var main = new[] { 10, 20, 30 };
        var aux = new[] { 5, 25 };
        var mainSpan = new SortSpan<int, ComparableComparer<int>, VisualizationContext>(main.AsSpan(), context, new ComparableComparer<int>(), 0);
        var auxSpan = new SortSpan<int, ComparableComparer<int>, VisualizationContext>(aux.AsSpan(), context, new ComparableComparer<int>(), 1);

        var result = mainSpan.IsLessOrEqualAcross(1, auxSpan, 1, out var mv, out var av);

        await Assert.That(result).IsTrue().Because("main[1]=20 <= aux[1]=25");
        await Assert.That(mv).IsEqualTo(20);
        await Assert.That(av).IsEqualTo(25);
        await Assert.That(reads).IsEquivalentTo(new List<(int, int)> { (1, 0), (1, 1) });
        await Assert.That((compares[0].I, compares[0].BufferI, compares[0].J, compares[0].BufferJ))
            .IsEqualTo((1, 0, 1, 1));
    }

    /// <summary>
    /// The strict cross-buffer out overloads keep each operand's buffer identity and read each operand once.
    /// A merge writes back the value returned here, so a re-read would announce the same element twice.
    /// </summary>
    [Test]
    public async Task StrictAcross_OutVariants_KeepEachOperandsBufferAndReadEachOnce()
    {
        var main = new[] { 10, 20, 30 };
        var aux = new[] { 5, 25 };

        var lessCompares = new List<CompareEvent>();
        var lessReads = new List<(int Index, int BufferId)>();
        var lessContext = Recording(lessCompares, lessReads);
        var lessResult = new SortSpan<int, ComparableComparer<int>, VisualizationContext>(main.AsSpan(), lessContext, new ComparableComparer<int>(), 0)
            .IsLessAcross(1, new SortSpan<int, ComparableComparer<int>, VisualizationContext>(aux.AsSpan(), lessContext, new ComparableComparer<int>(), 1), 1, out var lessMain, out var lessAux);

        await Assert.That(lessResult).IsTrue().Because("main[1]=20 < aux[1]=25");
        await Assert.That((lessMain, lessAux)).IsEqualTo((20, 25));
        await Assert.That(lessReads).IsEquivalentTo(new List<(int, int)> { (1, 0), (1, 1) })
            .Because("returning the values must not cost a second read");
        await Assert.That(lessCompares.Count).IsEqualTo(1);
        await Assert.That((lessCompares[0].I, lessCompares[0].BufferI, lessCompares[0].J, lessCompares[0].BufferJ))
            .IsEqualTo((1, 0, 1, 1));

        var greaterCompares = new List<CompareEvent>();
        var greaterReads = new List<(int Index, int BufferId)>();
        var greaterContext = Recording(greaterCompares, greaterReads);
        var greaterResult = new SortSpan<int, ComparableComparer<int>, VisualizationContext>(main.AsSpan(), greaterContext, new ComparableComparer<int>(), 0)
            .IsGreaterAcross(2, new SortSpan<int, ComparableComparer<int>, VisualizationContext>(aux.AsSpan(), greaterContext, new ComparableComparer<int>(), 1), 1, out var greaterMain, out var greaterAux);

        await Assert.That(greaterResult).IsTrue().Because("main[2]=30 > aux[1]=25");
        await Assert.That((greaterMain, greaterAux)).IsEqualTo((30, 25));
        await Assert.That(greaterReads).IsEquivalentTo(new List<(int, int)> { (2, 0), (1, 1) });
        await Assert.That(greaterCompares.Count).IsEqualTo(1);
        await Assert.That((greaterCompares[0].I, greaterCompares[0].BufferI, greaterCompares[0].J, greaterCompares[0].BufferJ))
            .IsEqualTo((2, 0, 1, 1));
    }

    /// <summary>
    /// A strict comparison must be false on equal operands. Merge stability rests on exactly that, so all
    /// three equivalence classes (less / equal / greater) are pinned on both the observed and the NullContext path.
    /// </summary>
    [Test]
    [Arguments(5, false, true)]   // main(20) > aux(5)
    [Arguments(20, false, false)] // main == aux: neither strict comparison holds
    [Arguments(30, true, false)]  // main(20) < aux(30)
    public async Task StrictAcross_OutVariants_TreatEqualAsNeitherLessNorGreater(int auxValue, bool expectedLess, bool expectedGreater)
    {
        var main = new[] { 20 };
        var aux = new[] { auxValue };

        var fastLess = new SortSpan<int, ComparableComparer<int>, NullContext>(main.AsSpan(), NullContext.Default, new ComparableComparer<int>(), 0)
            .IsLessAcross(0, new SortSpan<int, ComparableComparer<int>, NullContext>(aux.AsSpan(), NullContext.Default, new ComparableComparer<int>(), 1), 0, out var fastLessMain, out var fastLessAux);
        var fastGreater = new SortSpan<int, ComparableComparer<int>, NullContext>(main.AsSpan(), NullContext.Default, new ComparableComparer<int>(), 0)
            .IsGreaterAcross(0, new SortSpan<int, ComparableComparer<int>, NullContext>(aux.AsSpan(), NullContext.Default, new ComparableComparer<int>(), 1), 0, out _, out _);

        var observedContext = new StatisticsContext();
        var observedLess = new SortSpan<int, ComparableComparer<int>, StatisticsContext>(main.AsSpan(), observedContext, new ComparableComparer<int>(), 0)
            .IsLessAcross(0, new SortSpan<int, ComparableComparer<int>, StatisticsContext>(aux.AsSpan(), observedContext, new ComparableComparer<int>(), 1), 0, out var observedLessMain, out var observedLessAux);
        var observedGreater = new SortSpan<int, ComparableComparer<int>, StatisticsContext>(main.AsSpan(), observedContext, new ComparableComparer<int>(), 0)
            .IsGreaterAcross(0, new SortSpan<int, ComparableComparer<int>, StatisticsContext>(aux.AsSpan(), observedContext, new ComparableComparer<int>(), 1), 0, out _, out _);

        await Assert.That(fastLess).IsEqualTo(expectedLess);
        await Assert.That(fastGreater).IsEqualTo(expectedGreater);
        await Assert.That(observedLess).IsEqualTo(expectedLess).Because("the observed path calls the comparer directly and can disagree with the primitive path");
        await Assert.That(observedGreater).IsEqualTo(expectedGreater);
        await Assert.That((fastLessMain, fastLessAux)).IsEqualTo((20, auxValue));
        await Assert.That((observedLessMain, observedLessAux)).IsEqualTo((20, auxValue));
    }

    /// <summary>The strict cross-buffer out overloads must not bypass a custom comparer.</summary>
    [Test]
    public async Task StrictAcross_OutVariants_HonorACustomComparer()
    {
        var main = new[] { 30 };
        var aux = new[] { 20 };

        var ascending = new SortSpan<int, ComparableComparer<int>, NullContext>(main.AsSpan(), NullContext.Default, new ComparableComparer<int>(), 0)
            .IsLessAcross(0, new SortSpan<int, ComparableComparer<int>, NullContext>(aux.AsSpan(), NullContext.Default, new ComparableComparer<int>(), 1), 0, out _, out _);
        var descending = new SortSpan<int, DescendingIntComparer, NullContext>(main.AsSpan(), NullContext.Default, new DescendingIntComparer(), 0)
            .IsLessAcross(0, new SortSpan<int, DescendingIntComparer, NullContext>(aux.AsSpan(), NullContext.Default, new DescendingIntComparer(), 1), 0, out _, out _);

        await Assert.That(ascending).IsFalse().Because("30 < 20 is false");
        await Assert.That(descending).IsTrue()
            .Because("a descending comparer orders 30 below 20; the same result on both would mean the primitive specialization bypassed the comparer");
    }

    /// <summary>
    /// The new overloads must match the read and compare counts of the hand-written form they replace.
    /// </summary>
    [Test]
    public async Task ReadRetainingComparisons_MatchOperationCountsOfTheManualForm()
    {
        var source = new[] { 30, 10, 20, 40 };

        var manual = new StatisticsContext();
        RunManual(source, manual);

        var overloads = new StatisticsContext();
        RunOverloads(source, overloads);

        await Assert.That(overloads.IndexReadCount).IsEqualTo(manual.IndexReadCount);
        await Assert.That(overloads.CompareCount).IsEqualTo(manual.CompareCount);

        // Hand-written: two reads and one compare, then one read and one compare
        static void RunManual(int[] source, StatisticsContext context)
        {
            var s = new SortSpan<int, ComparableComparer<int>, StatisticsContext>(source.AsSpan(), context, new ComparableComparer<int>(), 0);
            var a = s.Read(0);
            var b = s.Read(1);
            _ = s.IsGreaterThan(a, b);
            var c = s.Read(2);
            _ = s.IsLessThan(c, 25);
        }

        static void RunOverloads(int[] source, StatisticsContext context)
        {
            var s = new SortSpan<int, ComparableComparer<int>, StatisticsContext>(source.AsSpan(), context, new ComparableComparer<int>(), 0);
            _ = s.IsGreaterAt(0, 1, out _, out _);
            _ = s.IsElementLessThan(2, 25, out _);
        }
    }

    /// <summary>The out overloads must not bypass a custom comparer (checking the primitive-specialization path).</summary>
    [Test]
    public async Task ReadRetainingComparisons_HonorACustomComparer()
    {
        var source = new[] { 10, 20, 30 };

        var (asc, desc) = Evaluate(source);

        await Assert.That(asc).IsEquivalentTo(new List<bool> { true, true });
        await Assert.That(desc).IsEquivalentTo(new List<bool> { false, false })
            .Because("a descending comparer flips both");

        static (List<bool> Ascending, List<bool> Descending) Evaluate(int[] source)
        {
            var ascending = new List<bool>();
            {
                var s = new SortSpan<int, ComparableComparer<int>, NullContext>(source.AsSpan(), NullContext.Default, new ComparableComparer<int>(), 0);
                ascending.Add(s.IsLessAt(0, 2, out _, out _));              // 10 < 30
                ascending.Add(s.IsElementLessThan(0, 15, out _));           // 10 < 15
            }

            var descending = new List<bool>();
            {
                var s = new SortSpan<int, DescendingIntComparer, NullContext>(source.AsSpan(), NullContext.Default, new DescendingIntComparer(), 0);
                descending.Add(s.IsLessAt(0, 2, out _, out _));
                descending.Add(s.IsElementLessThan(0, 15, out _));
            }

            return (ascending, descending);
        }
    }
}
