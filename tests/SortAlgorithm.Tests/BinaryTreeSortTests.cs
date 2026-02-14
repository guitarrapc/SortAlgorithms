using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class BinaryTreeSortTests
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

        BinaryTreeSort.Sort(array.AsSpan(), stats);

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

        BinaryTreeSort.Sort(array.AsSpan(), stats);

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

        BinaryTreeSort.Sort(array.AsSpan(), stats);

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

        BinaryTreeSort.Sort(array.AsSpan(), stats);

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

        BinaryTreeSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

#if DEBUG

    [Test]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        if (inputSample.Samples.Length > 1024)
            return;

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BinaryTreeSort.Sort(array.AsSpan(), stats);

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
        BinaryTreeSort.Sort(sorted.AsSpan(), stats);

        // For sorted data [0, 1, 2, ..., n-1], the BST becomes completely unbalanced
        // forming a right-skewed tree (worst case):
        // - Insertion comparisons: 0 + 1 + 2 + ... + (n-1) = n(n-1)/2
        // - Each insertion reads one element from the array: n reads
        // - In-order traversal writes all elements back: n writes
        var expectedCompares = (ulong)(n * (n - 1) / 2);
        var expectedReads = (ulong)n;  // Reading during insertion
        var expectedWrites = (ulong)n; // Writing during in-order traversal

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
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
        BinaryTreeSort.Sort(reversed.AsSpan(), stats);

        // For reversed data [n-1, n-2, ..., 1, 0], the BST becomes completely unbalanced
        // forming a left-skewed tree (worst case):
        // - Insertion comparisons: 0 + 1 + 2 + ... + (n-1) = n(n-1)/2
        // - Each insertion reads one element from the array: n reads
        // - In-order traversal writes all elements back: n writes
        var expectedCompares = (ulong)(n * (n - 1) / 2);
        var expectedReads = (ulong)n;  // Reading during insertion
        var expectedWrites = (ulong)n; // Writing during in-order traversal

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
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
        BinaryTreeSort.Sort(random.AsSpan(), stats);

        // For random data, the BST is likely to be more balanced (average case)
        // Average insertion comparisons for balanced tree:
        // Each insertion into a tree of i elements takes ~log2(i) comparisons
        // Total: sum of log2(i) for i=1 to n
        //
        // Approximation: n*log2(n) - 1.44*n (based on average case analysis)
        // However, random data can vary, so we use a flexible range:
        // - Lower bound: about 50% of n*log2(n) (very lucky balanced insertions)
        // - Upper bound: worst case n(n-1)/2 (unlikely but possible)
        var avgCompares = n * Math.Log2(n);
        var minCompares = (ulong)(avgCompares * 0.4);  // Allow significantly lower for balanced trees
        var maxCompares = (ulong)(n * (n - 1) / 2);    // Worst case (unbalanced)

        var expectedReads = (ulong)n;  // Reading during insertion
        var expectedWrites = (ulong)n; // Writing during in-order traversal

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    [Test]
    [Arguments(7)]  // Perfect binary tree: 3 levels
    [Arguments(15)] // Perfect binary tree: 4 levels
    [Arguments(31)] // Perfect binary tree: 5 levels
    public async Task TheoreticalValuesBalancedTest(int n)
    {
        var stats = new StatisticsContext();
        // Create a sequence that produces a balanced BST
        // Using middle-out insertion order for near-perfect balance
        var balanced = CreateBalancedInsertionOrder(n);
        BinaryTreeSort.Sort(balanced, stats);

        // For a balanced tree, insertion comparisons are O(n log n)
        // Each insertion into a balanced tree of height h requires ~h comparisons
        // Average height for balanced tree: log2(n)
        var minCompares = (ulong)(n * Math.Log2(n) * 0.5);  // Lower bound
        var maxCompares = (ulong)(n * Math.Log2(n) * 2.0);  // Upper bound with some overhead

        var expectedReads = (ulong)n;  // Reading during insertion
        var expectedWrites = (ulong)n; // Writing during in-order traversal

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    /// <summary>
    /// Creates an array with insertion order that produces a relatively balanced BST.
    /// Uses a middle-recursive approach similar to binary search tree construction.
    /// </summary>
    private static Span<int> CreateBalancedInsertionOrder(int n)
    {
        var sorted = Enumerable.Range(0, n).ToArray();
        var result = new int[n];
        var index = 0;

        void AddMiddle(int left, int right)
        {
            if (left > right) return;
            var mid = left + (right - left) / 2;
            result[index++] = sorted[mid];
            AddMiddle(left, mid - 1);
            AddMiddle(mid + 1, right);
        }

        AddMiddle(0, n - 1);
        return result.AsSpan();
    }

#endif

}
