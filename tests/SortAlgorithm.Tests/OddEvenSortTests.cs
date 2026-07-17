using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class OddEvenSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => OddEvenSort.Sort(span, context);

    // O(n^2) algorithm: keep data-driven inputs small.
    protected override int MaxOrderTestSize => 512;

    // Sorted input needs a single verification pass with no swaps, hence no writes.
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
        OddEvenSort.Sort(sorted.AsSpan(), stats);

        // Odd-Even Sort performs one pass through the array for sorted data
        // - Odd-even pass: floor(n/2) comparisons (positions 0-1, 2-3, 4-5, ...)
        // - Even-odd pass: floor((n-1)/2) comparisons (positions 1-2, 3-4, 5-6, ...)
        // Total comparisons per pass ≈ n-1
        // For sorted data, one pass is enough to verify, so approximately n-1 comparisons
        var expectedCompares = (ulong)(n - 1);
        var expectedSwaps = 0UL;
        var expectedWrites = 0UL; // No swaps = no writes

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
        OddEvenSort.Sort(reversed.AsSpan(), stats);

        // Odd-Even Sort worst case (reversed data):
        // The algorithm needs approximately n/2 phases to sort reversed data
        // Each phase does approximately n-1 comparisons:
        // - Odd-even pass: floor(n/2) comparisons
        // - Even-odd pass: floor((n-1)/2) comparisons
        // Total comparisons ≈ (n/2) * (n-1) ≈ n²/2
        //
        // Empirical observations:
        // - n=10: 54 comparisons
        // - n=20: 209 comparisons
        // - n=50: 1274 comparisons
        // - n=100: 5049 comparisons
        //
        // Pattern: approximately n²/2 comparisons
        var minCompares = (ulong)(n * n / 2 * 0.95); // Allow 5% below
        var maxCompares = (ulong)(n * n / 2 * 1.10); // Allow 10% above

        var expectedSwaps = (ulong)(n * (n - 1) / 2);
        var expectedWrites = expectedSwaps * 2; // Each swap writes 2 elements

        // Each comparison reads 2 elements
        var minIndexReads = minCompares * 2;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
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
        OddEvenSort.Sort(random.AsSpan(), stats);

        // Odd-Even Sort performs approximately n/4 to n/2 phases for random data
        // Each phase does approximately n-1 comparisons
        // Average comparisons ≈ n²/4 to n²/2
        //
        // For random data, the actual behavior varies significantly based on initial order
        // Allow wider range to account for lucky initial states (almost sorted by chance)
        var minCompares = (ulong)(n); // At minimum, need one pass (n-1) comparisons
        var maxCompares = (ulong)(n * n / 2 * 1.1); // Allow 10% above worst case average

        // For random data, swap count varies significantly
        // Average case: approximately n²/8 to n(n-1)/2
        // Allow wider range for random variations
        var minSwaps = 0UL; // Lucky case: already sorted
        var maxSwaps = (ulong)(n * (n - 1) / 2);

        // Each comparison reads 2 elements
        var minIndexReads = minCompares * 2;

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");
    }
}
