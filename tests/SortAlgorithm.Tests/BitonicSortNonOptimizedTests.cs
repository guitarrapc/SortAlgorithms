using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class BitonicSortNonOptimizedTests
{
    [Test]
    [MethodDataSource(typeof(MockPowerOfTwoRandomData), nameof(MockPowerOfTwoRandomData.Generate))]
    [MethodDataSource(typeof(MockPowerOfTwoNegativePositiveRandomData), nameof(MockPowerOfTwoNegativePositiveRandomData.Generate))]
    [MethodDataSource(typeof(MockPowerOfTwoReversedData), nameof(MockPowerOfTwoReversedData.Generate))]
    [MethodDataSource(typeof(MockPowerOfTwoNearlySortedData), nameof(MockPowerOfTwoNearlySortedData.Generate))]
    [MethodDataSource(typeof(MockPowerOfTwoSameValuesData), nameof(MockPowerOfTwoSameValuesData.Generate))]
    public async Task SortResultOrderTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        BitonicSortNonOptimized.Sort(array.AsSpan(), stats);

        // Check is sorted
        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test]
    public async Task ThrowsOnNonPowerOfTwo()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 3, 1, 4, 1, 5, 9, 2 }; // Length 7 is not power of 2

        Assert.Throws<ArgumentException>(() => BitonicSortNonOptimized.Sort(array.AsSpan(), stats));
    }

    [Test]
    public async Task EmptyArray()
    {
        var stats = new StatisticsContext();
        var array = Array.Empty<int>();
        BitonicSortNonOptimized.Sort(array.AsSpan(), stats);
        await Assert.That(array).IsEmpty();
    }

    [Test]
    public async Task SingleElement()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 42 };
        BitonicSortNonOptimized.Sort(array.AsSpan(), stats);
        await Assert.That(array).IsSingleElement();
        await Assert.That(array[0]).IsEqualTo(42);
    }

    [Test]
    public async Task TwoElements()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 2, 1 };
        BitonicSortNonOptimized.Sort(array.AsSpan(), stats);
        await Assert.That(array.Length).IsEqualTo(2);
        await Assert.That(array[0]).IsEqualTo(1);
        await Assert.That(array[1]).IsEqualTo(2);
    }

    [Test]
    public async Task FourElements()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 3, 1, 4, 2 };
        BitonicSortNonOptimized.Sort(array.AsSpan(), stats);
        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4], CollectionOrdering.Matching);
    }

    [Test]
    public async Task EightElements()
    {
        var stats = new StatisticsContext();
        var array = new int[] { 5, 2, 8, 1, 9, 3, 7, 4 };
        BitonicSortNonOptimized.Sort(array.AsSpan(), stats);
        await Assert.That(array).IsEquivalentTo([1, 2, 3, 4, 5, 7, 8, 9], CollectionOrdering.Matching);
    }

    [Test]
    public async Task SixteenElementsAllSame()
    {
        var stats = new StatisticsContext();
        var array = Enumerable.Repeat(42, 16).ToArray();
        BitonicSortNonOptimized.Sort(array.AsSpan(), stats);
        foreach (var item in array) await Assert.That(item).IsEqualTo(42);
    }


    [Test]
    [MethodDataSource(typeof(MockPowerOfTwoSortedData), nameof(MockPowerOfTwoSortedData.Generate))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BitonicSortNonOptimized.Sort(array.AsSpan(), stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);

        // Bitonic sort has O(n log^2 n) comparisons regardless of input
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
        BitonicSortNonOptimized.Sort(sorted.AsSpan(), stats);

        // Bitonic sort performs the same number of comparisons regardless of input order
        // For n = 2^k, the number of comparisons is (k(k+1)/2) * (n/2) where k = log2(n)
        var expectedCompares = CalculateBitonicComparisons(n);

        // For sorted data, fewer swaps but not necessarily 0 due to data-oblivious nature
        // The bitonic network structure may still swap elements that are already in order
        // Reads: Each comparison reads 2 elements, each swap reads 2 elements
        var expectedReads = expectedCompares * 2 + stats.SwapCount * 2;
        var expectedWrites = stats.SwapCount * 2;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        // Note: BitonicSort may perform some swaps even on sorted data
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
        BitonicSortNonOptimized.Sort(reversed.AsSpan(), stats);

        // Bitonic sort performs the same number of comparisons regardless of input order
        var expectedCompares = CalculateBitonicComparisons(n);

        // For reversed data, many swaps are needed
        // Reads: Compare reads 2 elements, Swap also reads 2 elements before swapping
        // Total reads = (comparisons * 2) + (swaps * 2)
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
        BitonicSortNonOptimized.Sort(random.AsSpan(), stats);

        // Bitonic sort always performs the same number of comparisons regardless of input
        var expectedCompares = CalculateBitonicComparisons(n);

        // For random data, swap count varies based on disorder
        // Reads: Compare reads 2 elements, Swap also reads 2 elements
        // Total reads = (comparisons * 2) + (swaps * 2)
        var expectedReads = expectedCompares * 2 + stats.SwapCount * 2;
        var expectedWrites = stats.SwapCount * 2;

        await Assert.That(stats.CompareCount).IsEqualTo(expectedCompares);
        await Assert.That(stats.SwapCount >= 0).IsTrue();
        await Assert.That(stats.IndexWriteCount).IsEqualTo(expectedWrites);
        await Assert.That(stats.IndexReadCount).IsEqualTo(expectedReads);
    }

    /// <summary>
    /// The recursion runs the same network as <see cref="BitonicSort"/>, depth first instead of layer by layer. Both
    /// orders respect every wire's sequence of comparators, so each comparator meets the same two values in either
    /// order. With distinct keys, the comparison sign and whether the comparator exchanged give its direction, so equal
    /// multisets of (positions, sign, exchange) mean the same comparators with the same directions.
    /// </summary>
    [Test]
    [Arguments(2, 42)]
    [Arguments(8, 42)]
    [Arguments(16, 1234)]
    [Arguments(64, 42)]
    [Arguments(256, 1234)]
    [Arguments(1024, 42)]
    public async Task SameNetworkAsBitonicSortTest(int n, int seed)
    {
        var input = TestHelpers.ShuffledRange(n, seed);

        var recursive = new ComparatorRecorder();
        BitonicSortNonOptimized.Sort(input.ToArray().AsSpan(), recursive);
        var layered = new ComparatorRecorder();
        BitonicSort.Sort(input.ToArray().AsSpan(), layered);

        var expected = layered.Comparators.Order(StringComparer.Ordinal).ToList();
        var actual = recursive.Comparators.Order(StringComparer.Ordinal).ToList();
        await Assert.That(actual).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    /// <summary>
    /// Records every comparator as "lower upper sign exchanged".
    /// </summary>
    sealed class ComparatorRecorder : ISortContext
    {
        public List<string> Comparators { get; } = [];

        public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0) { }
        public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) => Comparators.Add($"{i} {j} {Math.Sign(result)} keep");
        public void OnSwap(int i, int j, int bufferId) => Comparators[^1] = Comparators[^1].Replace(" keep", " swap");
        public void OnIndexRead(int index, int bufferId) { }
        public void OnIndexWrite(int index, int bufferId) { }
        public void OnIndexWrite<T>(int index, int bufferId, T value) { }
        public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId) { }
        public void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values) { }
        public void OnRole(int index, int bufferId, RoleType role) { }
    }

    /// <summary>
    /// Calculates the theoretical number of comparisons for Bitonic Sort.
    /// For n = 2^k, the formula is: n * k * (k+1) / 4, where k = log2(n)
    /// This comes from the recursive structure:
    /// - Total number of comparison stages: log n levels
    /// - At level i, we perform n/2 * i comparisons
    /// - Sum: n/2 * (1 + 2 + 3 + ... + log n) = n/2 * log n * (log n + 1) / 2 = n * log n * (log n + 1) / 4
    /// </summary>
    private static ulong CalculateBitonicComparisons(int n)
    {
        if (n <= 1) return 0;

        // n must be a power of 2 for BitonicSort
        int k = 0;
        int temp = n;
        while (temp > 1)
        {
            temp >>= 1;
            k++;
        }

        // Formula: n * k * (k+1) / 4
        return (ulong)(n * k * (k + 1) / 4);
    }
}
