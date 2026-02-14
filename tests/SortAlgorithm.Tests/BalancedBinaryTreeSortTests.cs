using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class BalancedBinaryTreeSortTests
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
        Skip.When(inputSample.Samples.Length > 1024, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        BalancedBinaryTreeSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateHalf))]
    public async Task SortHalfResultOrderTest(IInputSample<Half> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 1024, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        BalancedBinaryTreeSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateFloat))]
    public async Task SortFloatResultOrderTest(IInputSample<float> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 1024, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        BalancedBinaryTreeSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateDouble))]
    public async Task SortDoubleResultOrderTest(IInputSample<double> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 1024, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        BalancedBinaryTreeSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockIntKeyRandomData), nameof(MockIntKeyRandomData.Generate))]
    public async Task SortIntStructResultOrderTest(IInputSample<Utils.IntKey> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 1024, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        BalancedBinaryTreeSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

#if DEBUG

    [Test]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BalancedBinaryTreeSort.Sort(array.AsSpan(), stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
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
        BalancedBinaryTreeSort.Sort(sorted.AsSpan(), stats);

        // For sorted data [0, 1, 2, ..., n-1], AVL tree maintains balance through rotations
        // Expected comparisons for balanced tree:
        // - Each insertion into a tree of i elements takes ~log2(i) comparisons
        // - Total: approximately n*log2(n) comparisons
        // With Value caching: reads are only for inserts (each element read once)
        // - Total reads: n (every element is read once during insertion)
        var avgCompares = (ulong)(n * Math.Log2(Math.Max(n, 2)));
        var minCompares = avgCompares / 2;  // Allow some variance
        var maxCompares = avgCompares * 2;  // Upper bound for balanced insertions
        var expectedReads = (ulong)n;  // Every element is read once during insertion
        var expectedWrites = (ulong)n; // Writing during in-order traversal

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
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
        BalancedBinaryTreeSort.Sort(reversed.AsSpan(), stats);

        // For reversed data [n-1, n-2, ..., 1, 0], AVL tree maintains balance through rotations
        // Expected comparisons for balanced tree:
        // - Each insertion into a tree of i elements takes ~log2(i) comparisons
        // - Total: approximately n*log2(n) comparisons
        // With Value caching: reads are only for inserts (each element read once)
        // - Total reads: n (every element is read once during insertion)
        var avgCompares = (ulong)(n * Math.Log2(Math.Max(n, 2)));
        var minCompares = avgCompares / 2;  // Allow some variance
        var maxCompares = avgCompares * 2;  // Upper bound for balanced insertions
        var expectedReads = (ulong)n;  // Every element is read once during insertion
        var expectedWrites = (ulong)n; // Writing during in-order traversal

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
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
        BalancedBinaryTreeSort.Sort(random.AsSpan(), stats);

        // For random data, AVL tree maintains balance automatically
        // Expected comparisons:
        // - Each insertion into a balanced tree of i elements takes ~log2(i) comparisons
        // - Total: approximately n*log2(n) comparisons
        // - Balanced tree guarantees O(log n) height, so worst case is better than BST
        // With Value caching: comparisons and traversal use cached values
        // - Total reads: n (every element is read once during insertion)
        var avgCompares = (ulong)(n * Math.Log2(Math.Max(n, 2)));
        var minCompares = avgCompares / 2;  // Allow variance for very balanced insertions
        var maxCompares = avgCompares * 2;  // Upper bound (still O(n log n))
        var expectedReads = (ulong)n;  // Every element is read once during insertion
        var expectedWrites = (ulong)n; // Writing during in-order traversal

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesBalancedPropertyTest(int n)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        BalancedBinaryTreeSort.Sort(random.AsSpan(), stats);

        // AVL tree guarantees that the height is always O(log n)
        // This ensures that comparisons remain in the O(n log n) range
        // even in worst-case scenarios (unlike unbalanced BST which degrades to O(n^2))
        var worstCaseBST = (ulong)(n * (n - 1) / 2);  // Unbalanced BST worst case
        var balancedUpperBound = (ulong)(n * Math.Log2(Math.Max(n, 2)) * 3);  // 3x safety margin

        // Verify that comparisons are within balanced tree bounds (O(n log n))
        // For small n, the difference may not be dramatic, so we use 70% of worst case as threshold
        await Assert.That(stats.CompareCount < worstCaseBST * 7 / 10).IsTrue().Because($"CompareCount ({stats.CompareCount}) should be better than 70% of unbalanced BST worst case ({worstCaseBST})");
        await Assert.That(stats.CompareCount < balancedUpperBound).IsTrue().Because($"CompareCount ({stats.CompareCount}) should be within balanced tree bounds ({balancedUpperBound})");
    }

#endif

}
