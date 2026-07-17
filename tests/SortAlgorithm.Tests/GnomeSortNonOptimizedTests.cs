using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

// Non-optimized variant duplicates GnomeSort coverage; run locally only.
[SkipCI]
[InheritsTests]
public class GnomeSortNonOptimizedTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => GnomeSortNonOptimized.Sort(span, context);

    // O(n^2) algorithm: keep data-driven inputs small.
    protected override int MaxOrderTestSize => 512;

    // Sorted input walks forward without ever swapping: no swaps, no writes.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
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
        GnomeSortNonOptimized.Sort(sorted.AsSpan(), stats);

        // GnomeSortNonOptimized - Sorted case:
        // Compares: n-1 (each element checked once, all in correct position)
        // Swaps: 0 (no swaps needed)
        // Writes: 0 (no swaps = no writes)
        var expectedCompares = (ulong)(n - 1);
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL;

        // Each comparison reads 2 elements
        var minIndexReads = expectedCompares * 2;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(expectedSwaps);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
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
        GnomeSortNonOptimized.Sort(reversed.AsSpan(), stats);

        // GnomeSortNonOptimized - Reversed case (worst case):
        // This implementation is actually the same algorithm as GnomeSort
        // Swaps: 1 + 2 + 3 + ... + (n-1) = n(n-1)/2
        // Compares: Each swap requires 1 compare before swap + 1 compare in while/if condition check
        //           Total compares = 2 * n(n-1)/2 = n(n-1)
        // Each swap writes 2 elements, so total writes = n(n-1)
        var expectedSwaps = (ulong)(n * (n - 1) / 2);
        var expectedCompares = (ulong)(n * (n - 1));
        var expectedWrites = (ulong)(n * (n - 1));

        // Each comparison reads 2 elements
        var minIndexReads = expectedCompares * 2;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount).IsEqualTo(expectedSwaps);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
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
        GnomeSortNonOptimized.Sort(random.AsSpan(), stats);

        // GnomeSortNonOptimized - Random case:
        // Minimum compares: n-1 (best case, already sorted by chance)
        // Maximum compares: n(n-1) (worst case, reversed - each swap causes 2 compares)
        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n * (n - 1));

        // Swaps vary depending on initial disorder
        var minSwaps = 0UL;
        var maxSwaps = (ulong)(n * (n - 1) / 2);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
    }
}
