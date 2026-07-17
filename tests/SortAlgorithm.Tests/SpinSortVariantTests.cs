using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class SpinSortVariantTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => SpinSortVariant.Sort(span, context);

    // Sorted input: BinaryInsertionSort (n <= 72) or CheckPreSorted (n > 72) detects order: no writes, no swaps.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(10)]
    [Arguments(36)]
    [Arguments(72)]
    [Arguments(73)]
    [Arguments(100)]
    [Arguments(200)]
    public async Task SortSmallAndBoundaryTest(int n)
    {
        var stats = new StatisticsContext();
        var rng = new Random(42);
        var array = Enumerable.Range(0, n).Select(_ => rng.Next(0, n)).ToArray();
        var expected = array.ToArray();
        Array.Sort(expected);

        SpinSortVariant.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
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
        SpinSortVariant.Sort(sorted.AsSpan(), stats);

        // SpinSort for sorted data:
        // n ≤ 72: BinaryInsertionSort detects each element is already in place with a single comparison,
        //         yielding exactly n-1 comparisons, 0 writes, 0 swaps.
        // n > 72: CheckPreSorted detects ascending order with n-1 comparisons.
        // Both paths produce identical statistics.
        //
        // Observed:
        // n=10:  9 comparisons,  0 writes, 18 reads, 0 swaps
        // n=20:  19 comparisons, 0 writes, 38 reads, 0 swaps
        // n=50:  49 comparisons, 0 writes, 98 reads, 0 swaps
        // n=100: 99 comparisons, 0 writes, 198 reads, 0 swaps
        await Assert.That(stats.CompareCount).IsEqualTo((ulong)(n - 1));
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo((ulong)(2 * (n - 1)));
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
        SpinSortVariant.Sort(reversed.AsSpan(), stats);

        // SpinSort for reversed data:
        // n ≤ 72: BinaryInsertionSort handles it directly.
        //         O(n log n) comparisons for binary search, O(n²) writes for shifting elements.
        // n > 72: CheckPreSorted detects strictly descending → Reverse in-place.
        //         Exactly n-1 comparisons, n writes (2 per swap), n/2 swaps.
        //
        // Observed:
        // n=10:  34 comparisons, 54 writes, 0 swaps    (BinaryInsertionSort)
        // n=20:  88 comparisons, 209 writes, 0 swaps   (BinaryInsertionSort)
        // n=50:  286 comparisons, 1274 writes, 0 swaps (BinaryInsertionSort)
        // n=100: 99 comparisons, 100 writes, 50 swaps  (CheckPreSorted + Reverse)
        ulong minCompares, maxCompares, minWrites, maxWrites, minSwaps, maxSwaps;
        if (n <= 72)
        {
            // BinaryInsertionSort: O(n log n) compares, O(n²) writes
            minCompares = (ulong)(n - 1);
            maxCompares = (ulong)(n * Math.Ceiling(Math.Log2(n + 1)));
            minWrites = (ulong)(n);
            maxWrites = (ulong)((long)n * n);
            minSwaps = 0UL;
            maxSwaps = 0UL; // BinaryInsertionSort uses shifts, not swaps
        }
        else
        {
            // CheckPreSorted → Reverse: exact values
            minCompares = (ulong)(n - 1);
            maxCompares = (ulong)(n - 1);
            minWrites = (ulong)n;
            maxWrites = (ulong)n;
            minSwaps = (ulong)(n / 2);
            maxSwaps = (ulong)(n / 2);
        }

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
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
        SpinSortVariant.Sort(random.AsSpan(), stats);

        // SpinSort for random data:
        // n ≤ 72: BinaryInsertionSort — O(n log n) comparisons, O(n²) writes.
        // n > 72: Full SpinSort — O(n log n) comparisons and writes.
        //         CheckPreSorted returns 0, then half-buffer + ping-pong merge.
        //
        // Observed ranges (20 seeds):
        // n=10:  Compares=[20..31],     Writes=[16..36],       Swaps=0
        // n=20:  Compares=[65..81],     Writes=[81..140],      Swaps=0
        // n=50:  Compares=[243..268],   Writes=[512..844],     Swaps=0
        // n=100: Compares=[831..933],   Writes=[933..1033],    Swaps=0
        ulong minCompares, maxCompares, minWrites, maxWrites;
        if (n <= 72)
        {
            // BinaryInsertionSort: O(n log n) comparisons, up to O(n²) writes
            minCompares = (ulong)(n - 1);
            maxCompares = (ulong)(n * Math.Ceiling(Math.Log2(n + 1)));
            minWrites = 0UL;
            maxWrites = (ulong)((long)n * n);
        }
        else
        {
            // Full SpinSort: O(n log n) comparisons and writes
            var logN = Math.Log2(n);
            minCompares = (ulong)(n * logN * 0.5);
            maxCompares = (ulong)(n * logN * 2.0);
            minWrites = (ulong)n;
            maxWrites = (ulong)(n * logN * 2.0);
        }

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
        // SpinSort uses merges (reads + writes), not swaps — swaps only occur during Reverse in CheckPreSorted
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }
}
