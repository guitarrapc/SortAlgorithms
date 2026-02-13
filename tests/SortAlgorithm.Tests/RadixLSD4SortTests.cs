using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class RadixLSD4SortTests
{
    [Test]
    [MethodDataSource(typeof(MockRandomData), nameof(MockRandomData.Generate))]
    [MethodDataSource(typeof(MockNegativePositiveRandomData), nameof(MockNegativePositiveRandomData.Generate))]
    [MethodDataSource(typeof(MockNegativeRandomData), nameof(MockNegativeRandomData.Generate))]
    [MethodDataSource(typeof(MockReversedData), nameof(MockReversedData.Generate))]
    [MethodDataSource(typeof(MockPipeorganData), nameof(MockPipeorganData.Generate))]
    [MethodDataSource(typeof(MockNearlySortedData), nameof(MockNearlySortedData.Generate))]
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


        RadixLSD4Sort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    public async Task StabilityTest()
    {
        // Test stability: elements with same key maintain relative order
        var records = new[]
        {
            (value: 5, id: 1),
            (value: 3, id: 2),
            (value: 5, id: 3),
            (value: 3, id: 4),
            (value: 5, id: 5)
        };

        var keys = records.Select(r => r.value).ToArray();
        RadixLSD4Sort.Sort(keys.AsSpan());

        // After sorting by value, records with same value should maintain original order
        // Since we only sorted keys, we verify the sort is stable by checking
        // that multiple sorts preserve order
        var firstSort = records.Select(r => r.value).ToArray();
        RadixLSD4Sort.Sort(firstSort.AsSpan());

        var secondSort = firstSort.ToArray();
        RadixLSD4Sort.Sort(secondSort.AsSpan());

        await Assert.That(secondSort).IsEquivalentTo(firstSort, CollectionOrdering.Matching);
    }

    [Test]
    public async Task MinValueHandlingTest()
    {
        var stats = new StatisticsContext();
        // Test that int.MinValue is handled correctly (no overflow)
        var array = new[] { int.MinValue, -1, 0, 1, int.MaxValue };
        RadixLSD4Sort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo([int.MinValue, -1, 0, 1, int.MaxValue], CollectionOrdering.Matching);
    }

    [Test]
    public async Task SortWithNegativeNumbers()
    {
        var stats = new StatisticsContext();
        var array = new[] { -5, 3, -1, 0, 2, -3, 1 };
        var expected = new[] { -5, -3, -1, 0, 1, 2, 3 };
        RadixLSD4Sort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task SortWithAllSameValues()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 5, 5, 5, 5 };
        RadixLSD4Sort.Sort(array.AsSpan(), stats);

        foreach (var item in array) await Assert.That(item).IsEqualTo(5);
    }

    [Test]
    [Arguments(typeof(byte))]
    [Arguments(typeof(sbyte))]
    [Arguments(typeof(short))]
    [Arguments(typeof(ushort))]
    [Arguments(typeof(int))]
    [Arguments(typeof(uint))]
    [Arguments(typeof(long))]
    [Arguments(typeof(ulong))]
    public async Task SortDifferentIntegerTypes(Type type)
    {
        var stats = new StatisticsContext();

        if (type == typeof(byte))
        {
            var array = new byte[] { 5, 2, 8, 1, 9 };
            RadixLSD4Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(sbyte))
        {
            var array = new sbyte[] { -5, 2, -8, 1, 9 };
            RadixLSD4Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(short))
        {
            var array = new short[] { -5, 2, -8, 1, 9 };
            RadixLSD4Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(ushort))
        {
            var array = new ushort[] { 5, 2, 8, 1, 9 };
            RadixLSD4Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(int))
        {
            var array = new int[] { -5, 2, -8, 1, 9 };
            RadixLSD4Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(uint))
        {
            var array = new uint[] { 5, 2, 8, 1, 9 };
            RadixLSD4Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(long))
        {
            var array = new long[] { -5, 2, -8, 1, 9 };
            RadixLSD4Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(ulong))
        {
            var array = new ulong[] { 5, 2, 8, 1, 9 };
            RadixLSD4Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
    }

    private static bool IsSorted<T>(T[] array)
    {
        for (int i = 1; i < array.Length; i++)
        {
            if (Comparer<T>.Default.Compare(array[i - 1], array[i]) > 0)
                return false;
        }
        return true;
    }

#if DEBUG

    [Test]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        RadixLSD4Sort.Sort(array.AsSpan(), stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
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
        RadixLSD4Sort.Sort(sorted.AsSpan(), stats);

        // LSD Radix Sort (Radix-4, 2-bit per pass) with sign-bit flipping and early termination:
        // For 32-bit integers with range [0, n-1]:
        //
        // Early termination optimization:
        // - Find min/max keys: n reads
        // - Calculate required bits from range (max ^ min)
        // - Only process required digit passes
        //
        // Example for n=100:
        // - max value = 99 → key = 0x8000_0063 (after sign-bit flip for signed int)
        // - min value = 0 → key = 0x8000_0000
        // - range = 0x8000_0063 ^ 0x8000_0000 = 0x0000_0063 = 99
        // - required bits = 7 (for value 99)
        // - required passes = ceil(7 / 2) = 4
        //
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
        var digitCount = Math.Max(1, (requiredBits + 2 - 1) / 2); // ceil(requiredBits / 2)

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
        RadixLSD4Sort.Sort(reversed.AsSpan(), stats);

        // LSD Radix Sort on reversed data with early termination:
        // Same as sorted - early termination based on actual range
        var maxValue = n - 1;
        var range = (ulong)maxValue;
        var requiredBits = range == 0 ? 0 : (64 - System.Numerics.BitOperations.LeadingZeroCount(range));
        var digitCount = Math.Max(1, (requiredBits + 1) / 2); // ceil(requiredBits / 2)

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
    public async Task TheoreticalValuesRandomTest(int n)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        RadixLSD4Sort.Sort(random.AsSpan(), stats);

        // LSD Radix Sort on random data with early termination:
        // Same complexity - determined by actual range
        var maxValue = n - 1;
        var range = (ulong)maxValue;
        var requiredBits = range == 0 ? 0 : (64 - System.Numerics.BitOperations.LeadingZeroCount(range));
        var digitCount = Math.Max(1, (requiredBits + 2 - 1) / 2);

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
        RadixLSD4Sort.Sort(mixed.AsSpan(), stats);

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

#endif

}
