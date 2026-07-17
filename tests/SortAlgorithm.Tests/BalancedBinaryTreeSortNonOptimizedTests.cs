using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

// Non-optimized reference implementation is slow; the whole class is local-only.
[SkipCI]
[InheritsTests]
public class BalancedBinaryTreeSortNonOptimizedTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => BalancedBinaryTreeSortNonOptimized.Sort(span, context);

    // Node allocation and rebalancing overhead make large inputs slow.
    protected override int MaxOrderTestSize => 1024;

    // In-order traversal writes all elements back, even for sorted input.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    // AVL insertion moves elements via node writes, never swaps.
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        BalancedBinaryTreeSortNonOptimized.Sort(sorted.AsSpan(), stats);

        // For sorted data [0, 1, 2, ..., n-1], AVL tree maintains balance through rotations
        // Expected comparisons for balanced tree:
        // - Each insertion into a tree of i elements takes ~log2(i) comparisons
        // - Total: approximately n*log2(n) comparisons
        // - Each insertion reads one element from the array: n reads
        // - In-order traversal writes all elements back: n writes
        var avgCompares = (ulong)(n * Math.Log2(Math.Max(n, 2)));
        var minCompares = avgCompares / 2;  // Allow some variance
        var maxCompares = avgCompares * 2;  // Upper bound for balanced insertions
        var expectedReads = (ulong)n;  // Reading during insertion
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
        BalancedBinaryTreeSortNonOptimized.Sort(reversed.AsSpan(), stats);

        // For reversed data [n-1, n-2, ..., 1, 0], AVL tree maintains balance through rotations
        // Expected comparisons for balanced tree:
        // - Each insertion into a tree of i elements takes ~log2(i) comparisons
        // - Total: approximately n*log2(n) comparisons
        // - Each insertion reads one element from the array: n reads
        // - In-order traversal writes all elements back: n writes
        var avgCompares = (ulong)(n * Math.Log2(Math.Max(n, 2)));
        var minCompares = avgCompares / 2;  // Allow some variance
        var maxCompares = avgCompares * 2;  // Upper bound for balanced insertions
        var expectedReads = (ulong)n;  // Reading during insertion
        var expectedWrites = (ulong)n; // Writing during in-order traversal

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
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
        BalancedBinaryTreeSortNonOptimized.Sort(random.AsSpan(), stats);

        // For random data, AVL tree maintains balance automatically
        // Expected comparisons:
        // - Each insertion into a balanced tree of i elements takes ~log2(i) comparisons
        // - Total: approximately n*log2(n) comparisons
        // - Balanced tree guarantees O(log n) height, so worst case is better than BST
        var avgCompares = (ulong)(n * Math.Log2(Math.Max(n, 2)));
        var minCompares = avgCompares / 2;  // Allow variance for very balanced insertions
        var maxCompares = avgCompares * 2;  // Upper bound (still O(n log n))
        var expectedReads = (ulong)n;  // Reading during insertion
        var expectedWrites = (ulong)n; // Writing during in-order traversal

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
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
    public async Task TheoreticalValuesBalancedPropertyTest(int n, int seed)
    {
        var stats = new StatisticsContext();
        var random = TestHelpers.ShuffledRange(n, seed);
        BalancedBinaryTreeSortNonOptimized.Sort(random.AsSpan(), stats);

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
}
