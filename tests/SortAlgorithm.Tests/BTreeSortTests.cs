using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class BTreeSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => BTreeSort.Sort(span, context);

    // Node creation and the write-back pass always write, even for input that is already sorted.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    // Elements move through node slots; array slots are never swapped.
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    /// <summary>
    /// The point of a B-tree is that occupancy alone bounds the height, so no input degenerates the way an
    /// unbalanced binary search tree does on sorted input. The bound checked here is the theoretical one —
    /// each of the n descents visits at most one node per level and pays at most ⌈log₂(2t)⌉ comparisons
    /// inside it, plus one after a split — expressed against the height a B-tree of n keys can reach.
    /// </summary>
    [Test]
    [Arguments(1000)]
    [Arguments(4096)]
    public async Task ComparisonsStayWithinTheHeightBoundForEveryPattern(int n)
    {
        var inputs = new (string Name, int[] Data)[]
        {
            ("sorted", [.. Enumerable.Range(0, n)]),
            ("reversed", [.. Enumerable.Range(0, n).Reverse()]),
            ("random", TestHelpers.ShuffledRange(n, 20260803)),
            ("allEqual", [.. Enumerable.Repeat(7, n)]),
        };

        // A node holds at most 2t-1 = 15 keys, so the in-node binary search costs at most 4 comparisons,
        // and one more can follow a split. Height is bounded by log_t((n+1)/2) + 1.
        const int MinDegree = 8;
        const int MaxComparisonsPerNode = 5;
        var maxHeight = (int)Math.Floor(Math.Log((n + 1) / 2.0, MinDegree)) + 1;
        var bound = (ulong)n * MaxComparisonsPerNode * (ulong)maxHeight;

        foreach (var (name, data) in inputs)
        {
            var stats = new StatisticsContext();
            var array = data.ToArray();
            BTreeSort.Sort(array.AsSpan(), stats);

            await Assert.That(array).IsEquivalentTo([.. data.Order()], CollectionOrdering.Matching)
                .Because($"{name} input must sort");
            await Assert.That(stats.CompareCount).IsLessThanOrEqualTo(bound)
                .Because($"{name} input must stay within the O(n log n) comparison bound (height <= {maxHeight})");
            await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        }
    }

    /// <summary>
    /// Sorted input is what separates a B-tree from an unbalanced binary search tree: the tree still fills
    /// left to right, but every node it descends through is bounded in size, so the comparison count stays
    /// Θ(n log n) instead of collapsing to Θ(n²).
    /// </summary>
    [Test]
    public async Task SortedInputDoesNotDegenerate()
    {
        const int n = 2048;

        var bTree = new StatisticsContext();
        var bTreeArray = Enumerable.Range(0, n).ToArray();
        BTreeSort.Sort(bTreeArray.AsSpan(), bTree);

        var bst = new StatisticsContext();
        var bstArray = Enumerable.Range(0, n).ToArray();
        BinaryTreeSort.Sort(bstArray.AsSpan(), bst);

        // The unbalanced tree pays n(n-1)/2 here; a B-tree must be far below that.
        await Assert.That(bTree.CompareCount).IsLessThan(bst.CompareCount / 20);
    }

    /// <summary>
    /// Every element must reach the tree and come back: the write-back pass writes each position of the main
    /// span exactly once, so the main-buffer write count is exactly n and the sorted output is a permutation
    /// of the input even when the input is duplicate-heavy enough to fill nodes with equal keys.
    /// </summary>
    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(15)]   // exactly one full node, no split yet
    [Arguments(16)]   // the first root split
    [Arguments(17)]
    [Arguments(121)]  // enough for a third level
    [Arguments(1000)]
    public async Task DuplicateHeavyInputIsSortedAndPreserved(int n)
    {
        var random = new Random(20260804);
        var data = Enumerable.Range(0, n).Select(_ => random.Next(0, 4)).ToArray();
        var array = data.ToArray();

        BTreeSort.Sort(array.AsSpan(), new StatisticsContext());

        await Assert.That(array).IsEquivalentTo([.. data.Order()], CollectionOrdering.Matching);
    }
}
