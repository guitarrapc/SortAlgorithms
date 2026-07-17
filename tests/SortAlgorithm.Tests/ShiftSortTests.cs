using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class ShiftSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => ShiftSort.Sort(span, context);

    // Sorted input is detected as a single run: no merges, no writes, no swaps.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    [Arguments(256, 42)]  // Stackalloc threshold
    [Arguments(256, 1234)]
    [Arguments(257, 42)]  // Just over threshold (should use ArrayPool)
    [Arguments(257, 1234)]
    [Arguments(512, 42)]  // ArrayPool
    [Arguments(512, 1234)]
    [Arguments(1024, 42)] // Large array
    [Arguments(1024, 1234)]
    public async Task LargeArrayTest(int n, int seed)
    {
        var stats = new StatisticsContext();
        var array = TestHelpers.ShuffledRange(n, seed);
        ShiftSort.Sort(array.AsSpan(), stats);

        // Verify sorting correctness
        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, n).ToArray(), CollectionOrdering.Matching);
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
        ShiftSort.Sort(sorted.AsSpan(), stats);

        // For sorted data (with internal buffer tracking):
        // - Run detection: O(n) comparisons (n-1 comparisons in the scan loop)
        // - No run boundaries detected, so no merge operations
        // - No swaps needed
        // - No writes needed
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
        ShiftSort.Sort(reversed.AsSpan(), stats);

        // For reversed data [n-1, n-2, ..., 1, 0] (with internal buffer tracking):
        // - Run detection: O(n) comparisons
        //   * Every adjacent pair is out of order (n-1 boundaries detected)
        //   * Each boundary detection checks 2 elements (current and previous)
        //   * Three-element optimization applies when possible
        // - Maximum number of runs: approximately n/2 (worst case)
        // - Merge operations: O(n log k) where k is number of runs
        // - Swaps during run detection: O(n/2) empirically observed
        //   * The 3-element optimization swaps elements at positions x and x-2
        //   * For reversed data, this creates approximately n/2 swaps
        // - Writes during merge: O(n log k)
        //   * NOW includes writes to temp buffers (tmp1st or tmp2nd)
        //   * Each merge: writes to temp buffer + writes back to main

        // Run detection comparisons: approximately n
        var minRunDetectionCompares = (ulong)(n - 1);

        // Swaps are limited to run detection phase only (not during merge)
        // Empirically observed: reversed data produces approximately n/2 swaps
        // due to the 3-element optimization pattern
        var maxSwaps = (ulong)(n / 2 + 5); // Allow some margin for edge cases

        // Comparisons include both run detection and merge
        // For reversed data, expect O(n log n) total comparisons
        var minCompares = minRunDetectionCompares;
        var maxCompares = (ulong)(n * Math.Log(n, 2) * 2); // 2x for safety margin

        // Writes occur during merge (shift-based, not swap-based)
        // With internal buffer tracking: writes to temp buffer + writes back
        // For reversed data, most elements need to be shifted multiple times
        var minWrites = (ulong)(n - 1);
        // Allow for higher writes due to temp buffer operations being tracked
        var maxWrites = (ulong)(n * Math.Log(n, 2) * 3);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(0UL, maxSwaps);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minCompares * 2).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minCompares * 2}");
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
        ShiftSort.Sort(random.AsSpan(), stats);

        // For random data (with internal buffer tracking):
        // - Number of runs: varies significantly (typically k << n)
        // - Comparisons: O(n log k) where k is number of runs
        // - Swaps during run detection: typically less than n/2
        // - Writes during merge: O(n log k)
        //   * NOW includes writes to temp buffers

        var minCompares = (ulong)(n - 1); // At least run detection
        var maxCompares = (ulong)(n * Math.Log(n, 2) * 2); // At most O(n log n)

        var maxSwaps = (ulong)(n / 2 + 5); // Limited to run detection phase

        // Random data typically requires many merges
        // With internal buffer tracking: writes to temp buffer + writes back
        var minWrites = (ulong)(n / 4);
        var maxWrites = (ulong)(n * Math.Log(n, 2) * 3);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(0UL, maxSwaps);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount >= minCompares * 2).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be >= {minCompares * 2}");
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesAlternatingTest(int n)
    {
        var stats = new StatisticsContext();
        // Create alternating pattern: [0, 2, 4, ..., 1, 3, 5, ...]
        var alternating = Enumerable.Range(0, n)
            .OrderBy(x => x % 2)
            .ThenBy(x => x)
            .ToArray();
        ShiftSort.Sort(alternating.AsSpan(), stats);

        // Alternating data creates multiple runs that need merging
        // This tests the adaptive merge behavior

        var minCompares = (ulong)(n - 1);
        var maxCompares = (ulong)(n * Math.Log(n, 2) * 2);

        var maxSwaps = (ulong)(n / 2 + 5);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.SwapCount).IsBetween(0UL, maxSwaps);
        await Assert.That(stats.IndexWriteCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
    }
}
