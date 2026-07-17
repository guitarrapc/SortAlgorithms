using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class StableQuickSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => StableQuickSort.Sort(span, context);

    // Every partition level writes all elements through the temporary buffer, even on sorted input.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    // StableQuickSort doesn't use Swap - it uses Read/Write to copy via temporary buffers.
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    public async Task StabilityLargeDuplicateHeavyTest()
    {
        // Duplicate-heavy input large enough to exercise many recursion levels,
        // where an unstable partition would reorder equal keys
        var stats = new StatisticsContext();
        var random = new Random(42);
        var items = Enumerable.Range(0, 2000)
            .Select(i => new StabilityTestItem(random.Next(0, 10), i))
            .ToArray();

        StableQuickSort.Sort(items.AsSpan(), stats);

        // Values ascending, and within each equal-value group the original indices ascending
        for (var i = 1; i < items.Length; i++)
        {
            await Assert.That(items[i - 1].Value <= items[i].Value).IsTrue()
                .Because($"values must be ascending at index {i}");
            if (items[i - 1].Value == items[i].Value)
            {
                await Assert.That(items[i - 1].OriginalIndex < items[i].OriginalIndex).IsTrue()
                    .Because($"equal values must keep original order at index {i}");
            }
        }
    }

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        StableQuickSort.Sort(array.AsSpan(), 2, 6, stats);

        // Expected: first 2 elements unchanged, middle 4 sorted, last 3 unchanged
        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort the entire array using range API
        StableQuickSort.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

#if DEBUG

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        StableQuickSort.Sort(sorted.AsSpan(), stats);

        // Stable QuickSort on sorted data:
        // - Using middle element as pivot
        // - For sorted data, this provides balanced partitions
        // - Each partition reads all elements, compares with pivot, writes them back
        // - Uses temporary buffers (no swaps)
        //
        // Expected behavior:
        // - Comparisons: O(n log n) - Each element compared with pivot at each level
        //   For sorted data with middle pivot: approximately n log n comparisons
        // - Reads: O(n log n) - Each partitioning level reads all n elements + pivot
        //   Approximately (n+1) log n reads total
        // - Writes: O(n log n) - Each partitioning level writes all n elements back
        //   Approximately n log n writes total
        // - Swaps: 0 - This algorithm uses Read/Write, not Swap
        // - Recursion depth: O(log n) with balanced partitions
        var minCompares = (ulong)(n); // At minimum, each element visited once
        var maxCompares = (ulong)(n * n); // Worst case O(n²) if partitioning fails

        // StableQuickSort doesn't use Swap - it copies via temporary buffers
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        // IndexReads: Each recursion level reads pivot + all elements in range
        // For balanced partitioning: approximately (n+1) * log₂(n) reads
        var minIndexReads = (ulong)n; // At least read all elements once
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        // IndexWrites: Each recursion level writes all elements in range back
        // For balanced partitioning: approximately n * log₂(n) writes
        var minIndexWrites = (ulong)n; // At least write all elements once
        await Assert.That(stats.IndexWriteCount >= minIndexWrites).IsTrue().Because($"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minIndexWrites}");
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
        StableQuickSort.Sort(reversed.AsSpan(), stats);

        // Stable QuickSort on reversed data:
        // - Using middle element as pivot
        // - For reversed data with middle pivot, partitioning is still balanced
        // - Uses temporary buffers to rearrange elements (no swaps)
        //
        // Expected behavior:
        // - Comparisons: O(n log n) average case
        //   Similar to sorted data since middle pivot provides balance
        // - Reads: O(n log n) - Each partitioning level reads all elements
        // - Writes: O(n log n) - Each partitioning level writes elements back
        // - Swaps: 0 - This algorithm uses Read/Write, not Swap
        var minCompares = (ulong)(n);
        var maxCompares = (ulong)(n * n); // Worst case (rare with middle pivot)

        // StableQuickSort doesn't use Swap
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        // IndexReads: At least read all elements once
        var minIndexReads = (ulong)n;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        // IndexWrites: At least write all elements once
        var minIndexWrites = (ulong)n;
        await Assert.That(stats.IndexWriteCount >= minIndexWrites).IsTrue().Because($"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minIndexWrites}");
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
        StableQuickSort.Sort(random.AsSpan(), stats);

        // Stable QuickSort on random data: average case O(n log n)
        // - Middle element as pivot provides decent balance on average
        // - Partitioning divides array into approximately two halves
        // - Uses temporary buffers for stable partitioning
        //
        // Expected behavior:
        // - Comparisons: O(n log n) average
        //   Approximately n log₂ n comparisons
        // - Reads: O(n log n) average - each level reads all elements
        // - Writes: O(n log n) average - each level writes all elements back
        // - Swaps: 0 - This algorithm uses Read/Write, not Swap
        var minCompares = (ulong)(n);
        var maxCompares = (ulong)(n * n); // Worst case (very rare with random data)

        // StableQuickSort doesn't use Swap
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        // IndexReads: At least read all elements once
        var minIndexReads = (ulong)n;
        await Assert.That(stats.IndexReadCount >= minIndexReads).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minIndexReads}");

        // IndexWrites: At least write all elements once
        var minIndexWrites = (ulong)n;
        await Assert.That(stats.IndexWriteCount >= minIndexWrites).IsTrue().Because($"IndexWriteCount ({stats.IndexWriteCount}) should be >= {minIndexWrites}");
    }

    [Test]
    [Arguments(5)]
    [Arguments(10)]
    [Arguments(20)]
    public async Task TheoreticalValuesSameElementsTest(int n)
    {
        var stats = new StatisticsContext();
        var sameValues = Enumerable.Repeat(42, n).ToArray();
        StableQuickSort.Sort(sameValues.AsSpan(), stats);

        // Stable QuickSort on all equal elements:
        // - All elements equal to pivot
        // - Three-way partitioning puts all elements in the "equal" partition
        // - Equal partition is already sorted, so no recursion occurs
        // - This is an OPTIMIZATION: O(n) time instead of O(n log n)
        //
        // Expected behavior with SortSpan-based temporary buffer (index-based pivot):
        //
        // MedianOf3Index (all elements equal):
        //   - s.Compare(lowIdx, midIdx): comparison between indices
        //   - low == mid, so else branch
        //   - s.Compare(midIdx, highIdx): comparison between indices
        //   - mid == high, so else branch
        //   - Returns midIdx (no value read required)
        //   Total: 2 compares, 4 reads
        //
        // StablePartition Phase 1 (count):
        //   - for loop: n iterations
        //   - i == pivotIndex short-circuits: (n-1) comparisons (skips 1 Compare call)
        //   Total: n-1 compares, 2(n-1) reads
        //
        // StablePartition Phase 2 (distribute to temp):
        //   - for loop: n iterations
        //   - s.Read(i): n reads (store to element)
        //   - i == pivotIndex short-circuits: (n-1) comparisons (skips 1 Compare call)
        //   - tempSortSpan.Write(equalIdx++, element): n writes to temp buffer
        //   Total: n reads (element) + 2(n-1) reads (compare) = 3n-2 reads, n-1 compares, n writes to temp
        //
        // StablePartition Phase 3 (copy back):
        //   - tempSortSpan.CopyTo(...): n reads from temp, n writes to main
        //   Total: n reads from temp, n writes to main
        //
        // Grand Total:
        //   - Compares: 2 (median) + (n-1) (phase1) + (n-1) (phase2) = 2n
        //   - IndexReads: 4 (median) + 2(n-1) (phase1) + (3n-2) (phase2) + n (phase3) = 6n
        //   - Swaps: 0

        // Comparisons: 2 (median-of-3) + 2(n-1) (partition phases 1+2) = 2n
        var expectedCompares = (ulong)(2 * n);
        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);

        // StableQuickSort doesn't use Swap
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);

        // Verify the array is still correct (all values unchanged)
        foreach (var item in sameValues) await Assert.That(item).IsEqualTo(42);

        // IndexReads: 4 (median) + 2(n-1) (phase1) + (3n-2) (phase2) + n (phase3) = 6n
        var expectedIndexReads = (ulong)(6 * n);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedIndexReads);

        // IndexWrites: n (phase2 to temp) + n (phase3 to main) = 2n
        var expectedIndexWrites = (ulong)(2 * n);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedIndexWrites);
    }

#endif

}
