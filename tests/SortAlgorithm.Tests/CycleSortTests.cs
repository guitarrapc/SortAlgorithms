using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class CycleSortTests
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
        Skip.When(inputSample.Samples.Length > 512, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        CycleSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateHalf))]
    public async Task SortHalfResultOrderTest(IInputSample<Half> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 512, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        CycleSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateFloat))]
    public async Task SortFloatResultOrderTest(IInputSample<float> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 512, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        CycleSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateDouble))]
    public async Task SortDoubleResultOrderTest(IInputSample<double> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 512, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        CycleSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockIntKeyRandomData), nameof(MockIntKeyRandomData.Generate))]
    public async Task SortIntStructResultOrderTest(IInputSample<Utils.IntKey> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 512, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        CycleSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }


    [Test]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        if (inputSample.Samples.Length > 1024)
            return;

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        CycleSort.Sort(array.AsSpan(), stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
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
        CycleSort.Sort(sorted.AsSpan(), stats);

        // Cycle Sort performs FindPosition comparisons: n(n-1)/2
        // For sorted data, SkipDuplicates is called but no actual duplicates exist,
        // so it performs minimal additional comparisons (1 per call to verify no match)
        var findPositionCompares = (ulong)(n * (n - 1) / 2);

        // Sorted data: no writes needed (all elements already in correct positions)
        var expectedWrites = 0UL;

        // For sorted data, FindPosition is called n-1 times (once per cycleStart)
        // Each call results in pos == cycleStart, so no SkipDuplicates is invoked
        var expectedCompares = findPositionCompares;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount >= findPositionCompares).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {findPositionCompares}");
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
        CycleSort.Sort(reversed.AsSpan(), stats);

        // Cycle Sort performs FindPosition comparisons: n(n-1)/2 as the base
        var findPositionCompares = (ulong)(n * (n - 1) / 2);

        // However, FindPosition is called multiple times per cycle:
        // 1. Once before the initial write
        // 2. Multiple times in the while loop until the cycle completes
        //
        // For reversed data, the actual number of comparisons is approximately
        // 2x the base due to cycle rotations and SkipDuplicates calls.
        // Each element that moves requires additional FindPosition calls within its cycle.
        //
        // Empirical observations:
        // - n=10: ~90 comparisons (2.0x base)
        // - n=20: ~355 comparisons (1.87x base)
        // - n=50: ~2200 comparisons (1.80x base)
        //
        // We use a range to accommodate variations in cycle lengths.
        var minCompares = findPositionCompares;
        var maxCompares = findPositionCompares * 3; // Allow up to 3x for safety

        // Reversed data: each element needs to be moved to its correct position
        // In the worst case (reversed), n-1 elements need to be written
        var minWrites = (ulong)(n - 1);
        var maxWrites = (ulong)n;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minCompares).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minCompares}");
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
        CycleSort.Sort(random.AsSpan(), stats);

        // Cycle Sort performs FindPosition comparisons: n(n-1)/2 as the base
        var findPositionCompares = (ulong)(n * (n - 1) / 2);

        // For random data, the actual number of comparisons varies significantly
        // based on the random arrangement and resulting cycle lengths.
        // FindPosition is called multiple times per cycle (once initially, then
        // repeatedly in the while loop until the cycle completes).
        // Additionally, SkipDuplicates is called for each position found.
        //
        // Empirical observations show random data can require 2-3x the base comparisons.
        // We use a generous range to account for different random arrangements.
        var minCompares = findPositionCompares;
        var maxCompares = findPositionCompares * 4; // Allow up to 4x for random variation

        // Random data: most elements need to be moved
        // Typically between n/2 and n writes
        var minWrites = (ulong)(n / 2);
        var maxWrites = (ulong)n;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minCompares).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minCompares}");
    }

}
