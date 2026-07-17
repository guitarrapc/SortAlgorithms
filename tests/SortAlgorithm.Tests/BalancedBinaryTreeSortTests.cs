using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class BalancedBinaryTreeSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => BalancedBinaryTreeSort.Sort(span, context);

    // Node allocation and rebalancing overhead make large inputs slow.
    protected override int MaxOrderTestSize => 1024;

    // Tree construction (CreateNode, linking) and inorder write-back always write, even for sorted input.
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
        BalancedBinaryTreeSort.Sort(sorted.AsSpan(), stats);

        // For sorted data [0, 1, 2, ..., n-1], AVL tree maintains balance through rotations.
        // Each insertion into a tree of i elements takes ~log2(i) comparisons.
        var nLogN = (ulong)(n * Math.Log2(Math.Max(n, 2)));
        await Assert.That(stats.CompareCount).IsBetween(nLogN / 2, nLogN * 2);

        // With full structural tracking, reads include:
        //   n (main reads) + C (CompareWithNode) + C (Left/Right navigation)
        //   + 3n (Inorder: Left + Value + Right) + rebalancing reads (UpdateHeight, GetBalance, rotations)
        // Minimum: 4n + 2C (BST baseline without rebalancing). AVL adds O(n log n) rebalancing reads.
        var minReads = (ulong)(4 * n) + 2 * stats.CompareCount + 1;
        await Assert.That(stats.IndexReadCount).IsBetween(minReads, nLogN * 15);

        // With full structural tracking, writes include:
        //   n (CreateNode) + (n-1) (parent linking) + n (Inorder writes)
        //   + height update writes + rotation pointer writes
        // Minimum: 3n - 1 (BST baseline). AVL adds O(n log n) rebalancing writes.
        await Assert.That(stats.IndexWriteCount).IsBetween((ulong)(3 * n), nLogN * 5);

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

        // For reversed data [n-1, n-2, ..., 1, 0], AVL tree maintains balance through rotations.
        // Same formulas as sorted data (symmetric tree shape).
        var nLogN = (ulong)(n * Math.Log2(Math.Max(n, 2)));
        await Assert.That(stats.CompareCount).IsBetween(nLogN / 2, nLogN * 2);

        // Reads: BST baseline 4n + 2C plus rebalancing reads (UpdateHeight, GetBalance, rotations)
        var minReads = (ulong)(4 * n) + 2 * stats.CompareCount + 1;
        await Assert.That(stats.IndexReadCount).IsBetween(minReads, nLogN * 15);

        // Writes: BST baseline 3n - 1 plus rebalancing writes (height updates, rotation pointer writes)
        await Assert.That(stats.IndexWriteCount).IsBetween((ulong)(3 * n), nLogN * 5);

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
        BalancedBinaryTreeSort.Sort(random.AsSpan(), stats);

        // For random data, AVL tree guarantees O(n log n) comparisons regardless of input.
        var nLogN = (ulong)(n * Math.Log2(Math.Max(n, 2)));
        await Assert.That(stats.CompareCount).IsBetween(nLogN / 2, nLogN * 2);

        // Reads: BST baseline 4n + 2C plus rebalancing reads (UpdateHeight, GetBalance, rotations)
        var minReads = (ulong)(4 * n) + 2 * stats.CompareCount + 1;
        await Assert.That(stats.IndexReadCount).IsBetween(minReads, nLogN * 15);

        // Writes: BST baseline 3n - 1 plus rebalancing writes (height updates, rotation pointer writes)
        await Assert.That(stats.IndexWriteCount).IsBetween((ulong)(3 * n), nLogN * 5);

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
}
