using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class GlidesortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => Glidesort.Sort(span, context);

    // Sorted input is detected as a single ascending run (or handled by InsertionSort fast path): no writes, no swaps.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    [Arguments(4)]
    [Arguments(5)]
    [Arguments(6)]
    [Arguments(7)]
    public async Task StabilityTestSort4Path(int n)
    {
        // The pattern [2a, 2b, 1, 3, ...] specifically targets the Sort4 code path (4 <= n < 8).
        // The old in-place swap network [0,2],[1,3],[0,1],[2,3],[1,2] was unstable: comparator [0,2]
        // would swap 2a (position 0) with 1 (position 2), indirectly moving 2a past 2b and
        // producing [1, 2b, 2a, 3] instead of the stable [1, 2a, 2b, 3].
        // Sort4Into (matching reference sort4_raw) uses conditional value selection and is stable.
        var items = new StabilityTestItem[n];
        items[0] = new StabilityTestItem(2, 0); // value=2, originally at index 0
        items[1] = new StabilityTestItem(2, 1); // value=2, originally at index 1 (equal to items[0])
        items[2] = new StabilityTestItem(1, 2); // value=1
        items[3] = new StabilityTestItem(3, 3); // value=3
        for (var i = 4; i < n; i++) items[i] = new StabilityTestItem(i + 1, i);

        var stats = new StatisticsContext();
        Glidesort.Sort(items.AsSpan(), stats);

        // The two equal "2" values must remain in original order: origIdx 0 before 1
        var twoIndices = items.Where(x => x.Value == 2).Select(x => x.OriginalIndex).ToArray();
        await Assert.That(twoIndices).IsEquivalentTo(new[] { 0, 1 }, CollectionOrdering.Matching);
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
        Glidesort.Sort(sorted.AsSpan(), stats);

        // Glidesort for sorted data:
        // Small arrays (n < SMALL_SORT=48) use BlockInsertionSort (branchless networks: always write regardless of sortedness).
        // Larger arrays are detected as a single ascending run with n-1 comparisons and 0 writes.
        //
        // Actual observations for sorted data:
        // n=10:  21 comparisons, 20 writes, 0 swaps   (BlockInsertionSort: Sort8(18c,16w) + block[2](3c,4w))
        // n=20:  59 comparisons, 56 writes, 0 swaps   (BlockInsertionSort: Sort16(52c,48w) + block[4](7c,8w))
        //   Sort16 uses full Pow2SmallSort pipeline: Sort4Into×4 → DoubleMerge(k=4) → SymmetricMerge(k=8).
        //   SymmetricMerge always performs k iterations × 2 comparisons = 16, regardless of input order.
        // n=50:  49 comparisons, 0 writes, 0 swaps    (Single ascending run detected)
        // n=100: 99 comparisons, 0 writes, 0 swaps    (Single ascending run detected)
        if (n < 48)
        {
            // BlockInsertionSort: branchless Sort4/8/16/32 pipelines write deterministically regardless of input order.
            var expectedCompares = n == 10 ? 21UL : 59UL;
            var expectedWrites = n == 10 ? 20UL : 56UL;
            await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
            await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
            await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
            await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        }
        else
        {
            // GlidesortCore: detects single ascending run with n-1 comparisons, 0 writes
            var expectedCompares = (ulong)(n - 1);
            await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
            await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
            await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
            await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        }
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
        Glidesort.Sort(reversed.AsSpan(), stats);

        // Glidesort for reversed data:
        // Small arrays (n < SMALL_SORT=48) use BlockInsertionSort (branchless, fewer writes than InsertionSort for reversed data).
        // Larger arrays detect single strictly descending run and reverse via swaps: n-1 comparisons, n/2 swaps.
        //
        // Actual observations for reversed data:
        // n=10:  27 comparisons, 30 writes, 0 swaps   (BlockInsertionSort)
        // n=20:  74 comparisons, 81 writes, 0 swaps   (BlockInsertionSort: Sort16 SymmetricMerge pipeline)
        // n=50:  49 comparisons, 50 writes, 25 swaps   (Single descending run + reverse)
        // n=100: 99 comparisons, 100 writes, 50 swaps  (Single descending run + reverse)
        //
        // Pattern: For n < 48, BlockInsertionSort; for n >= 48, run detection + reverse with n/2 swaps
        ulong minCompares, maxCompares, minWrites, maxWrites, minSwaps, maxSwaps;
        if (n < 48)
        {
            // BlockInsertionSort: branchless networks (lower writes than InsertionSort for reversed)
            minCompares = (ulong)n;
            maxCompares = (ulong)(n * (n - 1) / 2 + n);
            minWrites = (ulong)n;
            maxWrites = (ulong)(n * (n + 1) / 2);
            minSwaps = 0UL;
            maxSwaps = 0UL;
        }
        else
        {
            // Single descending run detected + reverse
            minCompares = (ulong)(n - 1);
            maxCompares = (ulong)(n + 1);
            minWrites = (ulong)(n - 1);
            maxWrites = (ulong)(n + 1);
            minSwaps = (ulong)(n / 2 - 1);
            maxSwaps = (ulong)(n / 2 + 1);
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
        Glidesort.Sort(random.AsSpan(), stats);

        // Glidesort for random data:
        // Small arrays (n < SMALL_SORT=48) use InsertionSort at the top-level fast path.
        // Larger arrays use the powersort merge tree + stable quicksort for unsorted blocks.
        // Unsorted blocks (up to SMALL_SORT=48 elements) are sorted with BlockInsertionSort,
        // which uses Sort4/Sort8/Sort16/Sort32 sorting networks, then merged via powersort.
        //
        // Observed range for random data:
        // n=10:  ~29-37 comparisons, ~28-38 writes, 0 swaps   (InsertionSort top-level fast path)
        // n=20:  ~113-141 comparisons, ~114-140 writes, 0 swaps (InsertionSort top-level fast path)
        // n=50:  ~360-406 comparisons, ~408-472 writes, small swaps from Sort4 sorting network
        // n=100: ~785-891 comparisons, ~982-1105 writes, small swaps from Sort4 sorting network
        //
        // Pattern: approximately 1.0-2.0 * n * log₂(n) comparisons and writes
        // Swaps occur from Sort4 sorting network (up to 5 swaps per 4 elements) and
        // from reversing descending runs. Total Sort4 swaps ≤ 5*n/4 < 2*n.
        var logN = Math.Log2(n);
        ulong minCompares, maxCompares, minWrites, maxWrites;
        if (n < 48)
        {
            // InsertionSort top-level fast path: O(n) to O(n²)
            minCompares = (ulong)(n - 1);
            maxCompares = (ulong)(n * (n - 1) / 2 + n);
            minWrites = 0UL;
            maxWrites = (ulong)(n * (n + 1) / 2);
        }
        else
        {
            // Powersort merge tree + stable quicksort with BlockInsertionSort
            minCompares = (ulong)(n * logN * 0.5);
            maxCompares = (ulong)(n * logN * 2.5);
            minWrites = (ulong)(n * logN * 0.5);
            maxWrites = (ulong)(n * logN * 3.0);
        }

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
        // Swaps only come from Reverse() on strictly descending runs (≤ n/2 swaps per run)
        // and from the swap-in-place fallback in PhysicalMerge (rare). Sort4/Sort8/Sort16/Sort32
        // all use Sort4Into (out-of-place writes, no swaps). Total swaps ≪ 2*n for random data.
        await Assert.That(stats.SwapCount < (ulong)(2 * n)).IsTrue().Because($"SwapCount ({stats.SwapCount}) should be less than 2*n ({2 * n})");
    }
}
