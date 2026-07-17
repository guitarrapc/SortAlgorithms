using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class SplaySortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => SplaySort.Sort(span, context);

    // Tree construction is O(n log n) with per-node structural tracking; keep data-driven tests on small inputs.
    protected override int MaxOrderTestSize => 1024;

    // Splay tree construction always writes nodes (and writes back via inorder traversal), but never swaps.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
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
        SplaySort.Sort(sorted.AsSpan(), stats);

        // For sorted input [0, 1, 2, ..., n-1], the splay tree exhibits adaptive behavior:
        // - Each insertion compares against the current root (which is the previous element after splaying)
        // - Since each new element is always larger than the root, it inserts to the right in 1 comparison
        // - After splay (zig case) the new element becomes root, ready for the next insertion
        // - Total comparisons: n-1 (one per insertion after the first)
        //
        // With full structural tracking per insertion (i >= 1):
        //   Insert: 1 main read + 1 CompareWithNode read + 1 Right pointer read
        //           + 1 CreateNode write + 1 Right pointer write + 1 Parent pointer write
        //   Splay (zig): 7 reads (Parent check, grandparent check, direction check,
        //                rotation: Right/Left/Parent reads, loop end Parent check)
        //                + 4 writes (rotation: Right/Parent/Left/Parent writes)
        //   Per element: 10 reads + 7 writes + 1 compare
        //   First element: 1 read + 1 write
        //   Inorder: 3n reads + n writes
        // Total: R = 1 + 10(n-1) + 3n = 13n - 9
        //        W = 1 + 7(n-1) + n = 8n - 6
        var expectedCompares = (ulong)(n - 1);
        var expectedReads = (ulong)(13 * n - 9);
        var expectedWrites = (ulong)(8 * n - 6);

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
        SplaySort.Sort(reversed.AsSpan(), stats);

        // For reversed input [n-1, n-2, ..., 1, 0], the splay tree also exhibits O(n) comparisons:
        // - Each insertion compares against the current root (the previous, larger element after splaying)
        // - Since each new element is always smaller than the root, it inserts to the left in 1 comparison
        // - After splay (zig case) the new element becomes root, ready for the next insertion
        // - Same exact formulas as sorted data (symmetric tree shape and rotation patterns)
        var expectedCompares = (ulong)(n - 1);
        var expectedReads = (ulong)(13 * n - 9);
        var expectedWrites = (ulong)(8 * n - 6);

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
        SplaySort.Sort(random.AsSpan(), stats);

        // For random input, splay sort gives O(n log n) amortized comparisons.
        // Use a broad range to accommodate variance in random inputs:
        // - Lower bound: n-1 (best case: monotone input collapses to linear)
        // - Upper bound: n*(n-1)/2 (worst case single-operation, but amortized O(n log n))
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n * (n - 1) / 2);
        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        // With full structural tracking, reads include:
        //   n (main reads) + C (CompareWithNode) + C (Left/Right navigation)
        //   + 3n (Inorder: Left + Value + Right) + Splay reads (Parent, direction, rotation reads)
        // Minimum: 13n - 9 (sorted/reversed case with single zig per insertion).
        // Random input causes deeper insertions and zig-zig/zig-zag splay cases with more reads.
        var minReads = (ulong)(13 * n - 9);
        var nLogN = (ulong)(n * Math.Log2(Math.Max(n, 2)));
        await Assert.That(stats.IndexReadCount).IsBetween(minReads, nLogN * 20);

        // Writes: minimum 8n - 6 (sorted/reversed case). More with deeper splay operations.
        var minWrites = (ulong)(8 * n - 6);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, nLogN * 20);

        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }
}
