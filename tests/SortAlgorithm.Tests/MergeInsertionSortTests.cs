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
