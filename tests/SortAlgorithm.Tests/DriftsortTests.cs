using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class DriftsortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => Driftsort.Sort(span, context);

    // Sorted input: n <= 20 uses the guarded insertion fast path (no writes),
    // larger inputs are detected as a single ascending run (no writes, no swaps).
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    [Arguments(1000)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        Driftsort.Sort(sorted.AsSpan(), stats);

        // Driftsort sorts fully ascending input with exactly n-1 comparisons and no writes:
        // n <= 20 uses the guarded insertion fast path, larger inputs detect a single
        // ascending run covering the whole array (run threshold is min(n - n/2, 64) for
        // n <= 4096 and ~sqrt(n) above, both always cleared by a full-length run).
        await Assert.That(stats.CompareCount).IsEqualTo((ulong)(n - 1));
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    [Arguments(1000)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        Driftsort.Sort(reversed.AsSpan(), stats);

        if (n <= 20)
        {
            // Guarded insertion sort: reversed input is the worst case,
            // n(n-1)/2 + (n-1) comparisons (guard + shift scan) and O(n²) writes.
            await Assert.That(stats.CompareCount).IsBetween((ulong)(n - 1), (ulong)(n * (n - 1) / 2 + n));
            await Assert.That(stats.IndexWriteCount).IsBetween((ulong)(n - 1), (ulong)(n * (n + 1) / 2));
            await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        }
        else
        {
            // Single strictly descending run detected with n-1 comparisons,
            // then reversed in place with floor(n/2) swaps (StatisticsContext counts
            // each swap as two element writes).
            await Assert.That(stats.CompareCount).IsEqualTo((ulong)(n - 1));
            await Assert.That(stats.IndexWriteCount).IsEqualTo((ulong)(n / 2 * 2));
            await Assert.That(stats.SwapCount).IsEqualTo((ulong)(n / 2));
        }
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
    }

    [Test]
    [Arguments(10, 42)]
    [Arguments(10, 1234)]
    [Arguments(50, 42)]
    [Arguments(50, 1234)]
    [Arguments(100, 42)]
    [Arguments(100, 1234)]
    [Arguments(1000, 42)]
    [Arguments(1000, 1234)]
    public async Task TheoreticalValuesRandomTest(int n, int seed)
    {
        var stats = new StatisticsContext();
        var random = TestHelpers.ShuffledRange(n, seed);
        Driftsort.Sort(random.AsSpan(), stats);

        // Driftsort for random data:
        // n <= 20: guarded insertion sort, O(n) to O(n²).
        // 20 < n <= 64: eager mode, one or two small-sorts plus a bidirectional merge.
        // n > 64: random input contains no qualifying pre-sorted runs, so unsorted logical
        // runs concatenate into a single block handled by the stable quicksort
        // (~1.0-1.5 n log2(n) comparisons, writes dominated by partition round-trips).
        var logN = Math.Log2(n);
        ulong minCompares, maxCompares, maxWrites;
        if (n <= 20)
        {
            minCompares = (ulong)(n - 1);
            maxCompares = (ulong)(n * (n - 1) / 2 + n);
            maxWrites = (ulong)(n * (n + 1) / 2);
        }
        else
        {
            minCompares = (ulong)(n * logN * 0.5);
            maxCompares = (ulong)(n * logN * 2.5);
            maxWrites = (ulong)(n * logN * 3.0);
        }

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(1UL, maxWrites);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
        // Swaps only come from reversing strictly descending runs, which random data rarely forms.
        await Assert.That(stats.SwapCount < (ulong)n).IsTrue().Because($"SwapCount ({stats.SwapCount}) should be less than n ({n})");
    }

    [Test]
    [Arguments(21, 42)]
    [Arguments(32, 42)]
    [Arguments(33, 42)]
    [Arguments(63, 42)]
    [Arguments(64, 42)]
    [Arguments(65, 42)]
    [Arguments(128, 42)]
    [Arguments(21, 1234)]
    [Arguments(33, 1234)]
    [Arguments(65, 1234)]
    public async Task BoundarySizeCorrectnessTest(int n, int seed)
    {
        // Exercises the mode boundaries: insertion fast path (<=20), eager mode
        // (21..64, one or two small-sorts plus merge), and lazy mode (>=65).
        var stats = new StatisticsContext();
        var array = TestHelpers.ShuffledRange(n, seed);
        var expected = array.ToArray();
        Array.Sort(expected);

        Driftsort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(1000, 4, 42)]
    [Arguments(1000, 4, 1234)]
    [Arguments(1000, 2, 42)]
    [Arguments(2000, 8, 9999)]
    public async Task StabilityLowCardinalityTest(int n, int distinctValues, int seed)
    {
        // Duplicate-heavy input drives the quicksort into the equal-partition path
        // (pivot equal to the left-ancestor pivot). The sort must stay stable there.
        var rng = new Random(seed);
        var items = new StabilityTestItem[n];
        for (var i = 0; i < n; i++)
        {
            items[i] = new StabilityTestItem(rng.Next(distinctValues), i);
        }
        var expected = items.OrderBy(x => x.Value).ToArray(); // OrderBy is a stable sort.

        var stats = new StatisticsContext();
        Driftsort.Sort(items.AsSpan(), stats);

        for (var i = 0; i < n; i++)
        {
            await Assert.That(items[i].Value).IsEqualTo(expected[i].Value);
            await Assert.That(items[i].OriginalIndex).IsEqualTo(expected[i].OriginalIndex);
        }
    }

    [Test]
    [Arguments(500, 42)]
    [Arguments(500, 1234)]
    [Arguments(500, 9999)]
    public async Task StabilityRandomLargeTest(int n, int seed)
    {
        // Random input with a moderate key range: exercises the stable quicksort
        // partition and the powersort merges while checking stability end to end.
        var rng = new Random(seed);
        var items = new StabilityTestItem[n];
        for (var i = 0; i < n; i++)
        {
            items[i] = new StabilityTestItem(rng.Next(50), i);
        }
        var expected = items.OrderBy(x => x.Value).ToArray();

        var stats = new StatisticsContext();
        Driftsort.Sort(items.AsSpan(), stats);

        for (var i = 0; i < n; i++)
        {
            await Assert.That(items[i].Value).IsEqualTo(expected[i].Value);
            await Assert.That(items[i].OriginalIndex).IsEqualTo(expected[i].OriginalIndex);
        }
    }

    [Test]
    [Arguments(100, 42)]
    [Arguments(100, 1234)]
    [Arguments(1000, 42)]
    [Arguments(1000, 1234)]
    [Arguments(10000, 42)]
    [Arguments(10000, 9999)]
    public async Task NullContextIntKernelParityTest(int n, int seed)
    {
        // The fast partition kernels (AVX-512 vpcompressd for int, branchless two-cursor
        // otherwise) only run under NullContext, so the StatisticsContext-based suite never
        // exercises them. Parity-check the public NullContext API against Array.Sort across
        // patterns that hit both the normal partition (< pivot) and the equal-partition pass
        // (<= pivot with pivot-goes-left, triggered by duplicate-heavy input).
        var rng = new Random(seed);
        var patterns = new Dictionary<string, int[]>
        {
            ["shuffled"] = TestHelpers.ShuffledRange(n, seed),
            ["fewDistinct"] = Enumerable.Range(0, n).Select(_ => rng.Next(4)).ToArray(),
            ["someDistinct"] = Enumerable.Range(0, n).Select(_ => rng.Next(64)).ToArray(),
            ["sawtooth"] = Enumerable.Range(0, n).Select(i => i % 100).ToArray(),
            ["negativePositive"] = Enumerable.Range(0, n).Select(_ => rng.Next(int.MinValue, int.MaxValue)).ToArray(),
        };

        foreach (var (name, data) in patterns)
        {
            var expected = data.ToArray();
            Array.Sort(expected);
            var actual = data.ToArray();

            Driftsort.Sort(actual.AsSpan()); // NullContext fast path

            await Assert.That(actual).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"pattern={name}");
        }
    }

    [Test]
    [Arguments(1000, 4, 42)]
    [Arguments(1000, 50, 1234)]
    [Arguments(2000, 8, 9999)]
    public async Task NullContextStabilityBranchlessPathTest(int n, int distinctValues, int seed)
    {
        // StabilityTestItem is a reference type (8-byte element), which routes the NullContext
        // sort through the branchless two-cursor partition kernel. Its write-both trick briefly
        // stores elements in two scratch slots; stability must still hold end to end.
        var rng = new Random(seed);
        var items = new StabilityTestItem[n];
        for (var i = 0; i < n; i++)
        {
            items[i] = new StabilityTestItem(rng.Next(distinctValues), i);
        }
        var expected = items.OrderBy(x => x.Value).ToArray();

        Driftsort.Sort(items.AsSpan()); // NullContext fast path

        for (var i = 0; i < n; i++)
        {
            await Assert.That(items[i].Value).IsEqualTo(expected[i].Value);
            await Assert.That(items[i].OriginalIndex).IsEqualTo(expected[i].OriginalIndex);
        }
    }

    [Test]
    public async Task RangeOverloadSortsOnlySubrangeTest()
    {
        var array = new[] { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0 };
        var stats = new StatisticsContext();

        Driftsort.Sort(array.AsSpan(), 2, 8, stats);

        await Assert.That(array).IsEquivalentTo(new[] { 9, 8, 2, 3, 4, 5, 6, 7, 1, 0 }, CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(200)]
    [Arguments(2000)]
    public async Task PipeOrganMergePathTest(int n)
    {
        // Ascending then descending halves: both are detected as runs (the descending one
        // reversed), forcing the physical merge path rather than the quicksort path.
        // Deterministic input, so the cases vary size (different run thresholds), not seed.
        var array = new int[n];
        var half = n / 2;
        for (var i = 0; i < half; i++) array[i] = i * 2;
        for (var i = half; i < n; i++) array[i] = (n - i) * 2 - 1;
        var expected = array.ToArray();
        Array.Sort(expected);

        var stats = new StatisticsContext();
        Driftsort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
        // Runs were detected and physically merged: far fewer comparisons than a
        // from-scratch sort, and the reversal of the descending run produced swaps.
        await Assert.That(stats.SwapCount > 0).IsTrue().Because($"SwapCount ({stats.SwapCount}) should be > 0 (descending run reversal)");
    }
}
