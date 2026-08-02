using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

/// <summary>
/// Verifies that <see cref="PdqAdversaryGenerator"/> produces an input that is actually adversarial
/// for <see cref="PDQSort"/>, and not merely an unusual-looking permutation.
///
/// <para>
/// The distinction matters because PDQSort adapts. A layout drawn to poison one partition step is
/// recovered from by the shuffle and the pattern detection, so "looks scrambled" and "costs PDQSort
/// more than random input" are unrelated properties. Only the second one is what this pattern claims
/// to provide, so that is what is asserted here: the fallback to heapsort must fire, and the
/// comparison count must be far above random input of the same size.
/// </para>
///
/// <para>
/// O(n^2) is deliberately not asserted. PDQSort's heapsort fallback caps the worst case at
/// O(n log n); an adversary that reached quadratic behavior would be evidence of a bug in PDQSort,
/// not a better adversary.
/// </para>
/// </summary>
public class PdqAdversaryGeneratorTests
{
    /// <summary>Counts comparisons and notes whether PDQSort gave up and switched to heapsort.</summary>
    private sealed class FallbackObservingContext : ISortContext
    {
        public ulong Compares { get; private set; }
        public int HeapSortFallbacks { get; private set; }

        public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) => Compares++;

        public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0)
        {
            if (phase is SortPhase.HybridToHeapSort)
                HeapSortFallbacks++;
        }

        public void OnSwap(int i, int j, int bufferId) { }
        public void OnIndexRead(int index, int bufferId) { }
        public void OnIndexWrite(int index, int bufferId) { }
        public void OnIndexWrite<T>(int index, int bufferId, T value) { }
        public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId) { }
        public void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values) { }
        public void OnRole(int index, int bufferId, RoleType role) { }
    }

    private static FallbackObservingContext RunPdqSort(int[] input)
    {
        var context = new FallbackObservingContext();
        var work = (int[])input.Clone();
        PDQSort.Sort(work.AsSpan(), context);

        // Statistics only describe a real run, so confirm the run actually sorted.
        var expected = (int[])input.Clone();
        Array.Sort(expected);
        if (!work.SequenceEqual(expected))
            throw new InvalidOperationException("PDQSort did not sort the adversary input.");

        return context;
    }

    private static int[] Shuffled(int size, int seed)
    {
        var random = new Random(seed);
        var array = Enumerable.Range(0, size).ToArray();
        for (var i = array.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
        return array;
    }

    // Sizes straddle every branch PDQSort takes on size: below the insertion-sort threshold (24),
    // just above it, either side of the ninther threshold (128), and large enough to recurse.
    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(3)]
    [Arguments(23)]
    [Arguments(24)]
    [Arguments(25)]
    [Arguments(127)]
    [Arguments(128)]
    [Arguments(129)]
    [Arguments(1000)]
    [Arguments(10000)]
    public async Task GenerateProducesPermutationOfRange(int size)
    {
        var generated = ArrayPatterns.GeneratePdqSortAdversary(size);

        await Assert.That(generated.Length).IsEqualTo(size);
        await Assert.That(generated.Order()).IsEquivalentTo(Enumerable.Range(0, size));
    }

    /// <summary>
    /// The adversary decides values while PDQSort runs, so determinism is a property of the
    /// generator rather than an obvious consequence of its shape. Consumers that cache a pattern
    /// or replay a recorded run depend on it.
    /// </summary>
    [Test]
    [Arguments(100)]
    [Arguments(5000)]
    public async Task GenerateIsDeterministic(int size)
    {
        var first = ArrayPatterns.GeneratePdqSortAdversary(size);
        var second = ArrayPatterns.GeneratePdqSortAdversary(size);

        await Assert.That(second).IsEquivalentTo(first);
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    public async Task GenerateRejectsNonPositiveLength(int size)
    {
        await Assert.That(() => ArrayPatterns.GeneratePdqSortAdversary(size))
            .Throws<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// The defining property: PDQSort must exhaust its bad-partition budget and fall back to
    /// heapsort. Random input of the same size must not, which is what makes this a worst case
    /// rather than just another permutation.
    /// </summary>
    [Test]
    [Arguments(1000)]
    [Arguments(10000)]
    public async Task AdversaryDrivesPdqSortIntoItsHeapSortFallback(int size)
    {
        var adversarial = RunPdqSort(ArrayPatterns.GeneratePdqSortAdversary(size));
        var random = RunPdqSort(Shuffled(size, seed: 20260802));

        await Assert.That(adversarial.HeapSortFallbacks).IsGreaterThan(0);
        await Assert.That(random.HeapSortFallbacks).IsEqualTo(0);
    }

    /// <summary>
    /// Measured at n = 100,000 the adversary costs ~2.8 n log2 n comparisons against ~1.1 for
    /// random input. The 2x floor asserted here is well below that, so it fails on a regression of
    /// the generator rather than on ordinary drift in PDQSort.
    /// </summary>
    [Test]
    [Arguments(1000)]
    [Arguments(10000)]
    public async Task AdversaryCostsAtLeastTwiceTheComparisonsOfRandomInput(int size)
    {
        var adversarial = RunPdqSort(ArrayPatterns.GeneratePdqSortAdversary(size));
        var random = RunPdqSort(Shuffled(size, seed: 20260802));

        await Assert.That(adversarial.Compares).IsGreaterThan(random.Compares * 2);
    }
}
