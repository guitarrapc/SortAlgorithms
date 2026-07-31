using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

// NOTE: MergeInsertionSort (Ford-Johnson) is NOT stable: the Jacobsthal insertion
// order can place a later equal key before an earlier one, so no stability tests here.
[InheritsTests]
public class MergeInsertionSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => MergeInsertionSort.Sort(span, context);

    // Recursive pairing with buffer copies makes large inputs slow.
    protected override int MaxOrderTestSize => 512;

    // Reads all n elements upfront and writes all n elements back at the end, even for sorted input.
    protected override CountExpectation SortedInputWrites => CountExpectation.NonZero;
    // MergeInsertionSort moves elements via buffer writes, never swaps.
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    [MethodDataSource(typeof(MockRandomData), nameof(MockRandomData.Generate))]
    public async Task SortNoStatistics(IInputSample<int> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 512, "Skip large inputs for no stats test");

        var array = inputSample.Samples.ToArray();

        MergeInsertionSort.Sort(array.AsSpan());

        // Check is sorted
        for (var i = 0; i < array.Length - 1; i++)
        {
            await Assert.That(array[i]).IsLessThanOrEqualTo(array[i + 1]);
        }
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
        MergeInsertionSort.Sort(sorted.AsSpan(), stats);

        // Ford-Johnson comparison count is near-optimal for all inputs:
        // approximately n⌈log₂ n⌉ - 2^⌈log₂ n⌉ + 1, close to ⌈log₂(n!)⌉
        ulong minCompares = (ulong)(n - 1);
        ulong maxCompares = (ulong)(3 * n * Math.Max(1, Math.Log(n, 2)));

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        // IndexReadCount: n from initial CopyTo (BUFFER_MAIN source) + 2*compareCount from Compare(int,int) + n from write-back reads (BUFFER_TEMP) + chain reads
        // IndexWriteCount: n from initial CopyTo (BUFFER_TEMP dest) + n from write-back writes (BUFFER_MAIN) + chain writes (BUFFER_CHAIN) >= 2n
        await Assert.That(stats.IndexReadCount).IsGreaterThanOrEqualTo(2 * (ulong)n);
        await Assert.That(stats.IndexWriteCount).IsGreaterThanOrEqualTo(2 * (ulong)n);
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
        MergeInsertionSort.Sort(reversed.AsSpan(), stats);

        ulong minCompares = (ulong)(n - 1);
        ulong maxCompares = (ulong)(n * n);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        // IndexReadCount: n from initial CopyTo (BUFFER_MAIN source) + 2*compareCount from Compare(int,int) + n from write-back reads (BUFFER_TEMP) + chain reads
        // IndexWriteCount: n from initial CopyTo (BUFFER_TEMP dest) + n from write-back writes (BUFFER_MAIN) + chain writes (BUFFER_CHAIN) >= 2n
        await Assert.That(stats.IndexReadCount).IsGreaterThanOrEqualTo(2 * (ulong)n);
        await Assert.That(stats.IndexWriteCount).IsGreaterThanOrEqualTo(2 * (ulong)n);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
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
        MergeInsertionSort.Sort(random.AsSpan(), stats);

        // Ford-Johnson maintains near-optimal comparison count regardless of input order
        ulong minCompares = (ulong)(n - 1);
        ulong maxCompares = (ulong)(n * n);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        // IndexReadCount: n from initial CopyTo (BUFFER_MAIN source) + 2*compareCount from Compare(int,int) + n from write-back reads (BUFFER_TEMP) + chain reads
        // IndexWriteCount: n from initial CopyTo (BUFFER_TEMP dest) + n from write-back writes (BUFFER_MAIN) + chain writes (BUFFER_CHAIN) >= 2n
        await Assert.That(stats.IndexReadCount).IsGreaterThanOrEqualTo(2 * (ulong)n);
        await Assert.That(stats.IndexWriteCount).IsGreaterThanOrEqualTo(2 * (ulong)n);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    /// <summary>
    /// F(n) = sum(k=1..n) ceil(log2(3k/4)), the Ford-Johnson worst-case comparison count (OEIS A001768).
    /// F(1..10) = 0, 1, 3, 5, 7, 10, 13, 16, 19, 22.
    /// </summary>
    private static ulong FordJohnsonBound(int n)
    {
        ulong total = 0;
        for (var k = 1; k <= n; k++)
        {
            var t = 0;
            // smallest t with 2^t >= 3k/4
            while ((4L << t) < 3L * k) t++;
            total += (ulong)t;
        }
        return total;
    }

    private static ulong CompareCountOf(int[] data)
    {
        var stats = new StatisticsContext();
        var work = (int[])data.Clone();
        MergeInsertionSort.Sort(work.AsSpan(), stats);
        return stats.CompareCount;
    }

    /// <summary>
    /// The whole point of Ford-Johnson is the comparison count, so pin it exactly rather than
    /// with a loose big-O bound. Exhaustive over every permutation: the maximum must equal F(n).
    /// An off-by-one in the Jacobsthal grouping still sorts correctly but overshoots F(n) here.
    /// </summary>
    [Test]
    [Arguments(2)]
    [Arguments(3)]
    [Arguments(4)]
    [Arguments(5)]
    [Arguments(6)]
    [Arguments(7)]
    [Arguments(8)]
    public async Task WorstCaseComparisonCountEqualsFordJohnsonBound(int n)
    {
        ulong worst = 0;
        Permute(Enumerable.Range(0, n).ToArray(), 0, p =>
        {
            var c = CompareCountOf(p);
            if (c > worst) worst = c;
        });

        await Assert.That(worst).IsEqualTo(FordJohnsonBound(n));

        static void Permute(int[] a, int k, Action<int[]> visit)
        {
            if (k == a.Length) { visit(a); return; }
            for (var i = k; i < a.Length; i++)
            {
                (a[k], a[i]) = (a[i], a[k]);
                Permute(a, k + 1, visit);
                (a[k], a[i]) = (a[i], a[k]);
            }
        }
    }

    /// <summary>
    /// Above the exhaustive range, F(n) must still hold as a ceiling for every input shape.
    /// </summary>
    [Test]
    [Arguments(17)]
    [Arguments(21)]
    [Arguments(64)]
    [Arguments(100)]
    [Arguments(129)]
    [Arguments(256)]
    public async Task ComparisonCountNeverExceedsFordJohnsonBound(int n)
    {
        var bound = FordJohnsonBound(n);

        await Assert.That(CompareCountOf([.. Enumerable.Range(0, n)])).IsLessThanOrEqualTo(bound);
        await Assert.That(CompareCountOf([.. Enumerable.Range(0, n).Reverse()])).IsLessThanOrEqualTo(bound);
        await Assert.That(CompareCountOf([.. Enumerable.Repeat(42, n)])).IsLessThanOrEqualTo(bound);

        foreach (var seed in (int[])[42, 1234, 9999, 20250731])
        {
            await Assert.That(CompareCountOf(TestHelpers.ShuffledRange(n, seed))).IsLessThanOrEqualTo(bound);
        }
    }

    [Test]
    [Arguments(5)]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    public async Task TheoreticalValuesSameElementsTest(int n)
    {
        var stats = new StatisticsContext();
        var sameValues = Enumerable.Repeat(42, n).ToArray();
        MergeInsertionSort.Sort(sameValues.AsSpan(), stats);

        // Ford-Johnson compares equal elements identically to distinct elements
        ulong minCompares = (ulong)(n - 1);
        ulong maxCompares = (ulong)(n * Math.Max(1, (int)Math.Log(n, 2)) * 3);

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);

        // Verify all values remain correct
        foreach (var item in sameValues) await Assert.That(item).IsEqualTo(42);

        // IndexReadCount: n from initial CopyTo (BUFFER_MAIN source) + 2*compareCount from Compare(int,int) + n from write-back reads (BUFFER_TEMP) + chain reads
        // IndexWriteCount: n from initial CopyTo (BUFFER_TEMP dest) + n from write-back writes (BUFFER_MAIN) + chain writes (BUFFER_CHAIN) >= 2n
        await Assert.That(stats.IndexReadCount).IsGreaterThanOrEqualTo(2 * (ulong)n);
        await Assert.That(stats.IndexWriteCount).IsGreaterThanOrEqualTo(2 * (ulong)n);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }
}
