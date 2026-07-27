using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class IpnsortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => Ipnsort.Sort(span, context);

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
        Ipnsort.Sort(sorted.AsSpan(), stats);

        // ipnsort sorts fully ascending input with exactly n-1 comparisons and no writes:
        // n <= 20 uses the guarded insertion fast path, larger inputs detect a single
        // ascending run covering the whole array and return immediately.
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
        Ipnsort.Sort(reversed.AsSpan(), stats);

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
    [Arguments(10)]
    [Arguments(50)]
    [Arguments(100)]
    [Arguments(1000)]
    public async Task TheoreticalValuesAllEqualTest(int n)
    {
        var stats = new StatisticsContext();
        var allEqual = Enumerable.Repeat(42, n).ToArray();
        Ipnsort.Sort(allEqual.AsSpan(), stats);

        // All-equal input is a fully ascending (non-strict) run: exactly n-1 comparisons,
        // no writes, regardless of the code path (insertion guard or run detection).
        await Assert.That(stats.CompareCount).IsEqualTo((ulong)(n - 1));
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
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
        Ipnsort.Sort(random.AsSpan(), stats);

        // ipnsort for random data:
        // n <= 20: guarded insertion sort, O(n) to O(n²).
        // n > 20: quicksort with pseudo-median pivots down to 32-element partitions handled
        // by the network small sort; ~1.0-1.5 n log2(n) comparisons overall, writes dominated
        // by the 2-moves-per-element branchless partition plus small-sort merges.
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
            maxCompares = (ulong)(n * logN * 3.0);
            maxWrites = (ulong)(n * logN * 4.0);
        }

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(1UL, maxWrites);
        await Assert.That(stats.IndexReadCount > 0).IsTrue().Because($"IndexReadCount ({stats.IndexReadCount}) should be > 0");
    }

    [Test]
    [Arguments(21, 42)]
    [Arguments(32, 42)]
    [Arguments(33, 42)]
    [Arguments(48, 42)]
    [Arguments(64, 42)]
    [Arguments(65, 42)]
    [Arguments(128, 42)]
    [Arguments(21, 1234)]
    [Arguments(33, 1234)]
    [Arguments(65, 1234)]
    public async Task BoundarySizeCorrectnessTest(int n, int seed)
    {
        // Exercises the mode boundaries: insertion fast path (<=20), a single small sort
        // (21..32), and quicksort recursion over small-sort leaves (>=33).
        var stats = new StatisticsContext();
        var array = TestHelpers.ShuffledRange(n, seed);
        var expected = array.ToArray();
        Array.Sort(expected);

        Ipnsort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(1000, 2, 42)]
    [Arguments(1000, 4, 1234)]
    [Arguments(2000, 8, 9999)]
    [Arguments(2000, 64, 42)]
    public async Task DuplicateHeavyEqualPartitionTest(int n, int distinctValues, int seed)
    {
        // Duplicate-heavy input drives the quicksort into the equal-partition path
        // (pivot equal to the left-ancestor pivot, elements <= pivot partitioned out).
        var rng = new Random(seed);
        var array = new int[n];
        for (var i = 0; i < n; i++)
        {
            array[i] = rng.Next(distinctValues);
        }
        var expected = array.ToArray();
        Array.Sort(expected);

        var stats = new StatisticsContext();
        Ipnsort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo(expected, CollectionOrdering.Matching);
        // The equal-partition pass removes whole duplicate blocks from recursion, so the
        // comparison count stays well below a plain n log2(n) quicksort on such input.
        var maxCompares = (ulong)(n * Math.Log2(n) * 1.5);
        await Assert.That(stats.CompareCount).IsBetween(1UL, maxCompares);
    }

    [Test]
    public async Task RangeSortTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        // Sort only the range [2, 6) -> indices 2, 3, 4, 5
        Ipnsort.Sort(array.AsSpan(), 2, 6, stats);

        await Assert.That(array).IsEquivalentTo([5, 3, 1, 2, 8, 9, 7, 4, 6], CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortFullArrayTest()
    {
        var stats = new StatisticsContext();
        var array = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };

        Ipnsort.Sort(array.AsSpan(), 0, array.Length, stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(100, 42)]
    [Arguments(100, 1234)]
    [Arguments(1000, 42)]
    [Arguments(1000, 1234)]
    [Arguments(10000, 42)]
    [Arguments(10000, 9999)]
    public async Task NullContextParityTest(int n, int seed)
    {
        // The NullContext fast path uses IEEE-style primitive comparisons and the JIT-specialized
        // kernels; parity-check the public NullContext API against Array.Sort across patterns
        // that hit the normal partition, the equal-partition pass, run detection, and the
        // network small sort.
        var rng = new Random(seed);
        var patterns = new Dictionary<string, int[]>
        {
            ["shuffled"] = TestHelpers.ShuffledRange(n, seed),
            ["fewDistinct"] = Enumerable.Range(0, n).Select(_ => rng.Next(4)).ToArray(),
            ["someDistinct"] = Enumerable.Range(0, n).Select(_ => rng.Next(64)).ToArray(),
            ["sawtooth"] = Enumerable.Range(0, n).Select(i => i % 100).ToArray(),
            ["negativePositive"] = Enumerable.Range(0, n).Select(_ => rng.Next(int.MinValue, int.MaxValue)).ToArray(),
            ["pipeOrgan"] = Enumerable.Range(0, n).Select(i => Math.Min(i, n - i)).ToArray(),
        };

        foreach (var (name, data) in patterns)
        {
            var expected = data.ToArray();
            Array.Sort(expected);
            var actual = data.ToArray();

            Ipnsort.Sort(actual.AsSpan()); // NullContext fast path

            await Assert.That(actual).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"pattern={name}");
        }
    }

    /// <summary>
    /// 32-byte element: routes the partition through the branchless Lomuto kernel and the
    /// small sort through the general (stable 4/8 network) path instead of the integer network.
    /// </summary>
    private struct MidStruct : IComparable<MidStruct>
    {
        public int Key;
#pragma warning disable CS0169 // padding fields exist only to control the element size
        private long pad1, pad2, pad3;
#pragma warning restore CS0169

        public MidStruct(int key) => Key = key;
        public readonly int CompareTo(MidStruct other) => Key.CompareTo(other.Key);
    }

    /// <summary>
    /// 136-byte element (> 96-byte branchless limit, > 4096-byte scratch budget at 48 elements):
    /// routes the partition through the branchy Hoare kernel and the small sort through the
    /// scratch-free insertion fallback.
    /// </summary>
    private struct BigStruct : IComparable<BigStruct>
    {
        public int Key;
#pragma warning disable CS0169 // padding fields exist only to control the element size
        private long pad01, pad02, pad03, pad04, pad05, pad06, pad07, pad08;
        private long pad09, pad10, pad11, pad12, pad13, pad14, pad15, pad16;
#pragma warning restore CS0169

        public BigStruct(int key) => Key = key;
        public readonly int CompareTo(BigStruct other) => Key.CompareTo(other.Key);
    }

    [Test]
    [Arguments(500, 42)]
    [Arguments(1000, 1234)]
    public async Task MidSizeElementGeneralSmallSortTest(int n, int seed)
    {
        var rng = new Random(seed);
        var array = new MidStruct[n];
        for (var i = 0; i < n; i++)
        {
            array[i] = new MidStruct(rng.Next(n / 4)); // include duplicates
        }
        var expectedKeys = array.Select(x => x.Key).OrderBy(x => x).ToArray();

        var stats = new StatisticsContext();
        Ipnsort.Sort(array.AsSpan(), stats);

        await Assert.That(array.Select(x => x.Key)).IsEquivalentTo(expectedKeys, CollectionOrdering.Matching);
    }

    [Test]
    [Arguments(500, 42)]
    [Arguments(1000, 1234)]
    public async Task LargeElementHoarePartitionTest(int n, int seed)
    {
        var rng = new Random(seed);
        var array = new BigStruct[n];
        for (var i = 0; i < n; i++)
        {
            array[i] = new BigStruct(rng.Next(n / 4)); // include duplicates
        }
        var expectedKeys = array.Select(x => x.Key).OrderBy(x => x).ToArray();

        var stats = new StatisticsContext();
        Ipnsort.Sort(array.AsSpan(), stats);

        await Assert.That(array.Select(x => x.Key)).IsEquivalentTo(expectedKeys, CollectionOrdering.Matching);

        // NullContext path takes the same kernels for this element size; parity-check it too.
        var array2 = new BigStruct[n];
        var rng2 = new Random(seed);
        for (var i = 0; i < n; i++)
        {
            array2[i] = new BigStruct(rng2.Next(n / 4));
        }
        Ipnsort.Sort(array2.AsSpan());
        await Assert.That(array2.Select(x => x.Key)).IsEquivalentTo(expectedKeys, CollectionOrdering.Matching);
    }
}
