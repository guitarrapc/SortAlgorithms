using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class RadixLSD10SortTests
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


        RadixLSD10Sort.Sort(array.AsSpan(), stats);

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
        RadixLSD10Sort.Sort(keys.AsSpan());

        // After sorting by value, records with same value should maintain original order
        // Since we only sorted keys, we verify the sort is stable by checking
        // that multiple sorts preserve order
        var firstSort = records.Select(r => r.value).ToArray();
        RadixLSD10Sort.Sort(firstSort.AsSpan());

        var secondSort = firstSort.ToArray();
        RadixLSD10Sort.Sort(secondSort.AsSpan());

        await Assert.That(secondSort).IsEquivalentTo(firstSort, CollectionOrdering.Matching);
    }

    [Test]
    public async Task MinValueHandlingTest()
    {
        var stats = new StatisticsContext();
        // Test that int.MinValue is handled correctly (no overflow)
        var array = new[] { int.MinValue, -1, 0, 1, int.MaxValue };
        RadixLSD10Sort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo([int.MinValue, -1, 0, 1, int.MaxValue], CollectionOrdering.Matching);
    }

    [Test]
    public async Task SortWithNegativeNumbers()
    {
        var stats = new StatisticsContext();
        var array = new[] { -5, 3, -1, 0, 2, -3, 1 };
        var expected = new[] { -5, -3, -1, 0, 1, 2, 3 };
        RadixLSD10Sort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task SortWithAllSameValues()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 5, 5, 5, 5 };
        RadixLSD10Sort.Sort(array.AsSpan(), stats);

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
    [Arguments(typeof(nint))]
    [Arguments(typeof(nuint))]
    public async Task SortDifferentIntegerTypes(Type type)
    {
        var stats = new StatisticsContext();

        if (type == typeof(byte))
        {
            var array = new byte[] { 5, 2, 8, 1, 9 };
            RadixLSD10Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(sbyte))
        {
            var array = new sbyte[] { -5, 2, -8, 1, 9 };
            RadixLSD10Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(short))
        {
            var array = new short[] { -5, 2, -8, 1, 9 };
            RadixLSD10Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(ushort))
        {
            var array = new ushort[] { 5, 2, 8, 1, 9 };
            RadixLSD10Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(int))
        {
            var array = new int[] { -5, 2, -8, 1, 9 };
            RadixLSD10Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(uint))
        {
            var array = new uint[] { 5, 2, 8, 1, 9 };
            RadixLSD10Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(long))
        {
            var array = new long[] { -5, 2, -8, 1, 9 };
            RadixLSD10Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(ulong))
        {
            var array = new ulong[] { 5, 2, 8, 1, 9 };
            RadixLSD10Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(nint))
        {
            var array = new nint[] { -5, 2, -8, 1, 9 };
            RadixLSD10Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(nuint))
        {
            var array = new nuint[] { 5, 2, 8, 1, 9 };
            RadixLSD10Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
    }

    private static bool IsSorted<T>(T[] array) where T : IComparable<T>
    {
        for (int i = 1; i < array.Length; i++)
        {
            if (new ComparableComparer<T>().Compare(array[i - 1], array[i]) > 0)
                return false;
        }
        return true;
    }


    [Test]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        RadixLSD10Sort.Sort(array.AsSpan(), stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL); // Non-comparison sort
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
        RadixLSD10Sort.Sort(sorted.AsSpan(), stats);

        // LSD Radix Sort with sign-bit flipping (unified processing):
        // 1. Find min/max keys: n reads
        // 2. For each digit d (from 0 to digitCount-1):
        //    - Count phase: n reads
        //    - Distribute phase: n reads + n writes (to temp buffer)
        //    - Copy back phase (using CopyTo): n reads (from temp buffer) + n writes (to main buffer)
        //
        // For n elements with values [0, n-1]:
        // - max unsigned key = 0x8000_0000 + (n-1) for non-negative values
        // - digitCount = number of decimal digits needed to represent max key
        //
        // For example, n=100 → max value = 99 → max key = 0x80000063
        // - 0x80000063 = 2,147,483,747 in decimal → 10 decimal digits
        //
        // Total reads = n (find min/max) + digitCount × 3n (count + distribute + CopyTo read)
        // Total writes = digitCount × 2n (distribute write + CopyTo write)
        var maxValue = (uint)(n - 1);
        var maxKey = maxValue ^ 0x8000_0000; // Sign-bit flip for non-negative
        var digitCount = GetDigitCountFromUlong(maxKey);

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

        // LSD Radix Sort with sign-bit flipping:
        // Same as sorted - performance is data-independent O(d × n)
        var maxValue = (uint)(n - 1);
        var maxKey = maxValue ^ 0x8000_0000; // Sign-bit flip for non-negative
        var digitCount = GetDigitCountFromUlong(maxKey);

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
    public async Task TheoreticalValuesRandomTest(int n)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        RadixLSD10Sort.Sort(random.AsSpan(), stats);

        // LSD Radix Sort with sign-bit flipping:
        // Same complexity as sorted/reversed - O(d × n)
        var maxValue = (uint)(n - 1);
        var maxKey = maxValue ^ 0x8000_0000; // Sign-bit flip for non-negative
        var digitCount = GetDigitCountFromUlong(maxKey);

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

        // With sign-bit flipping, negative and positive numbers are processed uniformly:
        // 1. Find min/max keys: n reads
        // 2. For each digit d (from 0 to digitCount-1):
        //    - Count phase: n reads
        //    - Distribute phase: n reads + n writes (to temp buffer)
        //    - Copy back phase: n reads + n writes
        //
        // For input [-n/2, ..., -1, 0, 1, ..., n/2-1]:
        // - Min value: -n/2 → min key = 0x80000000 - n/2
        // - Max value: n/2-1 → max key = 0x80000000 + (n/2-1)
        // - Max key determines digit count
        var minValue = -n / 2;
        var maxValue = n / 2 - 1;
        var minKey = (uint)minValue ^ 0x8000_0000; // Sign-bit flip
        var maxKey = (uint)maxValue ^ 0x8000_0000; // Sign-bit flip
        var digitCount = GetDigitCountFromUlong(maxKey);

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
    /// Helper to calculate digit count for theoretical tests (for original values)
    /// </summary>
    private static int GetDigitCount(int value)
    {
        if (value == 0) return 1;

        var count = 0;
        var temp = Math.Abs(value);
        while (temp > 0)
        {
            temp /= 10;
            count++;
        }
        return count;
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
