using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

/// <summary>
/// Verifies the phase contract the distribution sorts publish through <see cref="ISortContext.OnPhase"/>.
///
/// <para>
/// An observer cannot tell which distribution sort it is watching from index operations alone: counting,
/// pigeonhole and bucket sort all read the input, write into an auxiliary buffer and write back, so the
/// phase events are the only self-describing signal for which stage of the family is running. A consumer
/// that has to infer the stage from operation shapes ends up classifying by heuristics that break when an
/// unrelated detail changes.
/// </para>
///
/// <para>
/// The key-selector and integer overloads of one algorithm must publish the same phase sequence. They are
/// the same algorithm and differ only in how the key is obtained, so a consumer that renders one and not
/// the other is reacting to an implementation detail. <see cref="BucketSortInteger"/> used to omit
/// <see cref="SortPhase.DistributionCount"/> while <see cref="BucketSort"/> emitted it, which is exactly
/// the asymmetry these tests exist to prevent.
/// </para>
/// </summary>
public class DistributionPhaseEventTests
{
    /// <summary>Records the distribution phases in the order they are announced, ignoring everything else.</summary>
    private sealed class PhaseRecordingContext : ISortContext
    {
        public List<SortPhase> Phases { get; } = [];

        public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0)
        {
            // Only the distribution family is under test; nested helpers (InsertionSort inside a bucket)
            // announce their own phases and must not be mistaken for a stage of the outer algorithm.
            if (phase is SortPhase.DistributionCount or SortPhase.DistributionAccumulate or SortPhase.DistributionWrite)
                Phases.Add(phase);
        }

        public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) { }
        public void OnSwap(int i, int j, int bufferId) { }
        public void OnIndexRead(int index, int bufferId) { }
        public void OnIndexWrite(int index, int bufferId) { }
        public void OnIndexWrite<T>(int index, int bufferId, T value) { }
        public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId) { }
        public void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values) { }
        public void OnRole(int index, int bufferId, RoleType role) { }
    }

    // Duplicate-heavy and unsorted, so no algorithm can take an early-exit path that skips its phases.
    private static int[] Sample() => [3, 1, 2, 1, 3, 2, 1];

    private static List<SortPhase> Record(Action<int[], PhaseRecordingContext> sort)
    {
        var context = new PhaseRecordingContext();
        var array = Sample();
        sort(array, context);

        // A phase sequence only describes a real run, so confirm the run actually sorted.
        var expected = Sample();
        Array.Sort(expected);
        if (!array.SequenceEqual(expected))
            throw new InvalidOperationException($"sort produced [{string.Join(", ", array)}]");

        return context.Phases;
    }

    /// <summary>
    /// Counting sort turns counts into offsets, so it announces all three stages.
    /// </summary>
    [Test]
    public async Task CountingSortAnnouncesCountAccumulateWrite()
    {
        SortPhase[] expected = [SortPhase.DistributionCount, SortPhase.DistributionAccumulate, SortPhase.DistributionWrite];

        await Assert.That(Record((a, c) => CountingSort.SortBy(a.AsSpan(), x => x, c)))
            .IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Assert.That(Record((a, c) => CountingSortInteger.Sort(a.AsSpan(), c)))
            .IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    /// <summary>
    /// Bucket sort computes each bucket's start offset, so it announces all three stages.
    /// </summary>
    [Test]
    public async Task BucketSortAnnouncesCountAccumulateWrite()
    {
        SortPhase[] expected = [SortPhase.DistributionCount, SortPhase.DistributionAccumulate, SortPhase.DistributionWrite];

        await Assert.That(Record((a, c) => BucketSort.SortBy(a.AsSpan(), x => x, c)))
            .IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Assert.That(Record((a, c) => BucketSortInteger.Sort(a.AsSpan(), c)))
            .IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    /// <summary>
    /// Pigeonhole sort places elements into holes directly, so there is no offset to accumulate and it
    /// announces only two stages. This absence is the algorithm's defining difference from counting sort.
    /// </summary>
    [Test]
    public async Task PigeonholeSortAnnouncesCountWriteWithoutAccumulate()
    {
        SortPhase[] expected = [SortPhase.DistributionCount, SortPhase.DistributionWrite];

        await Assert.That(Record((a, c) => PigeonholeSort.SortBy(a.AsSpan(), x => x, c)))
            .IsEquivalentTo(expected, CollectionOrdering.Matching);
        await Assert.That(Record((a, c) => PigeonholeSortInteger.Sort(a.AsSpan(), c)))
            .IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    /// <summary>
    /// An input whose keys are all equal is rejected before any distribution work happens, so no phase is
    /// announced at all. A consumer that assumes at least one phase per run would mis-render this case.
    /// </summary>
    [Test]
    public async Task AllEqualInputAnnouncesNoDistributionPhase()
    {
        static List<SortPhase> RecordAllEqual(Action<int[], PhaseRecordingContext> sort)
        {
            var context = new PhaseRecordingContext();
            sort([7, 7, 7, 7], context);
            return context.Phases;
        }

        await Assert.That(RecordAllEqual((a, c) => CountingSortInteger.Sort(a.AsSpan(), c))).IsEmpty();
        await Assert.That(RecordAllEqual((a, c) => PigeonholeSortInteger.Sort(a.AsSpan(), c))).IsEmpty();
        await Assert.That(RecordAllEqual((a, c) => BucketSortInteger.Sort(a.AsSpan(), c))).IsEmpty();
    }
}
