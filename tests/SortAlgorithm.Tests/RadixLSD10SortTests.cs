using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class RadixLSD10SortTests : IntegerSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => RadixLSD10Sort.Sort(span, context);

    // Distribution sort: no comparisons or swaps; distribute/copy-back passes always write.
    protected override CountExpectation SortedInputCompares => CountExpectation.Zero;
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    // The KeySelector overload carries satellite data, which makes stability observable:
    // these are the canonical stability tests, driven through SortBy(span, keySelector, context).

    [Test]
    [MethodDataSource(typeof(MockStabilityData), nameof(MockStabilityData.Generate))]
    public async Task StabilityTest(StabilityTestItem[] items)
    {
        // Test stability: equal keys should maintain relative order
        var stats = new StatisticsContext();

        RadixLSD10Sort.SortBy(items.AsSpan(), x => x.Value, stats);

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

        RadixLSD10Sort.SortBy(items.AsSpan(), x => x.Key, stats);

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

        RadixLSD10Sort.SortBy(items.AsSpan(), x => x.Value, stats);

        foreach (var item in items) await Assert.That(item.Value).IsEqualTo(1);
        await Assert.That(items.Select(x => x.OriginalIndex).ToArray()).IsEquivalentTo(MockStabilityAllEqualsData.Sorted, CollectionOrdering.Matching);
    }

    [Test]
    public async Task KeySelectorNegativeKeysTest()
    {
        // Keys spanning negative/zero/positive, ordered strictly by key
        var records = new (int Key, string Name)[] { (3, "c"), (-5, "a"), (0, "b"), (-5, "a2"), (3, "c2"), (int.MinValue, "min"), (int.MaxValue, "max") };
        RadixLSD10Sort.SortBy(records.AsSpan(), x => x.Key);

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

        RadixLSD10Sort.SortBy(records.AsSpan(), x => x.Key);

        // OrderBy+ThenBy(Index) is exactly what a stable key sort must produce
        await Assert.That(records).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateHalf))]
    public async Task SortHalfResultOrderTest(IInputSample<Half> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        RadixLSD10Sort.Sort(array.AsSpan(), stats);

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

        RadixLSD10Sort.Sort(array.AsSpan(), stats);

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

        RadixLSD10Sort.Sort(array.AsSpan(), stats);

        // Check is sorted (NaN-first total order, same as Array.Sort)
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    public async Task SortNativeIntegerTypesTest()
    {
        // nint/nuint coverage carried over from the old SortDifferentIntegerTypes
        // (the base class covers fixed-width integer types only).
        var stats = new StatisticsContext();
        var signed = new nint[] { -5, 2, -8, 1, 9 };
        Sort(signed.AsSpan(), stats);
        await Assert.That(IsSorted(signed)).IsTrue();

        var unsigned = new nuint[] { 5, 2, 8, 1, 9 };
        Sort(unsigned.AsSpan(), stats);
        await Assert.That(IsSorted(unsigned)).IsTrue();
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
        RadixLSD10Sort.Sort(sorted.AsSpan(), stats);

        // LSD Radix Sort with range-based optimization:
        // 1. Find min/max keys: n reads
        // 2. Build every digit's histogram in one pass: n reads
        // 3. For each digit d (from 0 to digitCount-1):
        //    - No count phase: its histogram already exists
        //    - Distribute phase: n reads + n writes (to the destination buffer)
        //    - No copy back: src and dst trade roles (ping-pong)
        // 4. If digitCount is odd, one final copy from the temp buffer: n reads + n writes
        //
        // For n elements with values [0, n-1]:
        // - min unsigned key = 0x8000_0000 + 0 (for non-negative 0)
        // - max unsigned key = 0x8000_0000 + (n-1) (for non-negative n-1)
        // - range = maxKey - minKey = (n-1)
        // - digitCount = number of decimal digits needed to represent range
        //
        // For example, n=100 → range = 99 → 3 digits (not 10!)
        //
        // Total reads = n (find min/max) + digitCount × 3n (count + distribute + CopyTo read)
        // Total writes = digitCount × 2n (distribute write + CopyTo write)
        var minValue = (uint)0;
        var maxValue = (uint)(n - 1);
        var minKey = minValue ^ 0x8000_0000; // Sign-bit flip for non-negative
        var maxKey = maxValue ^ 0x8000_0000; // Sign-bit flip for non-negative
        var range = maxKey - minKey; // range = n - 1
        var digitCount = GetDigitCountFromUlong(range);

        // Every digit's histogram is built in one preprocessing pass, so a digit pass reads each
        // element once (to distribute it) and writes it once. The buffers trade roles instead of
        // being copied back, so an odd pass count leaves the result in the temp buffer and costs
        // one final copy.
        var finalCopy = digitCount % 2 == 1 ? n : 0;
        var expectedReads = (ulong)(2 * n + digitCount * n + finalCopy); // min/max scan + histogram pass + distribute per digit + final copy
        var expectedWrites = (ulong)(digitCount * n + finalCopy);        // distribute per digit + final copy

        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL); // Non-comparison sort
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
        RadixLSD10Sort.Sort(reversed.AsSpan(), stats);

        // LSD Radix Sort with range-based optimization:
        // Same as sorted - performance is data-independent O(d × n)
        // Range is still [0, n-1], so digitCount is based on range = n - 1
        var minValue = (uint)0;
        var maxValue = (uint)(n - 1);
        var minKey = minValue ^ 0x8000_0000; // Sign-bit flip for non-negative
        var maxKey = maxValue ^ 0x8000_0000; // Sign-bit flip for non-negative
        var range = maxKey - minKey; // range = n - 1
        var digitCount = GetDigitCountFromUlong(range);

        // Every digit's histogram is built in one preprocessing pass, so a digit pass reads each
        // element once (to distribute it) and writes it once. The buffers trade roles instead of
        // being copied back, so an odd pass count leaves the result in the temp buffer and costs
        // one final copy.
        var finalCopy = digitCount % 2 == 1 ? n : 0;
        var expectedReads = (ulong)(2 * n + digitCount * n + finalCopy); // min/max scan + histogram pass + distribute per digit + final copy
        var expectedWrites = (ulong)(digitCount * n + finalCopy);        // distribute per digit + final copy

        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL); // Non-comparison sort
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
        RadixLSD10Sort.Sort(random.AsSpan(), stats);

        // LSD Radix Sort with range-based optimization:
        // Same complexity as sorted/reversed - O(d × n)
        // Range is [0, n-1], so digitCount is based on range = n - 1
        var minValue = (uint)0;
        var maxValue = (uint)(n - 1);
        var minKey = minValue ^ 0x8000_0000; // Sign-bit flip for non-negative
        var maxKey = maxValue ^ 0x8000_0000; // Sign-bit flip for non-negative
        var range = maxKey - minKey; // range = n - 1
        var digitCount = GetDigitCountFromUlong(range);

        // Every digit's histogram is built in one preprocessing pass, so a digit pass reads each
        // element once (to distribute it) and writes it once. The buffers trade roles instead of
        // being copied back, so an odd pass count leaves the result in the temp buffer and costs
        // one final copy.
        var finalCopy = digitCount % 2 == 1 ? n : 0;
        var expectedReads = (ulong)(2 * n + digitCount * n + finalCopy); // min/max scan + histogram pass + distribute per digit + final copy
        var expectedWrites = (ulong)(digitCount * n + finalCopy);        // distribute per digit + final copy

        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL); // Non-comparison sort
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
        RadixLSD10Sort.Sort(mixed.AsSpan(), stats);

        // With range-based optimization and sign-bit flipping:
        // 1. Find min/max keys: n reads
        // 2. Build every digit's histogram in one pass: n reads
        // 3. For each digit d (from 0 to digitCount-1):
        //    - No count phase: its histogram already exists
        //    - Distribute phase: n reads + n writes (to the destination buffer)
        //    - No copy back: src and dst trade roles (ping-pong)
        // 4. If digitCount is odd, one final copy from the temp buffer: n reads + n writes
        //
        // For input [-n/2, ..., -1, 0, 1, ..., n/2-1]:
        // - Min value: -n/2 → min key = 0x80000000 - n/2
        // - Max value: n/2-1 → max key = 0x80000000 + (n/2-1)
        // - Range = maxKey - minKey = (n/2-1) - (-n/2) = n - 1
        // - digitCount is based on range, not maxKey
        var minValue = -n / 2;
        var maxValue = n / 2 - 1;
        var minKey = (uint)minValue ^ 0x8000_0000; // Sign-bit flip
        var maxKey = (uint)maxValue ^ 0x8000_0000; // Sign-bit flip
        var range = maxKey - minKey; // range = n - 1
        var digitCount = GetDigitCountFromUlong(range);

        // Every digit's histogram is built in one preprocessing pass, so a digit pass reads each
        // element once (to distribute it) and writes it once. The buffers trade roles instead of
        // being copied back, so an odd pass count leaves the result in the temp buffer and costs
        // one final copy.
        var finalCopy = digitCount % 2 == 1 ? n : 0;
        var expectedReads = (ulong)(2 * n + digitCount * n + finalCopy); // min/max scan + histogram pass + distribute per digit + final copy
        var expectedWrites = (ulong)(digitCount * n + finalCopy);        // distribute per digit + final copy

        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL); // Still non-comparison
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        // Verify result is sorted
        await Assert.That(mixed).IsEquivalentTo(mixed.OrderBy(x => x), CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(100, 42, 10)]         // range 990        -> 3 decimal digits (odd: one final copy back)
    [Arguments(100, 42, 1_000)]      // range 99,000     -> 5 decimal digits (odd)
    [Arguments(100, 42, 1_000_000)]  // range 99,000,000 -> 8 decimal digits (even: result ends in the main buffer)
    public async Task TheoreticalValuesWideRangeTest(int n, int seed, int stride)
    {
        var stats = new StatisticsContext();
        var values = TestHelpers.ShuffledRangeScaled(n, seed, stride);
        var expected = values.OrderBy(x => x).ToArray();
        RadixLSD10Sort.Sort(values.AsSpan(), stats);

        // The other statistics tests here use a dense [0, n) range, which caps the pass count at 2.
        // Scaling the values widens the key range without changing n, so the pass count is what varies
        // here, and the per-pass cost and the ping-pong parity dominate the totals.
        var range = (ulong)((n - 1) * stride);
        var digitCount = GetDigitCountFromUlong(range);
        await Assert.That(digitCount).IsGreaterThanOrEqualTo(2); // guard: this test is pointless at 1 pass

        // Only the digits the values disagree on are distributed; a digit they all share is an identity
        // pass, already visible in the histograms before any pass runs. Every value here is a multiple of
        // the stride, so its low digits are all zero and those passes are skipped outright — this input
        // is the case the optimisation exists for.
        var executed = TestHelpers.CountNonIdentityDigits(values: expected, digitCount, radix: 10);
        await Assert.That(executed).IsLessThan(digitCount); // multiples of a power of ten always have identity digits

        var finalCopy = executed % 2 == 1 ? n : 0;
        var expectedReads = (ulong)(2 * n + executed * n + finalCopy);
        var expectedWrites = (ulong)(executed * n + finalCopy);

        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        await Assert.That(values).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    /// <summary>
    /// Helper to calculate digit count from unsigned long value (for sign-flipped keys)
    /// </summary>
    private static int GetDigitCountFromUlong(ulong value)
    {
        if (value == 0) return 1;

        var count = 0;
        while (value > 0)
        {
            value /= 10;
            count++;
        }
        return count;
    }
}
