using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class BitonicSortParallelTests
{
    [Test]
    [MethodDataSource(typeof(MockPowerOfTwoRandomData), nameof(MockPowerOfTwoRandomData.Generate))]
    [MethodDataSource(typeof(MockPowerOfTwoNegativePositiveRandomData), nameof(MockPowerOfTwoNegativePositiveRandomData.Generate))]
    [MethodDataSource(typeof(MockPowerOfTwoReversedData), nameof(MockPowerOfTwoReversedData.Generate))]
    [MethodDataSource(typeof(MockPowerOfTwoNearlySortedData), nameof(MockPowerOfTwoNearlySortedData.Generate))]
    [MethodDataSource(typeof(MockPowerOfTwoSameValuesData), nameof(MockPowerOfTwoSameValuesData.Generate))]
    public async Task SortResultOrderTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();


        BitonicSortParallel.Sort(array, stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    public async Task ThrowsOnNonPowerOfTwo()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 3, 1, 4, 1, 5, 9, 2 }; // Length 7 is not power of 2

        Assert.Throws<ArgumentException>(() => BitonicSortParallel.Sort(array, stats));
    }

    [Test]
    public async Task ThrowsOnNullArray()
    {
        var stats = new StatisticsContext();
        int[]? array = null;

        Assert.Throws<ArgumentNullException>(() => BitonicSortParallel.Sort(array!, stats));
    }

    [Test]
    public async Task ThrowsOnNullContext()
    {
        var array = new int[] { 1, 2, 3, 4 };

        Assert.Throws<ArgumentNullException>(() => BitonicSortParallel.Sort(array, (StatisticsContext)null!));
    }

    [Test]
    public async Task EmptyArray()
    {
        var stats = new StatisticsContext();
        var array = Array.Empty<int>();
        BitonicSortParallel.Sort(array, stats);
        await Assert.That(array).IsEmpty();
    }

    [Test]
    public async Task SingleElement()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 42 };
        BitonicSortParallel.Sort(array, stats);
        await Assert.That(array).IsSingleElement();
        await Assert.That(array[0]).IsEqualTo(42);
    }

    [Test]
    public async Task TwoElements()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 2, 1 };
        BitonicSortParallel.Sort(array, stats);
        await Assert.That(array.Length).IsEqualTo(2);
        await Assert.That(array[0]).IsEqualTo(1);
        await Assert.That(array[1]).IsEqualTo(2);
    }

    [Test]
    public async Task FourElements()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 3, 1, 4, 2 };
        BitonicSortParallel.Sort(array, stats);
        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4], CollectionOrdering.Matching);
    }

    [Test]
    public async Task EightElements()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 5, 2, 8, 1, 9, 3, 7, 4 };
        BitonicSortParallel.Sort(array, stats);
        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task SixteenElementsAllSame()
    {
        var stats = new StatisticsContext();
        var array = Enumerable.Repeat(42, 16).ToArray();
        BitonicSortParallel.Sort(array, stats);
        foreach (var item in array) await Assert.That(item).IsEqualTo(42);
    }

    [Test]
    public async Task LargeArrayParallelization()
    {
        // Test with array size >= PARALLEL_THRESHOLD (1024)
        var stats = new StatisticsContext();
        var array = Enumerable.Range(0, 2048).Reverse().ToArray();
        BitonicSortParallel.Sort(array, stats);

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, 2048).ToArray(), CollectionOrdering.Matching);
    }

    [Test]
    public async Task VeryLargeArray()
    {
        // Test with a very large power-of-2 array to ensure parallelization works
        var stats = new StatisticsContext();
        var random = new Random(42);
        var array = Enumerable.Range(0, 4096).OrderBy(_ => random.Next()).ToArray();
        var expected = array.OrderBy(x => x).ToArray();

        BitonicSortParallel.Sort(array, stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }


    [Test]
    [MethodDataSource(typeof(MockPowerOfTwoSortedData), nameof(MockPowerOfTwoSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BitonicSortParallel.Sort(array, stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.CompareCount > 0).IsTrue();
        await Assert.That(stats.IndexReadCount > 0).IsTrue();
    }

    [Test]
    [Arguments(2)]
    [Arguments(4)]
    [Arguments(8)]
    [Arguments(16)]
    [Arguments(32)]
    [Arguments(64)]
    [Arguments(128)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        BitonicSortParallel.Sort(sorted, stats);

        // Bitonic sort performs the same number of comparisons regardless of input order
        var expectedCompares = CalculateBitonicComparisons(n);

        // For sorted data, swaps may still occur due to data-oblivious nature
        var expectedReads = expectedCompares * 2 + stats.SwapCount * 2;
        var expectedWrites = stats.SwapCount * 2;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount >= 0).IsTrue();
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
    }

    [Test]
    [Arguments(2)]
    [Arguments(4)]
    [Arguments(8)]
    [Arguments(16)]
    [Arguments(32)]
    [Arguments(64)]
    [Arguments(128)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        BitonicSortParallel.Sort(reversed, stats);

        var expectedCompares = CalculateBitonicComparisons(n);
        var expectedReads = expectedCompares * 2 + stats.SwapCount * 2;
        var expectedWrites = stats.SwapCount * 2;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount > 0).IsTrue().Because("Reversed array should require swaps");
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
    }

    [Test]
    [Arguments(2)]
    [Arguments(4)]
    [Arguments(8)]
    [Arguments(16)]
    [Arguments(32)]
    [Arguments(64)]
    [Arguments(128)]
    public async Task TheoreticalValuesRandomTest(int n)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        BitonicSortParallel.Sort(random, stats);

        var expectedCompares = CalculateBitonicComparisons(n);
        var expectedReads = expectedCompares * 2 + stats.SwapCount * 2;
        var expectedWrites = stats.SwapCount * 2;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount >= 0).IsTrue();
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
    }

    private static ulong CalculateBitonicComparisons(int n)
    {
        if (n <= 1) return 0;

        int k = 0;
        int temp = n;
        while (temp > 1)
        {
            temp >>= 1;
            k++;
        }

        return (ulong)(n * k * (k + 1) / 4);
    }
}
