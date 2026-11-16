using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class PoplarHeapSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => PoplarHeapSort.Sort(span, context);

    // A sorted input is already a poplar heap and every extraction finds the maximum at the last root,
    // so nothing moves: no swaps and no writes.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        PoplarHeapSort.Sort(array.AsSpan(), 2, 6, stats);

        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        PoplarHeapSort.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortSingleElementTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9 };

        PoplarHeapSort.Sort(array.AsSpan(), 2, 3, stats);

        await Assert.That(array).IsEquivalentTo([5, 3, 8, 1, 9], CollectionOrdering.Matching);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);
    }

    [Test]
    public async Task RangeSortBeginningTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 9, 7, 5, 3, 1, 2, 4, 6, 8 };

        PoplarHeapSort.Sort(array.AsSpan(), 0, 5, stats);

        await Assert.That(array).IsEquivalentTo([1, 3, 5, 7, 9, 2, 4, 6, 8], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortEndTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 1, 3, 5, 7, 9, 8, 6, 4, 2 };

        PoplarHeapSort.Sort(array.AsSpan(), 5, 9, stats);

        await Assert.That(array).IsEquivalentTo([1, 3, 5, 7, 9, 2, 4, 6, 8], CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(-1, 3)]
    [Arguments(0, 10)]
    [Arguments(4, 3)]
    public async Task RangeSortRejectsInvalidBounds(int first, int last)
    {
        var array = new[] { 3, 1, 2, 5, 4 };

        await Assert.That(() => PoplarHeapSort.Sort(array.AsSpan(), first, last, NullContext.Default))
            .Throws<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// Every size from 0 to 1100 in turn: the build loop's stopping rule depends on how n splits into poplars of
    /// size 2^k - 1, so the tail cases are distinct per n. n = 13 is the first size where the reference
    /// implementation's <c>poplar_size &gt; size</c> guard would sift a root past the end with single-element base
    /// poplars, and n = 125 is the first with its 15-element base. Debug builds bounds-check every access through
    /// <c>SortSpan</c>, so an overrun fails here rather than reading adjacent memory.
    /// </summary>
    [Test]
    [Arguments(0)]
    [Arguments(1)]
    [Arguments(2)]
    public async Task EverySizeSortsCorrectly(int kind)
    {
        for (var n = 0; n <= 1100; n++)
        {
            var random = new Random(n);
            var array = kind switch
            {
                0 => Enumerable.Range(0, n).Select(_ => random.Next()).ToArray(),
                1 => Enumerable.Range(0, n).Reverse().ToArray(),
                _ => Enumerable.Range(0, n).Select(_ => random.Next(3)).ToArray(),
            };
            var expected = array.ToArray();
            Array.Sort(expected);

            var stats = new StatisticsContext();
            PoplarHeapSort.Sort(array.AsSpan(), stats);

            await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"n={n}, kind={kind}");
        }
    }

    [Test]
    [Arguments(13)]
    [Arguments(125)]
    public async Task ReferenceGuardOverrunSizesSortCorrectly(int n)
    {
        foreach (var seed in new[] { 1, 42, 1234 })
        {
            var array = TestHelpers.ShuffledRange(n, seed);
            var optimized = array.ToArray();

            PoplarHeapSort.Sort(array.AsSpan(), new StatisticsContext());
            PoplarHeapSort.Sort(optimized.AsSpan());

            await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
            await Assert.That(optimized).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
        }
    }

    [Test]
    public async Task CustomComparerSortsDescending()
    {
        var array = TestHelpers.ShuffledRange(200, 42);
        var descending = Comparer<int>.Create((a, b) => b.CompareTo(a));

        PoplarHeapSort.Sort(array.AsSpan(), descending, new StatisticsContext());

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, 200).Reverse().ToArray(), CollectionOrdering.Matching);
    }

    /// <summary>
    /// Pins the reordering that <c>IsStable => false</c> declares.
    /// Building [1a, 0b, 0c] sifts the 3-element semipoplar: its root 0c is smaller than the left subpoplar root 1a,
    /// so the two are exchanged and 0c lands at index 0, in front of 0b. The array is then already sorted by key,
    /// and extraction moves nothing.
    /// </summary>
    [Test]
    public async Task SiftCarriesEqualKeyPastItsTwin()
    {
        var items = new[]
        {
            new StabilityTestItem(1, 0),
            new StabilityTestItem(0, 1),
            new StabilityTestItem(0, 2),
        };

        PoplarHeapSort.Sort(items.AsSpan(), new StatisticsContext());

        await Assert.That(items.Select(x => x.Value).ToArray()).IsEquivalentTo([0, 0, 1], CollectionOrdering.Matching);
        await Assert.That(items.Select(x => x.OriginalIndex).ToArray()).IsEquivalentTo([2, 1, 0], CollectionOrdering.Matching);
    }

    /// <summary>
    /// Elements move only by swapping, and every comparison and swap announces its reads through SortSpan,
    /// so the four counters are tied exactly. This is the check behind the Index Reads and Index Writes rows.
    /// </summary>
    [Test]
    [Arguments(100, 42)]
    [Arguments(1000, 1234)]
    public async Task ReadsAndWritesFollowComparisonsAndSwaps(int n, int seed)
    {
        var stats = new StatisticsContext();
        var random = TestHelpers.ShuffledRange(n, seed);
        PoplarHeapSort.Sort(random.AsSpan(), stats);

        await Assert.That(stats.IndexWriteCount).IsEqualTo(2 * stats.SwapCount);
        await Assert.That(stats.IndexReadCount).IsEqualTo(2 * stats.CompareCount + 2 * stats.SwapCount);
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    [Arguments(1000)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        PoplarHeapSort.Sort(sorted.AsSpan(), stats);

        // Nothing moves, but the extraction phase still compares against each poplar root,
        // so the count grows faster than n: this is not smoothsort's linear best case.
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsGreaterThanOrEqualTo((ulong)(n - 1));
        await Assert.That(stats.CompareCount).IsLessThanOrEqualTo((ulong)(2 * n * Math.Log2(n)));
    }

    /// <summary>
    /// The sorted-input comparison count is Θ(n log n): per element it rises with log n instead of staying flat.
    /// </summary>
    [Test]
    public async Task SortedInputComparisonsGrowFasterThanLinear()
    {
        static double PerElement(int n)
        {
            var stats = new StatisticsContext();
            var sorted = Enumerable.Range(0, n).ToArray();
            PoplarHeapSort.Sort(sorted.AsSpan(), stats);
            return (double)stats.CompareCount / n;
        }

        var small = PerElement(1 << 8);
        var large = PerElement(1 << 14);

        await Assert.That(large).IsGreaterThan(small * 1.5)
            .Because($"comparisons per element: n=256 -> {small:F2}, n=16384 -> {large:F2}");
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
        PoplarHeapSort.Sort(reversed.AsSpan(), stats);

        var maxCompares = (ulong)(n * Math.Log2(n) * 3);

        await Assert.That(stats.CompareCount).IsBetween((ulong)(n - 1), maxCompares);
        await Assert.That(stats.SwapCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(2 * stats.SwapCount);
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
        PoplarHeapSort.Sort(random.AsSpan(), stats);

        var maxCompares = (ulong)(n * Math.Log2(n) * 3);

        await Assert.That(stats.CompareCount).IsBetween((ulong)(n - 1), maxCompares);
        await Assert.That(random).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
    }
}
