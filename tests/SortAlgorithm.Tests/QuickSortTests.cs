using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class QuickSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => QuickSort.Sort(span, context);

    // Hoare partition swaps elements even on sorted input (i/j crossing swaps).
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.NonZero;

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        QuickSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        QuickSort.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
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
        QuickSort.Sort(sorted.AsSpan(), stats);

        // QuickSort with Hoare partition on sorted data:
        // - Using middle element as pivot
        // - For sorted data, this provides relatively balanced partitions
        // - Each partition pass scans through elements comparing with pivot
        //
        // Expected behavior:
        // - Comparisons: O(n log n) - Hoare partition compares elements with pivot
        //   Average case: approximately 2n ln n ≈ 1.39n log₂ n comparisons
        //   For sorted data with middle pivot, performance is near-optimal
        // - Swaps: O(n log n) - Elements swap positions during partitioning
        //   Even on sorted data, Hoare partition performs swaps when i and j cross
        //   Average case: approximately n ln n / 3 swaps
        // - Recursion depth: O(log n) with balanced partitions
        var minCompares = (ulong)(n); // At minimum, each element visited once
        var maxCompares = (ulong)(n * n); // Worst case O(n²) if partitioning fails

        // For sorted data with middle pivot, swaps still occur during partitioning
        var minSwaps = 0UL;
        var maxSwaps = (ulong)(n * Math.Log(n, 2)); // O(n log n) average

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);

        // IndexReads: Each comparison reads elements
        // In Hoare partition, s.Compare(i, pivot) reads s[i], and s.Compare(pivot, j) reads s[j]
        // Plus initial pivot read and swap reads
        var minIndexReads = stats.CompareCount; // At least one read per comparison
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        // IndexWrites: Each swap writes 2 elements
        var expectedWrites = stats.SwapCount * 2;
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
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
        QuickSort.Sort(reversed.AsSpan(), stats);

        // QuickSort with Hoare partition on reversed data:
        // - Using middle element as pivot
        // - For reversed data with middle pivot, partitioning is still balanced
        // - Many swaps needed to rearrange elements
        //
        // Expected behavior:
        // - Comparisons: O(n log n) average case
        //   Similar to sorted data since middle pivot provides balance
        // - Swaps: O(n log n) - Many elements need repositioning
        //   Hoare partition performs approximately n ln n / 3 swaps on average
        var minCompares = (ulong)(n);
        var maxCompares = (ulong)(n * n); // Worst case (rare with middle pivot)

        var minSwaps = (ulong)(n / 2); // At least half the elements need swapping
        var maxSwaps = (ulong)(n * Math.Log(n, 2) * 2); // O(n log n) with some overhead

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);

        // IndexReads: At least one per comparison
        var minIndexReads = stats.CompareCount;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        // IndexWrites: Each swap writes 2 elements
        var expectedWrites = stats.SwapCount * 2;
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
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
        QuickSort.Sort(random.AsSpan(), stats);

        // QuickSort with Hoare partition on random data: average case O(n log n)
        // - Middle element as pivot provides decent balance on average
        // - Partitioning divides array into approximately two halves
        //
        // Expected behavior:
        // - Comparisons: O(n log n) average
        //   Approximately 2n ln n ≈ 1.39n log₂ n comparisons
        // - Swaps: O(n log n) average
        //   Approximately n ln n / 3 swaps
        //   Hoare partition performs fewer swaps than Lomuto partition
        var minCompares = (ulong)(n);
        var maxCompares = (ulong)(n * n); // Worst case (very rare with random data)

        var minSwaps = 0UL; // Best case: already sorted by chance
        var maxSwaps = (ulong)(n * Math.Log(n, 2) * 2); // Average with overhead

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);

        // IndexReads: At least one per comparison
        var minIndexReads = stats.CompareCount;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        // IndexWrites: Each swap writes 2 elements
        var expectedWrites = stats.SwapCount * 2;
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
    }

    [Test]
    [Arguments(5)]
    [Arguments(10)]
    [Arguments(20)]
    public async Task TheoreticalValuesSameElementsTest(int n)
    {
        var stats = new StatisticsContext();
        var sameValues = Enumerable.Repeat(42, n).ToArray();
        QuickSort.Sort(sameValues.AsSpan(), stats);

        // QuickSort with Hoare partition on all equal elements:
        // - All elements equal to pivot
        // - Hoare partition still scans through array
        // - Many unnecessary swaps occur since all elements are equal
        // - This is a weakness of basic QuickSort (3-way partitioning handles this better)
        //
        // Expected behavior:
        // - Comparisons: O(n log n) - Still performs full partitioning at each level
        // - Swaps: O(n log n) - Many redundant swaps of equal elements
        //   Hoare partition swaps elements even when they're equal to pivot
        var minCompares = (ulong)(n);
        var maxCompares = (ulong)(n * n); // Worst case for equal elements

        // Swaps: Hoare partition swaps equal elements unnecessarily
        var minSwaps = 0UL;
        var maxSwaps = (ulong)(n * Math.Log(Math.Max(n, 2), 2) * 2);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(minSwaps, maxSwaps);

        // Verify the array is still correct (all values unchanged)
        foreach (var item in sameValues) await Assert.That(item).IsEqualTo(42);

        // IndexReads: At least one per comparison
        var minIndexReads = stats.CompareCount;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        // IndexWrites: Each swap writes 2 elements
        var expectedWrites = stats.SwapCount * 2;
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
    }
}
