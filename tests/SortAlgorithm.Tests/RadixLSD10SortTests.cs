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
        // 2. For each digit d (from 0 to digitCount-1):
        //    - Count phase: n reads
        //    - Distribute phase: n reads + n writes (to temp buffer)
        //    - Copy back phase (using CopyTo): n reads (from temp buffer) + n writes (to main buffer)
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

        var expectedReads = (ulong)(n + digitCount * 3 * n); // Find min/max + (count + distribute + CopyTo) per digit
        var expectedWrites = (ulong)(digitCount * 2 * n); // (distribute + CopyTo) writes per digit

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

        var expectedReads = (ulong)(n + digitCount * 3 * n); // Find min/max + (count + distribute + CopyTo)
        var expectedWrites = (ulong)(digitCount * 2 * n); // (distribute + CopyTo) writes

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

        var expectedReads = (ulong)(n + digitCount * 3 * n); // Find min/max + (count + distribute + CopyTo)
        var expectedWrites = (ulong)(digitCount * 2 * n); // (distribute + CopyTo) writes

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
        // 2. For each digit d (from 0 to digitCount-1):
        //    - Count phase: n reads
        //    - Distribute phase: n reads + n writes (to temp buffer)
        //    - Copy back phase: n reads + n writes
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

        var expectedReads = (ulong)(n + digitCount * 3 * n); // Find min/max + (count + distribute + CopyTo) per digit
        var expectedWrites = (ulong)(digitCount * 2 * n); // (distribute + CopyTo) writes per digit

        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL); // Still non-comparison
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        // Verify result is sorted
        await Assert.That(mixed).IsEquivalentTo(mixed.OrderBy(x => x), CollectionOrdering.Matching);
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
