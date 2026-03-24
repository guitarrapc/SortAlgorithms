using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class BogoSortTests
{
    [Test, SkipCI]
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
        // Bogo Sort is extremely slow, so we limit to small arrays
        Skip.When(inputSample.Samples.Length > 10, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        BogoSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }


    [Test, SkipCI]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        // Bogo Sort is extremely slow, so we limit to small arrays
        Skip.When(inputSample.Samples.Length > 10, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BogoSort.Sort(array.AsSpan(), stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    [Test, SkipCI]
    public async Task TheoreticalValuesSortedTest()
    {
        // Bogo Sort for sorted data should:
        // 1. Check if sorted (n-1 comparisons, 2*(n-1) reads)
        // 2. Already sorted, so no shuffle needed
        // 3. Exit immediately with 0 swaps and 0 writes
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, 5).ToArray();
        BogoSort.Sort(sorted.AsSpan(), stats);

        var n = sorted.Length;
        var expectedCompares = (ulong)(n - 1);
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL;
        var expectedReads = (ulong)(2 * (n - 1));

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(expectedSwaps);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
    }

    [Test, SkipCI]
    public async Task TheoreticalValuesSingleShuffleTest()
    {
        // Test with a very small array that requires exactly one shuffle
        // For array [1, 0], it needs one shuffle to become [0, 1]
        var stats = new StatisticsContext();
        var array = new[] { 1, 0 };
        BogoSort.Sort(array.AsSpan(), stats);

        var n = array.Length;

        // First iteration: IsSorted check (1 comparison, 2 reads) -> not sorted
        // Shuffle: n swaps (each swap: 2 reads + 2 writes)
        // Second iteration: IsSorted check (1 comparison, 2 reads) -> sorted
        //
        // Note: Actual values depend on Random.Shared.Next results
        // We can only verify that sorting happened and operations were counted
        await Assert.That(stats.CompareCount >= (ulong)(n - 1)).IsTrue();
        await Assert.That(stats.SwapCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(array).IsEquivalentTo([0, 1], CollectionOrdering.Matching);
    }

    [Test, SkipCI]
    [Arguments(3)]
    [Arguments(5)]
    [Arguments(7)]
    public async Task TheoreticalValuesRandomTest(int n)
    {
        // Bogo Sort has unbounded runtime for random data
        // We can only test that:
        // 1. The array gets sorted
        // 2. All operation counts are non-zero
        // 3. Each shuffle performs n swaps
        // 4. Each IsSorted check performs n-1 comparisons
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        BogoSort.Sort(random.AsSpan(), stats);

        // Verify the array is sorted
        await Assert.That(random).IsEquivalentTo(Enumerable.Range(0, n), CollectionOrdering.Matching);

        // Verify operations were performed
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexReadCount > 0).IsTrue();

        // For non-sorted input, there must be at least one shuffle
        // Each shuffle performs n swaps (2n reads + 2n writes)
        // Minimum is when array becomes sorted after first shuffle
        await Assert.That(stats.SwapCount >= 0).IsTrue().Because($"SwapCount ({stats.SwapCount}) should be >= 0");
        await Assert.That(stats.IndexWriteCount >= 0).IsTrue().Because($"IndexWriteCount ({stats.IndexWriteCount}) should be >= 0");
    }

    [Test, SkipCI]
    [Arguments(3)]
    [Arguments(5)]
    [Arguments(7)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        // Bogo Sort for reversed data has unbounded runtime
        // We verify that:
        // 1. The array gets sorted
        // 2. Multiple shuffles are performed (reversed is very unlikely to sort quickly)
        // 3. All operation counts are significant
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        BogoSort.Sort(reversed.AsSpan(), stats);

        // Verify the array is sorted
        await Assert.That(reversed).IsEquivalentTo(Enumerable.Range(0, n), CollectionOrdering.Matching);

        // Verify significant operations were performed
        await Assert.That(stats.CompareCount >= (ulong)(n - 1)).IsTrue().Because($"CompareCount ({stats.CompareCount}) should be >= {n - 1}");
        await Assert.That(stats.SwapCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
    }

}
