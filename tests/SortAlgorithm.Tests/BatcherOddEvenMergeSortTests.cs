using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class BatcherOddEvenMergeSortTests
{
    [Test]
    [MethodDataSource(typeof(MockRandomData), nameof(MockRandomData.Generate))]
    [MethodDataSource(typeof(MockPowerOfTwoRandomData), nameof(MockPowerOfTwoRandomData.Generate))]
    [MethodDataSource(typeof(MockPowerOfTwoNegativePositiveRandomData), nameof(MockPowerOfTwoNegativePositiveRandomData.Generate))]
    [MethodDataSource(typeof(MockPowerOfTwoReversedData), nameof(MockPowerOfTwoReversedData.Generate))]
    [MethodDataSource(typeof(MockPowerOfTwoNearlySortedData), nameof(MockPowerOfTwoNearlySortedData.Generate))]
    [MethodDataSource(typeof(MockPowerOfTwoSameValuesData), nameof(MockPowerOfTwoSameValuesData.Generate))]
    public async Task SortResultOrderTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        BatcherOddEvenMergeSort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    public async Task EmptyArray()
    {
        var stats = new StatisticsContext();
        var array = Array.Empty<int>();
        BatcherOddEvenMergeSort.Sort(array.AsSpan(), stats);
        await Assert.That(array).IsEmpty();
    }

    [Test]
    public async Task SingleElement()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 42 };
        BatcherOddEvenMergeSort.Sort(array.AsSpan(), stats);
        await Assert.That(array).IsSingleElement();
        await Assert.That(array[0]).IsEqualTo(42);
    }

    [Test]
    public async Task TwoElements()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 2, 1 };
        BatcherOddEvenMergeSort.Sort(array.AsSpan(), stats);
        await Assert.That(array).IsEquivalentTo([1, 2], CollectionOrdering.Matching);
    }

    [Test]
    public async Task TwoElementsAlreadySorted()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 1, 2 };
        BatcherOddEvenMergeSort.Sort(array.AsSpan(), stats);
        await Assert.That(array).IsEquivalentTo([1, 2], CollectionOrdering.Matching);
    }

    [Test]
    public async Task ThreeElements()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 3, 1, 2 };
        BatcherOddEvenMergeSort.Sort(array.AsSpan(), stats);
        await Assert.That(array).IsEquivalentTo([1, 2, 3], CollectionOrdering.Matching);
    }

    [Test]
    public async Task FourElements()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 3, 1, 4, 2 };
        BatcherOddEvenMergeSort.Sort(array.AsSpan(), stats);
        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4], CollectionOrdering.Matching);
    }

    [Test]
    public async Task EightElements()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 5, 2, 8, 1, 9, 3, 7, 4 };
        BatcherOddEvenMergeSort.Sort(array.AsSpan(), stats);
        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task FiveElementsNonPowerOfTwo()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 5, 3, 1, 4, 2 };
        BatcherOddEvenMergeSort.Sort(array.AsSpan(), stats);
        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5], CollectionOrdering.Matching);
    }

    [Test]
    public async Task SevenElementsNonPowerOfTwo()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 3, 1, 4, 1, 5, 9, 2 };
        BatcherOddEvenMergeSort.Sort(array.AsSpan(), stats);
        await Assert.That(array).IsEquivalentTo([1, 1, 2, 3, 4, 5, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task AllSameValues()
    {
        var stats = new StatisticsContext();
        var array = Enumerable.Repeat(42, 16).ToArray();
        BatcherOddEvenMergeSort.Sort(array.AsSpan(), stats);
        foreach (var item in array) await Assert.That(item).IsEqualTo(42);
    }

    [Test]
    public async Task AlreadySorted()
    {
        var stats = new StatisticsContext();
        var array = Enumerable.Range(1, 8).ToArray();
        BatcherOddEvenMergeSort.Sort(array.AsSpan(), stats);
        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8], CollectionOrdering.Matching);
    }

    [Test]
    public async Task ReverseSorted()
    {
        var stats = new StatisticsContext();
        var array = Enumerable.Range(1, 8).Reverse().ToArray();
        BatcherOddEvenMergeSort.Sort(array.AsSpan(), stats);
        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 6, 7, 8], CollectionOrdering.Matching);
    }

    [Test]
    public async Task StatisticsTracked()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 4, 3, 2, 1 };
        BatcherOddEvenMergeSort.Sort(array.AsSpan(), stats);

        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4], CollectionOrdering.Matching);
        await Assert.That(stats.CompareCount > 0).IsTrue();
        await Assert.That(stats.IndexReadCount > 0).IsTrue();
    }

    [Test]
    [MethodDataSource(typeof(MockPowerOfTwoSortedData), nameof(MockPowerOfTwoSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BatcherOddEvenMergeSort.Sort(array.AsSpan(), stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);

        // Batcher Odd-Even Merge Sort has O(n log^2 n) comparisons regardless of input
        await Assert.That(stats.CompareCount > 0).IsTrue();
        await Assert.That(stats.IndexReadCount > 0).IsTrue();
    }

    [Test]
    [Arguments(2)]
    [Arguments(4)]
    [Arguments(8)]
    [Arguments(16)]
    [Arguments(32)]
    [Arguments(64)]
    [Arguments(128)]
    public async Task TheoreticalValuesSortedTest(int n)
    {
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, n).ToArray();
        BatcherOddEvenMergeSort.Sort(sorted.AsSpan(), stats);

        // Batcher Odd-Even Merge Sort is data-oblivious: comparison count is independent of input order
        // For n = 2^k: C(n) = n * k * (k-1) / 4 + n - 1, where k = log2(n)
        var expectedCompares = CalculateBatcherOddEvenMergeComparisons(n);

        // Reads: each comparison reads 2 elements, each swap also reads 2 elements
        var expectedReads = expectedCompares * 2 + stats.SwapCount * 2;
        var expectedWrites = stats.SwapCount * 2;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount >= 0).IsTrue();
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
    }

    [Test]
    [Arguments(2)]
    [Arguments(4)]
    [Arguments(8)]
    [Arguments(16)]
    [Arguments(32)]
    [Arguments(64)]
    [Arguments(128)]
    public async Task TheoreticalValuesReversedTest(int n)
    {
        var stats = new StatisticsContext();
        var reversed = Enumerable.Range(0, n).Reverse().ToArray();
        BatcherOddEvenMergeSort.Sort(reversed.AsSpan(), stats);

        // Batcher Odd-Even Merge Sort is data-oblivious: same comparison count regardless of input order
        var expectedCompares = CalculateBatcherOddEvenMergeComparisons(n);

        var expectedReads = expectedCompares * 2 + stats.SwapCount * 2;
        var expectedWrites = stats.SwapCount * 2;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount > 0).IsTrue().Because("Reversed array should require swaps");
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
    }

    [Test]
    [Arguments(2, 42)]
    [Arguments(2, 1234)]
    [Arguments(4, 42)]
    [Arguments(4, 1234)]
    [Arguments(8, 42)]
    [Arguments(8, 1234)]
    [Arguments(16, 42)]
    [Arguments(16, 1234)]
    [Arguments(32, 42)]
    [Arguments(32, 1234)]
    [Arguments(64, 42)]
    [Arguments(64, 1234)]
    [Arguments(128, 42)]
    [Arguments(128, 1234)]
    public async Task TheoreticalValuesRandomTest(int n, int seed)
    {
        var stats = new StatisticsContext();
        var random = TestHelpers.ShuffledRange(n, seed);
        BatcherOddEvenMergeSort.Sort(random.AsSpan(), stats);

        // Batcher Odd-Even Merge Sort always performs the same number of comparisons regardless of input
        var expectedCompares = CalculateBatcherOddEvenMergeComparisons(n);

        var expectedReads = expectedCompares * 2 + stats.SwapCount * 2;
        var expectedWrites = stats.SwapCount * 2;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount >= 0).IsTrue();
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
    }

    public static IEnumerable<int> SequenceSizes() => [.. Enumerable.Range(0, 301), 511, 512, 513, 1000, 1023, 1024, 1025, 4097];

    /// <summary>
    /// The phases announced, the comparators run, their order and their orientation are exactly those of the
    /// textbook enumeration: for p = 1, 2, 4, ...; k = p, p/2, ..., 1; j = k mod p, stepping 2k; i = 0, ..., k - 1,
    /// compare (i + j, i + j + k) when both lie inside the array and in the same block of size 2p. The reference
    /// below transcribes that definition with its per-pair division check and 64-bit arithmetic, independently of
    /// how the implementation enumerates the same pairs.
    /// </summary>
    [Test]
    [MethodDataSource(nameof(SequenceSizes))]
    public async Task EventSequenceMatchesTextbookEnumerationTest(int n)
    {
        var recorder = new NetworkEventRecorder();
        BatcherOddEvenMergeSort.Sort(TestHelpers.ShuffledRange(n, 42).AsSpan(), recorder);

        var expected = new List<string>();
        if (n > 1)
        {
            for (long p = 1; p < n; p *= 2)
            {
                expected.Add($"pass {p} {n}");
                for (var k = p; k >= 1; k /= 2)
                {
                    expected.Add($"stage {k} {p} {n}");
                    for (var j = k % p; j + k < n; j += 2 * k)
                    {
                        for (long i = 0; i < k; i++)
                        {
                            var left = i + j;
                            var right = left + k;
                            if (right < n && left / (2 * p) == right / (2 * p))
                                expected.Add($"compare {left} {right}");
                        }
                    }
                }
            }
        }

        await Assert.That(recorder.Events).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    /// <summary>
    /// The steps that end the pass loop and the group loops are the largest values the loop control computes, and for
    /// counts above 2^30 they reach 2^31, past int.MaxValue. Sorting that many elements is too slow for a test, so this
    /// replays the loop bounds in 64-bit arithmetic and checks that each helper returns the exact step instead of a
    /// wrapped value: the step after every pass, and the step from each stage's last group start below count - k
    /// (earlier group steps of the same stage are smaller).
    /// </summary>
    [Test]
    [Arguments((1 << 30) + 1)]
    [Arguments(int.MaxValue - 1)]
    [Arguments(int.MaxValue)]
    public async Task LargeCountIndexArithmeticDoesNotWrapTest(int count)
    {
        var wrapped = new List<string>();
        long largestStep = 0;
        for (long p = 1; p < count; p *= 2)
        {
            long nextPass = BatcherOddEvenMergeSort.NextPass((uint)p);
            if (nextPass != 2 * p) wrapped.Add($"NextPass({p}) = {nextPass}");
            largestStep = Math.Max(largestStep, nextPass);

            for (var k = p; k >= 1; k /= 2)
            {
                var rightLimit = count - k;
                var last = k % p;
                if (last >= rightLimit) continue;
                last += (rightLimit - 1 - last) / (2 * k) * (2 * k);

                long nextGroup = BatcherOddEvenMergeSort.NextGroup((uint)last, (uint)k);
                if (nextGroup != last + 2 * k) wrapped.Add($"NextGroup({last}, {k}) = {nextGroup}");
                largestStep = Math.Max(largestStep, nextGroup);
            }
        }

        await Assert.That(wrapped).IsEmpty();
        await Assert.That(largestStep).IsEqualTo(1L << 31);
    }

    /// <summary>
    /// Records phase announcements and comparisons, in the order they happen, as text.
    /// </summary>
    sealed class NetworkEventRecorder : ISortContext
    {
        public List<string> Events { get; } = [];

        public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0)
            => Events.Add(phase switch
            {
                SortPhase.OddEvenMergeSortPass => $"pass {param1} {param2}",
                SortPhase.OddEvenMergeSortStage => $"stage {param1} {param2} {param3}",
                _ => $"unexpected {phase}",
            });

        public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) => Events.Add($"compare {i} {j}");
        public void OnSwap(int i, int j, int bufferId) { }
        public void OnIndexRead(int index, int bufferId) { }
        public void OnIndexWrite(int index, int bufferId) { }
        public void OnIndexWrite<T>(int index, int bufferId, T value) { }
        public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId) { }
        public void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values) { }
        public void OnRole(int index, int bufferId, RoleType role) { }
    }

    /// <summary>
    /// Calculates the theoretical number of comparisons for Batcher's Odd-Even Merge Sort.
    /// For n = 2^k, the formula is: n * k * (k-1) / 4 + n - 1, where k = log2(n).
    /// This comes from the recursive merge structure:
    /// - The odd-even merge of two sequences of size n/2 uses fewer comparators than Bitonic Sort
    /// - For n=2: 1, n=4: 5, n=8: 19, n=16: 63, n=32: 191
    /// </summary>
    static ulong CalculateBatcherOddEvenMergeComparisons(int n)
    {
        if (n <= 1) return 0;

        var k = 0;
        var temp = n;
        while (temp > 1)
        {
            temp >>= 1;
            k++;
        }

        // Formula: n * k * (k-1) / 4 + n - 1
        return (ulong)(n * k * (k - 1) / 4 + n - 1);
    }
}
