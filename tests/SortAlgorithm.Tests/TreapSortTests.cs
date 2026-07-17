using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class TreapSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => TreapSort.Sort(span, context);

    // Node allocation and traversal overhead make large inputs slow.
    protected override int MaxOrderTestSize => 1024;

    // Tree construction (CreateNode, linking) and inorder write-back always write, even for sorted input.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    // TreapSort moves elements via node writes, never swaps.
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
