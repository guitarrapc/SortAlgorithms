using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class TreapSortTests
{
    [Test]
    [MethodDataSource(typeof(MockRandomData), nameof(MockRandomData.Generate))]
    [MethodDataSource(typeof(MockNegativePositiveRandomData), nameof(MockNegativePositiveRandomData.Generate))]
    [MethodDataSource(typeof(MockNegativeRandomData), nameof(MockNegativeRandomData.Generate))]
    [MethodDataSource(typeof(MockReversedData), nameof(MockReversedData.Generate))]
    [MethodDataSource(typeof(MockReversedWithDuplicatesData), nameof(MockReversedWithDuplicatesData.Generate))]
    [MethodDataSource(typeof(MockPipeorganData), nameof(MockPipeorganData.Generate))]
    [MethodDataSource(typeof(MockNearlySortedData), nameof(MockNearlySortedData.Generate))]
    [MethodDataSource(typeof(MockAllSameData), nameof(MockAllSameData.Generate))]
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

        TreapSort.Sort(array.AsSpan(), stats);

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

        TreapSort.Sort(array.AsSpan(), stats);

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

        TreapSort.Sort(array.AsSpan(), stats);

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

        TreapSort.Sort(array.AsSpan(), stats);

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

        TreapSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        TreapSort.Sort(array.AsSpan(), stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
        // Comparisons are tracked via OnCompare(-1, -1, ...) in CompareWithNode
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
        TreapSort.Sort(sorted.AsSpan(), stats);

        // For sorted input, treap uses random priorities so tree shape is independent of input order.
        // BST insertion depth depends on random priorities, giving expected O(n log n) comparisons.
        // - Lower bound: n-1 (each insertion needs at least 1 comparison, except the first element)
        // - Upper bound: n*(n-1)/2 (degenerate case, astronomically unlikely with random priorities)
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n * (n - 1) / 2);
        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        // With full structural tracking, reads include:
        //   n (main reads) + C (CompareWithNode) + C (Left/Right navigation)
        //   + 3n (Inorder: Left + Value + Right) + HeapUp reads (Parent, Priority) + rotation reads
        // Minimum: 4n + 2C (BST baseline). HeapUp/rotations add more.
        var nLogN = (ulong)(n * Math.Log2(Math.Max(n, 2)));
        var minReads = (ulong)(4 * n) + 2 * stats.CompareCount + 1;
        await Assert.That(stats.IndexReadCount).IsBetween(minReads, nLogN * 20);

        // With full structural tracking, writes include:
        //   n (CreateNode) + (n-1) (parent linking) + n (Inorder writes)
        //   + HeapUp rotation writes (parent pointer updates, child pointer updates)
        // Minimum: 4n - 2 (BST baseline: n CreateNode + 2(n-1) linking + n Inorder).
        await Assert.That(stats.IndexWriteCount).IsBetween((ulong)(3 * n), nLogN * 20);

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
        TreapSort.Sort(reversed.AsSpan(), stats);

        // For reversed input, treap behavior is the same as sorted input because random priorities
        // make the tree shape independent of input order. Expected O(n log n) comparisons.
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n * (n - 1) / 2);
        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        // Reads: BST baseline 4n + 2C plus HeapUp reads (Parent, Priority) and rotation reads
        var nLogN = (ulong)(n * Math.Log2(Math.Max(n, 2)));
        var minReads = (ulong)(4 * n) + 2 * stats.CompareCount + 1;
        await Assert.That(stats.IndexReadCount).IsBetween(minReads, nLogN * 20);

        // Writes: BST baseline 4n - 2 plus HeapUp/rotation writes
        await Assert.That(stats.IndexWriteCount).IsBetween((ulong)(3 * n), nLogN * 20);

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
        TreapSort.Sort(random.AsSpan(), stats);

        // For random input, treap gives expected O(n log n) comparisons.
        // Random priorities combined with random input order yield the same expected tree depth.
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n * (n - 1) / 2);
        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        // Reads: BST baseline 4n + 2C plus HeapUp reads (Parent, Priority) and rotation reads
        var nLogN = (ulong)(n * Math.Log2(Math.Max(n, 2)));
        var minReads = (ulong)(4 * n) + 2 * stats.CompareCount + 1;
        await Assert.That(stats.IndexReadCount).IsBetween(minReads, nLogN * 20);

        // Writes: BST baseline 4n - 2 plus HeapUp/rotation writes
        await Assert.That(stats.IndexWriteCount).IsBetween((ulong)(3 * n), nLogN * 20);

        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }
}
