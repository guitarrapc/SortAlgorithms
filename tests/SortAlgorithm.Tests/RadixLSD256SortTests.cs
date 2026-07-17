using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class RadixLSD256SortTests : IntegerSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => RadixLSD256Sort.Sort(span, context);

    // Distribution sort: no comparisons or swaps; distribute/copy-back passes always write.
    protected override CountExpectation SortedInputCompares => CountExpectation.Zero;
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    // The KeySelector overload carries satellite data, which makes stability observable:
    // these are the canonical stability tests, driven through Sort(span, keySelector, context).

    [Test]
    [MethodDataSource(typeof(MockStabilityData), nameof(MockStabilityData.Generate))]
    public async Task StabilityTest(StabilityTestItem[] items)
    {
        // Test stability: equal keys should maintain relative order
        var stats = new StatisticsContext();

        RadixLSD256Sort.SortBy(items.AsSpan(), x => x.Value, stats);

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

        RadixLSD256Sort.SortBy(items.AsSpan(), x => x.Key, stats);

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

        RadixLSD256Sort.SortBy(items.AsSpan(), x => x.Value, stats);

        foreach (var item in items) await Assert.That(item.Value).IsEqualTo(1);
        await Assert.That(items.Select(x => x.OriginalIndex).ToArray()).IsEquivalentTo(MockStabilityAllEqualsData.Sorted, CollectionOrdering.Matching);
    }

    [Test]
    public async Task KeySelectorNegativeKeysTest()
    {
        // Keys spanning negative/zero/positive, ordered strictly by key
        var records = new (int Key, string Name)[] { (3, "c"), (-5, "a"), (0, "b"), (-5, "a2"), (3, "c2"), (int.MinValue, "min"), (int.MaxValue, "max") };
        RadixLSD256Sort.SortBy(records.AsSpan(), x => x.Key);

        await Assert.That(records.Select(x => x.Key).ToArray())
            .IsEquivalentTo([int.MinValue, -5, -5, 0, 3, 3, int.MaxValue], CollectionOrdering.Matching);
        // Equal keys keep input order (stability)
        await Assert.That(records.Select(x => x.Name).ToArray())
            .IsEquivalentTo(["min", "a", "a2", "b", "c", "c2", "max"], CollectionOrdering.Matching);
    }

    [Test]
    public async Task KeySelectorLargeInputTest()
    {
        // Large input exercises the full LSD digit passes
        var random = new Random(42);
        var records = Enumerable.Range(0, 1000).Select(i => (Key: random.Next(-10000, 10000), Index: i)).ToArray();
        var expected = records.OrderBy(x => x.Key).ThenBy(x => x.Index).ToArray();

        RadixLSD256Sort.SortBy(records.AsSpan(), x => x.Key);

        // OrderBy+ThenBy(Index) is exactly what a stable key sort must produce
        await Assert.That(records).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateHalf))]
    public async Task SortHalfResultOrderTest(IInputSample<Half> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        RadixLSD256Sort.Sort(array.AsSpan(), stats);

        // Check is sorted (NaN-first total order, same as Array.Sort)
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateFloat))]
    public async Task SortFloatResultOrderTest(IInputSample<float> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        RadixLSD256Sort.Sort(array.AsSpan(), stats);

        // Check is sorted (NaN-first total order, same as Array.Sort)
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateDouble))]
    public async Task SortDoubleResultOrderTest(IInputSample<double> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        RadixLSD256Sort.Sort(array.AsSpan(), stats);

        // Check is sorted (NaN-first total order, same as Array.Sort)
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
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
        RadixLSD256Sort.Sort(sorted.AsSpan(), stats);

        // LSD Radix Sort (Radix-256) with sign-bit flipping:
        // For 32-bit integers: digitCount = 4 (4 bytes)
        //
        // Unified processing for all values (no separate negative/positive paths):
        // Per pass d (d=0,1,2,3):
        //   - Count phase: n reads
        //   - Distribute phase: n reads + n writes (to temp)
        //   - Copy back phase: n reads (from temp) + n writes (to main)
        //
        // Total per pass:
        // - Reads: n (count) + n (distribute) + n (copy back) = 3n
        // - Writes: n (distribute to temp) + n (copy back to main) = 2n
        //
        // Total:
        // - Initial scan: n reads
        // - Radix passes: digitCount × (3n reads + 2n writes)
        var maxValue = n - 1;
        var range = (ulong)maxValue; // min=0 after sign-bit flip, range = max - min
        var requiredBits = range == 0 ? 0 : (64 - System.Numerics.BitOperations.LeadingZeroCount(range));
        var digitCount = Math.Max(1, (requiredBits + 7) / 8); // ceil(requiredBits / 8)

        var expectedReads = (ulong)(n + digitCount * 3 * n);  // Initial + (count + distribute + copy) × passes
        var expectedWrites = (ulong)(digitCount * 2 * n);     // (temp write + main write) × passes

        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
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
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        RadixLSD256Sort.Sort(reversed.AsSpan(), stats);

        // LSD Radix Sort on reversed data with early termination:
        // Same as sorted - early termination based on actual range
        var maxValue = n - 1;
        var range = (ulong)maxValue;
        var requiredBits = range == 0 ? 0 : (64 - System.Numerics.BitOperations.LeadingZeroCount(range));
        var digitCount = Math.Max(1, (requiredBits + 7) / 8);

        var expectedReads = (ulong)(n + digitCount * 3 * n);
        var expectedWrites = (ulong)(digitCount * 2 * n);

        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);
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
        var random = TestHelpers.ShuffledRange(n, seed);
        RadixLSD256Sort.Sort(random.AsSpan(), stats);

        // LSD Radix Sort on random data with early termination:
        // Same complexity - determined by actual range
        var maxValue = n - 1;
        var range = (ulong)maxValue;
        var requiredBits = range == 0 ? 0 : (64 - System.Numerics.BitOperations.LeadingZeroCount(range));
        var digitCount = Math.Max(1, (requiredBits + 7) / 8);

        var expectedReads = (ulong)(n + digitCount * 3 * n);
        var expectedWrites = (ulong)(digitCount * 2 * n);

        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);
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
        // Mix of negative and non-negative: [-n/2, ..., -1, 0, 1, ..., n/2-1]
        var mixed = Enumerable.Range(-n / 2, n).ToArray();
        RadixLSD256Sort.Sort(mixed.AsSpan(), stats);

        // With sign-bit flipping and early termination:
        // For mixed negative/positive data, verify the sort is correct
        // The actual pass count depends on the range after sign-bit flipping

        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        // Verify result is sorted
        await Assert.That(mixed).IsEquivalentTo(mixed.OrderBy(x => x), CollectionOrdering.Matching);
    }
}
