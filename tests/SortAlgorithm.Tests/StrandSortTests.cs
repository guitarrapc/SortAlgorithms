using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class StrandSortTests
{
    [Test]
    [MethodDataSource(typeof(MockRandomData), nameof(MockRandomData.Generate))]
    [MethodDataSource(typeof(MockNegativePositiveRandomData), nameof(MockNegativePositiveRandomData.Generate))]
    [MethodDataSource(typeof(MockNegativeRandomData), nameof(MockNegativeRandomData.Generate))]
    [MethodDataSource(typeof(MockReversedData), nameof(MockReversedData.Generate))]
    [MethodDataSource(typeof(MockReversedWithDuplicatesData), nameof(MockReversedWithDuplicatesData.Generate))]
    [MethodDataSource(typeof(MockPipeorganData), nameof(MockPipeorganData.Generate))]
    [MethodDataSource(typeof(MockNearlySortedData), nameof(MockNearlySortedData.Generate))]
    [MethodDataSource(typeof(MockAllSameData), nameof(MockAllSameData.Generate))]
    [MethodDataSource(typeof(MockSameValuesData), nameof(MockSameValuesData.Generate))]
    [MethodDataSource(typeof(MockQuickSortWorstCaseData), nameof(MockQuickSortWorstCaseData.Generate))]
    [MethodDataSource(typeof(MockTwoDistinctValuesData), nameof(MockTwoDistinctValuesData.Generate))]
    [MethodDataSource(typeof(MockHalfZeroHalfOneData), nameof(MockHalfZeroHalfOneData.Generate))]
    [MethodDataSource(typeof(MockValleyRandomData), nameof(MockValleyRandomData.Generate))]
    [MethodDataSource(typeof(MockHighlySkewedData), nameof(MockHighlySkewedData.Generate))]
    public async Task SortResultOrderTest(IInputSample<int> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 512, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        StrandSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateHalf))]
    public async Task SortHalfResultOrderTest(IInputSample<Half> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 512, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        StrandSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateFloat))]
    public async Task SortFloatResultOrderTest(IInputSample<float> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 512, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        StrandSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockNanRandomData), nameof(MockNanRandomData.GenerateDouble))]
    public async Task SortDoubleResultOrderTest(IInputSample<double> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 512, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        StrandSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockIntKeyRandomData), nameof(MockIntKeyRandomData.Generate))]
    public async Task SortIntStructResultOrderTest(IInputSample<Utils.IntKey> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 512, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        StrandSort.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    [MethodDataSource(typeof(MockSortedData), nameof(MockSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        Skip.When(inputSample.Samples.Length > 512, "Skip large inputs for order test");

        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        StrandSort.Sort(array.AsSpan(), stats);

        var n = (ulong)array.Length;
        await Assert.That(n).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        if (inputSample.Samples.Length <= 1)
        {
            await Assert.That(stats.CompareCount).IsEqualTo(0UL);
            await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        }
        else
        {
            // Sorted input: all elements form one strand (1 pass)
            // Extraction compares: n-1 (each element >= strand tail, no merge compare needed)
            await Assert.That(stats.CompareCount).IsEqualTo(n - 1);
            // 3 write phases: s→remaining (n) + extraction→strand (n) + merge→s (n)
            await Assert.That(stats.IndexWriteCount).IsEqualTo(3 * n);
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
        StrandSort.Sort(sorted.AsSpan(), stats);

        // Sorted input: best case O(n) — all n elements form a single strand in one pass.
        //
        // Comparisons (n-1 total):
        // - Extraction: n-1 (each element i=1..n-1 compares against strand tail → always appended)
        // - Merge: 0 (result is empty, inner loop never runs)
        var expectedCompares = (ulong)(n - 1);

        // Writes (3n total):
        // - Initial s→remaining copy:    n writes (BUFFER_REMAINING)
        // - Extraction to strand buffer: n writes (BUFFER_STRAND)
        // - Merge trailing-strand loop:  n writes (BUFFER_MAIN)
        var expectedWrites = (ulong)(3 * n);

        // Reads (4n-1 total):
        // - Initial s copy:              n reads (BUFFER_MAIN)
        // - Extraction remaining reads:  n reads (BUFFER_REMAINING, one per element)
        // - Extraction strand-tail reads: n-1 reads (BUFFER_STRAND, one per compare)
        // - Merge trailing-strand reads: n reads (BUFFER_STRAND)
        var expectedReads = (ulong)(4 * n - 1);

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
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
        StrandSort.Sort(reversed.AsSpan(), stats);

        // Reversed input: worst case O(n²) — n passes, each extracting a strand of length 1.
        // Pass k starts with n-k+1 remaining elements; remaining[0] is always the pass maximum,
        // so all other elements are smaller and stay in the remaining pool.
        //
        // Comparisons: (n-1)(n+2)/2 total
        // - Extraction: Σ(k=1..n)(n-k) = n(n-1)/2  (pass k scans n-k elements against strand tail)
        // - Merge:      n-1               (pass 2..n each fires 1 compare: new strand < result[0])
        var expectedCompares = (ulong)((n - 1) * (n + 2) / 2);

        // Writes: 3n(n+1)/2 total
        // - Initial s→remaining copy:               n       writes (BUFFER_REMAINING)
        // - Extraction to strand (1 per pass):       n       writes (BUFFER_STRAND)
        // - Extraction compaction to remaining:      n(n-1)/2 writes (BUFFER_REMAINING)
        // - Merge writes to s (k elements in pass k): n(n+1)/2 writes (BUFFER_MAIN)
        // - Non-final copy s→result (k in pass k):   n(n-1)/2 writes (BUFFER_RESULT)
        var expectedWrites = (ulong)(3 * n * (n + 1) / 2);

        // Reads: 2n²+2n-1 total
        // - Initial s copy:                          n       reads (BUFFER_MAIN)
        // - Extraction per pass k (remaining + tail): Σ(2(n-k)+1) = n²  reads
        // - Merge reads per pass (strand re-read + result including double-read of result[0]):
        //     1 + Σ(k=2..n)(k+1) = (n+1)(n+2)/2-2  reads
        // - Non-final copy s→result (k reads in pass k): n(n-1)/2 reads (BUFFER_MAIN)
        var expectedReads = (ulong)(2 * n * n + 2 * n - 1);

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    [Test]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task TheoreticalValuesRandomTest(int n)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        StrandSort.Sort(random.AsSpan(), stats);

        // Random permutation: between best case (1 pass, sorted) and worst case (n passes, reversed).
        // All bounds are inclusive.
        var minCompares = (ulong)(n - 1);                    // sorted: n-1
        var maxCompares = (ulong)((n - 1) * (n + 2) / 2);   // reversed: (n-1)(n+2)/2

        var minWrites = (ulong)(3 * n);                      // sorted: 3n
        var maxWrites = (ulong)(3 * n * (n + 1) / 2);        // reversed: 3n(n+1)/2

        var minReads = (ulong)(4 * n - 1);                   // sorted: 4n-1
        var maxReads = (ulong)(2 * n * n + 2 * n - 1);       // reversed: 2n²+2n-1

        await Assert.That(stats.CompareCount).IsBetween(minCompares, maxCompares);
        await Assert.That(stats.IndexWriteCount).IsBetween(minWrites, maxWrites);
        await Assert.That(stats.IndexReadCount).IsBetween(minReads, maxReads);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }
}
