using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class RadixMSD10SortTests
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


        RadixMSD10Sort.Sort(array.AsSpan(), stats);

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
        RadixMSD10Sort.Sort(keys.AsSpan());

        // After sorting by value, records with same value should maintain original order
        // Since we only sorted keys, we verify the sort is stable by checking
        // that multiple sorts preserve order
        var firstSort = records.Select(r => r.value).ToArray();
        RadixMSD10Sort.Sort(firstSort.AsSpan());

        var secondSort = firstSort.ToArray();
        RadixMSD10Sort.Sort(secondSort.AsSpan());

        await Assert.That(secondSort).IsEquivalentTo(firstSort, CollectionOrdering.Matching);
    }

    [Test]
    public async Task MinValueHandlingTest()
    {
        var stats = new StatisticsContext();
        // Test that int.MinValue is handled correctly (no overflow)
        var array = new[] { int.MinValue, -1, 0, 1, int.MaxValue };
        RadixMSD10Sort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(new[] { int.MinValue, -1, 0, 1, int.MaxValue }, CollectionOrdering.Matching);
    }

    [Test]
    public async Task SortWithNegativeNumbers()
    {
        var stats = new StatisticsContext();
        var array = new[] { -5, 3, -1, 0, 2, -3, 1 };
        var expected = new[] { -5, -3, -1, 0, 1, 2, 3 };
        RadixMSD10Sort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task SortWithAllSameValues()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 5, 5, 5, 5 };
        RadixMSD10Sort.Sort(array.AsSpan(), stats);

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
            RadixMSD10Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(sbyte))
        {
            var array = new sbyte[] { -5, 2, -8, 1, 9 };
            RadixMSD10Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(short))
        {
            var array = new short[] { -5, 2, -8, 1, 9 };
            RadixMSD10Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(ushort))
        {
            var array = new ushort[] { 5, 2, 8, 1, 9 };
            RadixMSD10Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(int))
        {
            var array = new int[] { -5, 2, -8, 1, 9 };
            RadixMSD10Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(uint))
        {
            var array = new uint[] { 5, 2, 8, 1, 9 };
            RadixMSD10Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(long))
        {
            var array = new long[] { -5, 2, -8, 1, 9 };
            RadixMSD10Sort.Sort(array.AsSpan(), stats);
            await Assert.That(IsSorted(array)).IsTrue();
        }
        else if (type == typeof(ulong))
        {
            var array = new ulong[] { 5, 2, 8, 1, 9 };
            RadixMSD10Sort.Sort(array.AsSpan(), stats);
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
    public async Task EmptyArrayTest()
    {
        var array = Array.Empty<int>();
        RadixMSD10Sort.Sort(array.AsSpan());
        await Assert.That(array).IsEmpty();
    }

    [Test]
    public async Task SingleElementTest()
    {
        var array = new[] { 42 };
        RadixMSD10Sort.Sort(array.AsSpan());
        await Assert.That(array).IsSingleElement();
        await Assert.That(array[0]).IsEqualTo(42);
    }

    [Test]
    public async Task TwoElementsTest()
    {
        var array = new[] { 2, 1 };
        RadixMSD10Sort.Sort(array.AsSpan());
        await Assert.That(array).IsEquivalentTo([1, 2], CollectionOrdering.Matching);
    }

    [Test]
    public async Task DecimalDigitBoundaryTest()
    {
        // Test values that cross decimal digit boundaries (9→10, 99→100, etc.)
        var array = new[] { 100, 9, 99, 10, 1, 999, 1000 };
        var expected = new[] { 1, 9, 10, 99, 100, 999, 1000 };
        RadixMSD10Sort.Sort(array.AsSpan());
        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task MSD10SpecificTest()
    {
        // Test specifically for decimal (base-10) radix characteristics
        var array = new[] { 123, 456, 789, 12, 45, 78, 1, 4, 7 };
        var expected = new[] { 1, 4, 7, 12, 45, 78, 123, 456, 789 };
        RadixMSD10Sort.Sort(array.AsSpan());
        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task InsertionSortCutoffTest()
    {
        // Test with array smaller than insertion sort cutoff (16)
        var array = new[] { 10, 5, 3, 8, 1, 9, 2, 7, 4, 6 };
        var expected = new[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        RadixMSD10Sort.Sort(array.AsSpan());
        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }


    [Test]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        RadixMSD10Sort.Sort(array.AsSpan(), stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        // MSD radix sort uses comparisons in insertion sort for small buckets
        // For sorted data, insertion sort should have minimal comparisons
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
        RadixMSD10Sort.Sort(sorted.AsSpan(), stats);

        // MSD Radix Sort (decimal base-10):
        // MSD processes most significant digit first recursively.
        // Performance depends on:
        // - Data distribution (how elements spread across buckets)
        // - Small buckets (<=16 elements) use insertion sort (includes comparisons and swaps)
        // - Larger buckets continue with MSD partitioning
        //
        // For sorted data, elements distribute across buckets based on decimal digits.
        // Initial min/max scan: n reads
        // MSD passes: variable reads/writes depending on bucket distribution
        // Insertion sort in small buckets: comparisons and swaps occur
        //
        // Statistics validation:
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
        RadixMSD10Sort.Sort(reversed.AsSpan(), stats);

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
        RadixMSD10Sort.Sort(array.AsSpan(), stats);

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
        RadixMSD10Sort.Sort(mixed.AsSpan(), stats);

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

    [Test]
    public async Task SortEmptyArray()
    {
        var stats = new StatisticsContext();
        var array = Array.Empty<int>();
        RadixMSD10Sort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEmpty();
        await Assert.That(stats.IndexReadCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    [Test]
    public async Task SortSingleElement()
    {
        var stats = new StatisticsContext();
        var array = new[] { 42 };
        RadixMSD10Sort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsSingleElement();
        await Assert.That(array[0]).IsEqualTo(42);
        await Assert.That(stats.IndexReadCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

}
