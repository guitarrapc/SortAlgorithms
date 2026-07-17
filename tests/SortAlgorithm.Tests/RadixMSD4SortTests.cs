using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class RadixMSD4SortTests : IntegerSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => RadixMSD4Sort.Sort(span, context);

    // MSD distribute passes always write on MockSortedData sizes; compares/swaps vary with bucket distribution.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;

    // The KeySelector overload carries satellite data, which makes stability observable:
    // these are the canonical stability tests, driven through Sort(span, keySelector, context).

    [Test]
    [MethodDataSource(typeof(MockStabilityData), nameof(MockStabilityData.Generate))]
    public async Task StabilityTest(StabilityTestItem[] items)
    {
        // Test stability: equal keys should maintain relative order
        var stats = new StatisticsContext();

        RadixMSD4Sort.Sort(items.AsSpan(), x => x.Value, stats);

        // Verify sorting correctness - values should be in ascending order
        await Assert.That(items.Select(x => x.Value).ToArray()).IsEquivalentTo(MockStabilityData.Sorted, CollectionOrdering.Matching);

        // Verify stability: for each group of equal values, original order is preserved
        var value1Indices = items.Where(x => x.Value == 1).Select(x => x.OriginalIndex).ToArray();
        var value2Indices = items.Where(x => x.Value == 2).Select(x => x.OriginalIndex).ToArray();
        var value3Indices = items.Where(x => x.Value == 3).Select(x => x.OriginalIndex).ToArray();

        await Assert.That(value1Indices).IsEquivalentTo(MockStabilityData.Sorted1, CollectionOrdering.Matching);
        await Assert.That(value2Indices).IsEquivalentTo(MockStabilityData.Sorted2, CollectionOrdering.Matching);
        await Assert.That(value3Indices).IsEquivalentTo(MockStabilityData.Sorted3, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockStabilityWithIdData), nameof(MockStabilityWithIdData.Generate))]
    public async Task StabilityTestWithComplex(StabilityTestItemWithId[] items)
    {
        // Test stability with more complex scenario - multiple equal keys
        var stats = new StatisticsContext();

        RadixMSD4Sort.Sort(items.AsSpan(), x => x.Key, stats);

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
        // All keys equal: original order must be fully preserved
        var stats = new StatisticsContext();

        RadixMSD4Sort.Sort(items.AsSpan(), x => x.Value, stats);

        foreach (var item in items) await Assert.That(item.Value).IsEqualTo(1);
        await Assert.That(items.Select(x => x.OriginalIndex).ToArray()).IsEquivalentTo(MockStabilityAllEqualsData.Sorted, CollectionOrdering.Matching);
    }

    [Test]
    public async Task KeySelectorNegativeKeysTest()
    {
        // Keys spanning negative/zero/positive, ordered strictly by key
        var records = new (int Key, string Name)[] { (3, "c"), (-5, "a"), (0, "b"), (-5, "a2"), (3, "c2"), (int.MinValue, "min"), (int.MaxValue, "max") };
        RadixMSD4Sort.Sort(records.AsSpan(), x => x.Key);

        await Assert.That(records.Select(x => x.Key).ToArray())
            .IsEquivalentTo([int.MinValue, -5, -5, 0, 3, 3, int.MaxValue], CollectionOrdering.Matching);
        // Equal keys keep input order (stability)
        await Assert.That(records.Select(x => x.Name).ToArray())
            .IsEquivalentTo(["min", "a", "a2", "b", "c", "c2", "max"], CollectionOrdering.Matching);
    }

    [Test]
    public async Task KeySelectorLargeInputTest()
    {
        // Exceed the insertion-sort cutoff so the MSD digit passes actually run
        var random = new Random(42);
        var records = Enumerable.Range(0, 1000).Select(i => (Key: random.Next(-10000, 10000), Index: i)).ToArray();
        var expected = records.OrderBy(x => x.Key).ThenBy(x => x.Index).ToArray();

        RadixMSD4Sort.Sort(records.AsSpan(), x => x.Key);

        // OrderBy+ThenBy(Index) is exactly what a stable key sort must produce
        await Assert.That(records).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task DecimalDigitBoundaryTest()
    {
        // Test values that cross decimal digit boundaries (9→10, 99→100, etc.)
        var array = new[] { 100, 9, 99, 10, 1, 999, 1000 };
        var expected = new[] { 1, 9, 10, 99, 100, 999, 1000 };
        RadixMSD4Sort.Sort(array.AsSpan());
        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task MSD10SpecificTest()
    {
        // Test specifically for decimal (base-10) radix characteristics
        var array = new[] { 123, 456, 789, 12, 45, 78, 1, 4, 7 };
        var expected = new[] { 1, 4, 7, 12, 45, 78, 123, 456, 789 };
        RadixMSD4Sort.Sort(array.AsSpan());
        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task InsertionSortCutoffTest()
    {
        // Test with array smaller than insertion sort cutoff (16)
        var array = new[] { 10, 5, 3, 8, 1, 9, 2, 7, 4, 6 };
        var expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        RadixMSD4Sort.Sort(array.AsSpan());
        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
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
        RadixMSD4Sort.Sort(sorted.AsSpan(), stats);

        // MSD Radix Sort (Radix-4, 2-bit per pass) with sign-bit flipping and early termination:
        // For 32-bit integers with range [0, n-1]:
        //
        // MSD processes from most significant digit, recursively partitioning buckets.
        // For sorted input, elements tend to distribute into buckets naturally,
        // and small buckets (<=16 elements) switch to insertion sort.
        //
        // Initial scan: n reads (to find min/max)
        // MSD pass complexity varies based on bucket distribution and insertion sort cutoff.
        //
        // For sorted data:
        // - Elements may cluster in certain buckets
        // - Small buckets use insertion sort (includes comparisons and swaps)
        // - Larger buckets continue with MSD partitioning
        //
        // We verify that the sort completes successfully with non-zero statistics.
        await Assert.That((ulong)sorted.Length).IsEqualTo((ulong)n);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);

        // IndexWriteCount: For small n (<=16), entire array uses insertion sort, so IndexWriteCount may be 0
        // For larger n (>16), MSD partitioning occurs, so IndexWriteCount > 0
        if (n > 16)
        {
            await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
        }

        // CompareCount and SwapCount: Always occur due to insertion sort (for buckets <=16)
        // For small n, entire array uses insertion sort
        // For large n, at least some buckets use insertion sort
        // Values are data-dependent, so we only verify they are recorded (>= 0)
        // No specific assertions as values vary with bucket distribution
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
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
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        RadixMSD4Sort.Sort(reversed.AsSpan(), stats);

        // MSD Radix Sort on reversed data:
        // Similar to sorted data, but distribution pattern is reversed.
        // Small buckets (<=16 elements) still use insertion sort.
        // Reversed data in insertion sort leads to more comparisons and swaps.
        await Assert.That((ulong)reversed.Length).IsEqualTo((ulong)n);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);

        // IndexWriteCount: For n > 16, MSD partitioning occurs
        if (n > 16)
        {
            await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
        }

        // CompareCount and SwapCount are non-zero due to insertion sort
        // Reversed data typically causes more swaps in insertion sort
        // Exact values depend on bucket distribution and are data-dependent
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesRandomTest(int n)
    {
        var stats = new StatisticsContext();
        var random = new Random(42);
        var array = Enumerable.Range(0, n).OrderBy(_ => random.Next()).ToArray();
        RadixMSD4Sort.Sort(array.AsSpan(), stats);

        // MSD Radix Sort on random data:
        // Random distribution tends to spread elements across buckets more evenly.
        // More buckets will exceed the insertion sort cutoff (16 elements), requiring more MSD passes.
        // Eventually small buckets are sorted via insertion sort.
        await Assert.That((ulong)array.Length).IsEqualTo((ulong)n);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);

        // IndexWriteCount: For n > 16, MSD partitioning occurs
        if (n > 16)
        {
            await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
        }

        // CompareCount and SwapCount from insertion sort in small buckets
        // Random data means varied bucket sizes, so statistics vary by run
        // Exact values are data-dependent and vary with bucket distribution
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesNegativeTest(int n)
    {
        var stats = new StatisticsContext();
        // Mix of negative and positive: [-n/2, ..., -1, 0, 1, ..., n/2-1]
        var mixed = Enumerable.Range(-n / 2, n).ToArray();
        RadixMSD4Sort.Sort(mixed.AsSpan(), stats);

        // MSD Radix Sort on mixed negative/positive data:
        // With sign-bit flipping, negative and positive numbers are processed uniformly.
        // Data distribution across buckets depends on the value range.
        // Small buckets (<=16 elements) use insertion sort with comparisons and swaps.
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);

        // IndexWriteCount: For n > 16, MSD partitioning occurs
        if (n > 16)
        {
            await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
        }

        // CompareCount and SwapCount from insertion sort in small buckets
        // Values depend on how data distributes across decimal digit buckets
        // Exact values are data-dependent and vary with bucket distribution
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }
}
