using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

/// <summary>
/// Verifies that the integer distribution sorts report the comparisons their range scan actually performs.
///
/// <para>
/// A distribution sort orders without comparing, but none of these overloads is handed its key range: each
/// discovers it with a min/max scan, and that scan compares. Comparisons made on the numeric operators
/// directly, rather than through the observable element accessors, disappear from the stream entirely while
/// reads and writes stay untouched, so the totals remain plausible and nothing about the sorted result
/// changes. <see cref="CountingSortInteger"/> did exactly that while <see cref="PigeonholeSortInteger"/> and
/// <see cref="BucketSortInteger"/> did not, so a consumer comparing the three saw 0 against 2n+1 for the
/// identical scan and would have read the difference as counting sort doing less work.
/// </para>
///
/// <para>
/// The counts are asserted as relations to n rather than as constants. An expectation derived from an
/// implementation that already omits the scan would simply encode the omission, which is what let the gap
/// survive: the per-algorithm theoretical-value tests asserted <c>0</c> and passed.
/// </para>
/// </summary>
public class DistributionRangeScanObservationTests
{
    /// <summary>
    /// The integer overloads that discover their own range. Every one of them scans for min and max before
    /// it can size its auxiliary structure, so every one of them owes the stream those comparisons.
    /// </summary>
    private static readonly (string Name, Action<int[], StatisticsContext> Sort)[] RangeDiscoveringSorts =
    [
        (nameof(CountingSortInteger), static (a, c) => CountingSortInteger.Sort(a.AsSpan(), c)),
        (nameof(PigeonholeSortInteger), static (a, c) => PigeonholeSortInteger.Sort(a.AsSpan(), c)),
        (nameof(BucketSortInteger), static (a, c) => BucketSortInteger.Sort(a.AsSpan(), c)),
    ];

    private static ulong Comparisons(Action<int[], StatisticsContext> sort, int n)
    {
        var stats = new StatisticsContext();
        var array = TestHelpers.ShuffledRange(n, 42);
        sort(array, stats);

        // A comparison count only describes a real run, so confirm the run actually sorted.
        var expected = TestHelpers.ShuffledRange(n, 42);
        Array.Sort(expected);
        if (!array.SequenceEqual(expected))
            throw new InvalidOperationException($"sort produced [{string.Join(", ", array)}]");

        return stats.CompareCount;
    }

    /// <summary>
    /// The scan tests every element against the running min and the running max, so its cost grows with n.
    /// A lower bound of 2(n-1) holds whether the scan seeds its extrema from the type's limits and starts at
    /// index 0, or seeds them from the first element and starts at index 1.
    /// </summary>
    [Test]
    [Arguments(16)]
    [Arguments(64)]
    [Arguments(256)]
    public async Task RangeScanComparisonsScaleWithInputSize(int n)
    {
        foreach (var (name, sort) in RangeDiscoveringSorts)
        {
            var comparisons = Comparisons(sort, n);
            using var _ = Assert.Multiple();
            await Assert.That(comparisons)
                .IsGreaterThanOrEqualTo((ulong)(2 * (n - 1)))
                .Because($"{name} scans for min and max before it can size its auxiliary structure");
        }
    }

    /// <summary>
    /// Counting sort and pigeonhole sort run the same range scan and compare nowhere else — the ordering step
    /// of both is comparison-free — so their totals must agree exactly. Bucket sort is excluded because it
    /// sorts inside each bucket and its total legitimately carries those comparisons too.
    /// </summary>
    [Test]
    [Arguments(16)]
    [Arguments(64)]
    [Arguments(256)]
    public async Task CountingAndPigeonholeReportTheSameScan(int n)
    {
        var counting = Comparisons(static (a, c) => CountingSortInteger.Sort(a.AsSpan(), c), n);
        var pigeonhole = Comparisons(static (a, c) => PigeonholeSortInteger.Sort(a.AsSpan(), c), n);

        await Assert.That(counting)
            .IsEqualTo(pigeonhole)
            .Because("both perform the identical min/max scan and neither compares anywhere else");
    }
}
