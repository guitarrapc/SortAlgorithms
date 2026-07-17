using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class BinaryTreeSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => BinaryTreeSort.Sort(span, context);

    // Sorted input degenerates the BST into a linked list (O(n²) insertions).
    protected override int MaxOrderTestSize => 1024;

    // Tree construction (CreateNode, linking) and inorder write-back always write, even for sorted input.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    // BinaryTreeSort moves elements via node writes, never swaps.
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
        BinaryTreeSort.Sort(sorted.AsSpan(), stats);

        // For sorted data [0, 1, 2, ..., n-1], the BST becomes completely unbalanced
        // forming a right-skewed tree (worst case):
        // - Insertion comparisons: 0 + 1 + 2 + ... + (n-1) = n(n-1)/2
        // - Index reads: 4n + 2C
        //   = n (main reads) + C (value reads in CompareWithNode) + C (Left/Right navigation reads)
        //   + 3n (Inorder: Left + Value + Right per node)
        // - Index writes: 3n - 1
        //   = n (CreateNode) + (n-1) (parent linking) + n (main writes in Inorder)
        var expectedCompares = (ulong)(n * (n - 1) / 2);
        var expectedReads = (ulong)(4 * n) + 2 * expectedCompares;
        var expectedWrites = (ulong)(3 * n - 1);

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
        // - Same formulas as sorted data (symmetric tree shape)
        var expectedCompares = (ulong)(n * (n - 1) / 2);
        var expectedReads = (ulong)(4 * n) + 2 * expectedCompares;
        var expectedWrites = (ulong)(3 * n - 1);

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
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

        // Reads: 4n + 2*C (n main + C value reads + C structural reads + 3n inorder)
        // Writes: 3n - 1 (n CreateNode + (n-1) linking + n main writes)
        var expectedWrites = (ulong)(3 * n - 1);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexReadCount).IsEqualTo((ulong)(4 * n) + 2 * stats.CompareCount);
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

        // Reads: 4n + 2*C (n main + C value reads + C structural reads + 3n inorder)
        // Writes: 3n - 1 (n CreateNode + (n-1) linking + n main writes)
        var expectedWrites = (ulong)(3 * n - 1);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexReadCount).IsEqualTo((ulong)(4 * n) + 2 * stats.CompareCount);
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
}
