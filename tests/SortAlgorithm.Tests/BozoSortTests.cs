using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class BozoSortTests
{
    [Test, SkipCI]
    [MethodDataSource(typeof(MockJokeSortData), nameof(MockJokeSortData.Generate))]
    public async Task SortResultOrderTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();

        BozoSort.Sort(array.AsSpan(), stats);

        Array.Sort(inputSample.Samples);
        await Assert.That(array).IsEquivalentTo(inputSample.Samples, CollectionOrdering.Matching);
    }

    [Test, SkipCI]
    [MethodDataSource(typeof(MockJokeSortData), nameof(MockJokeSortData.GenerateSorted))]
    public async Task StatisticsSortedTest(IInputSample<int> inputSample)
    {
        var stats = new StatisticsContext();
        var array = inputSample.Samples.ToArray();
        BozoSort.Sort(array.AsSpan(), stats);

        await Assert.That((ulong)array.Length).IsEqualTo((ulong)inputSample.Samples.Length);
        await Assert.That(stats.IndexReadCount).IsNotEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.CompareCount).IsNotEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    [Test, SkipCI]
    public async Task TheoreticalValuesSortedTest()
    {
        // Sorted input: one successful check (n-1 comparisons, 2 reads each) and no transposition.
        var stats = new StatisticsContext();
        var sorted = Enumerable.Range(0, 5).ToArray();
        BozoSort.Sort(sorted.AsSpan(), stats);

        var n = sorted.Length;
        await Assert.That(stats.CompareCount).IsEqualTo((ulong)(n - 1));
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo((ulong)(2 * (n - 1)));
    }

    [Test, SkipCI]
    [Arguments(0)]
    [Arguments(1)]
    public async Task TrivialLengthTest(int n)
    {
        // Spans of length 0 and 1 are already sorted and must not be touched or checked.
        var stats = new StatisticsContext();
        var recorder = new BozoRecorder();
        var array = Enumerable.Range(0, n).ToArray();
        BozoSort.Sort(array.AsSpan(), new CompositeContext(stats, recorder));

        await Assert.That(stats.CompareCount).IsEqualTo(0UL);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo(0UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(0UL);
        await Assert.That(recorder.Events).IsEmpty();
    }

    [Test, SkipCI]
    [Arguments(0u)]
    [Arguments(1u)]
    [Arguments(42u)]
    [Arguments(uint.MaxValue)]
    public async Task TheoreticalValuesTwoElementsTest(uint seed)
    {
        // With two elements the only pair of distinct positions is {0, 1}, so every seed gives:
        // failed check (1 comparison) -> one swap -> successful check (1 comparison).
        // A reading that allowed i = j could waste iterations here; bozo-sort+ cannot.
        var stats = new StatisticsContext();
        var array = new[] { 1, 0 };
        BozoSort.Sort(array.AsSpan(), Comparer<int>.Default, stats, seed);

        await Assert.That(array).IsEquivalentTo([0, 1], CollectionOrdering.Matching);
        await Assert.That(stats.CompareCount).IsEqualTo(2UL);
        await Assert.That(stats.SwapCount).IsEqualTo(1UL);
        await Assert.That(stats.IndexWriteCount).IsEqualTo(2UL);
        await Assert.That(stats.IndexReadCount).IsEqualTo(6UL);
    }

    [Test, SkipCI]
    [Arguments(3, 42)]
    [Arguments(3, 1234)]
    [Arguments(5, 42)]
    [Arguments(5, 1234)]
    [Arguments(7, 42)]
    [Arguments(7, 1234)]
    public async Task TheoreticalValuesRandomTest(int n, int seed)
    {
        // Structural invariants that hold for every run, whatever the random choices:
        // - each transposition is announced once, numbered 1, 2, ..., immediately before its swap, with the same positions
        // - the two positions are distinct and in range
        // - each swap is preceded by one failed check (1..n-1 comparisons) and the run ends with one successful check (n-1)
        // - elements move only by swapping: 2 writes and 2 reads per swap, 2 reads per comparison
        var stats = new StatisticsContext();
        var recorder = new BozoRecorder();
        var array = TestHelpers.ShuffledRange(n, seed);
        BozoSort.Sort(array.AsSpan(), new CompositeContext(stats, recorder));

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, n), CollectionOrdering.Matching);

        var swaps = stats.SwapCount;
        await Assert.That((ulong)recorder.Events.Count).IsEqualTo(2 * swaps);
        for (var k = 0; k < recorder.Events.Count; k += 2)
        {
            var phase = recorder.Events[k];
            var swap = recorder.Events[k + 1];
            await Assert.That(phase.Kind).IsEqualTo(EventKind.Phase);
            await Assert.That(swap.Kind).IsEqualTo(EventKind.Swap);
            await Assert.That(phase.Attempt).IsEqualTo(k / 2 + 1);
            await Assert.That((phase.I, phase.J)).IsEqualTo((swap.I, swap.J));
            await Assert.That(swap.I).IsNotEqualTo(swap.J);
            await Assert.That(swap.I >= 0 && swap.I < n && swap.J >= 0 && swap.J < n).IsTrue();
        }

        var lowerCompares = swaps + (ulong)(n - 1);
        var upperCompares = (swaps + 1) * (ulong)(n - 1);
        await Assert.That(stats.CompareCount >= lowerCompares && stats.CompareCount <= upperCompares).IsTrue()
            .Because($"CompareCount ({stats.CompareCount}) should be within [{lowerCompares}, {upperCompares}] for {swaps} swaps");
        await Assert.That(stats.IndexWriteCount).IsEqualTo(2 * swaps);
        await Assert.That(stats.IndexReadCount).IsEqualTo(2 * stats.CompareCount + 2 * swaps);
    }

    [Test, SkipCI]
    [Arguments("Sorted", 42u)]
    [Arguments("OneTransposition", 42u)]
    [Arguments("OneTransposition", 1234u)]
    [Arguments("TwoTranspositions", 42u)]
    [Arguments("TwoTranspositions", 1234u)]
    [Arguments("ThreeCycle", 42u)]
    [Arguments("ThreeCycle", 1234u)]
    [Arguments("FourCycle", 42u)]
    [Arguments("FourCycle", 1234u)]
    [Arguments("FiveCycle", 42u)]
    [Arguments("FiveCycle", 1234u)]
    [Arguments("Reversed", 42u)]
    [Arguments("Reversed", 1234u)]
    public async Task SwapParityAndLowerBoundTest(string shape, uint seed)
    {
        // For distinct keys each transposition flips the permutation's parity and the sorted arrangement is even,
        // so the swap count has the input's parity. A permutation with c cycles also needs at least n - c
        // transpositions. Both classes (even and odd inputs) are covered: an i = j "swap" would break the parity.
        int[] input = shape switch
        {
            "Sorted" => [0, 1, 2, 3, 4],            // 5 cycles: 0 swaps, even
            "OneTransposition" => [1, 0, 2, 3, 4],  // 4 cycles: >= 1, odd
            "TwoTranspositions" => [1, 0, 3, 2, 4], // 3 cycles: >= 2, even
            "ThreeCycle" => [1, 2, 0, 3, 4],        // 3 cycles: >= 2, even
            "FourCycle" => [1, 2, 3, 0, 4],         // 2 cycles: >= 3, odd
            "FiveCycle" => [1, 2, 3, 4, 0],         // 1 cycle: >= 4, even
            "Reversed" => [4, 3, 2, 1, 0],          // 3 cycles: >= 2, even
            _ => throw new ArgumentOutOfRangeException(nameof(shape)),
        };
        var minimumSwaps = (ulong)(input.Length - CountCycles(input));

        var stats = new StatisticsContext();
        var array = input.ToArray();
        BozoSort.Sort(array.AsSpan(), Comparer<int>.Default, stats, seed);

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, input.Length), CollectionOrdering.Matching);
        await Assert.That(stats.SwapCount >= minimumSwaps).IsTrue()
            .Because($"SwapCount ({stats.SwapCount}) should be at least n - cycles ({minimumSwaps})");
        await Assert.That(stats.SwapCount % 2).IsEqualTo(minimumSwaps % 2);
    }

    [Test, SkipCI]
    public async Task UniformPairTest()
    {
        // The transposed pair is drawn uniformly from the n(n-1)/2 pairs of distinct positions.
        // Deterministic seeds give a fixed sample of several thousand draws; every pair must sit near the mean.
        const int n = 5;
        var counts = new int[n, n];
        var draws = 0;
        for (var seed = 1u; seed <= 60u; seed++)
        {
            var recorder = new BozoRecorder();
            var array = Enumerable.Range(0, n).Reverse().ToArray();
            BozoSort.Sort(array.AsSpan(), Comparer<int>.Default, recorder, seed * 2654435761u);
            foreach (var e in recorder.Events)
            {
                if (e.Kind != EventKind.Swap) continue;
                counts[Math.Min(e.I, e.J), Math.Max(e.I, e.J)]++;
                draws++;
            }
        }

        var mean = draws / (n * (n - 1) / 2.0);
        await Assert.That(draws > 5000).IsTrue().Because($"draws ({draws}) should be large enough to compare pair frequencies");
        for (var i = 0; i < n; i++)
        {
            for (var j = i + 1; j < n; j++)
            {
                var count = counts[i, j];
                await Assert.That(Math.Abs(count - mean) <= mean * 0.15).IsTrue()
                    .Because($"pair ({i}, {j}) was drawn {count} times; mean is {mean:F1}");
            }
        }
    }

    [Test, SkipCI]
    public async Task AverageSwapsTest()
    {
        // Gruber, Holzer and Ruepp (Theorem 11, after Flatto, Odlyzko and Wales): the expected number of swaps
        // on a uniformly random input is n! + 2(n-2)! + o((n-2)!). For n = 5 that is 132 plus a small term;
        // solving the 120-state Markov chain exactly gives 131.25. Every permutation of 5 is used equally often.
        const int n = 5;
        const double leadingTerms = 120 + 2 * 6;
        double total = 0;
        var runs = 0;
        foreach (var permutation in Permutations(n))
        {
            for (var seed = 1u; seed <= 20u; seed++)
            {
                var stats = new StatisticsContext();
                var array = permutation.ToArray();
                BozoSort.Sort(array.AsSpan(), Comparer<int>.Default, stats, seed * 2654435761u);
                total += stats.SwapCount;
                runs++;
            }
        }

        var average = total / runs;
        await Assert.That(Math.Abs(average - leadingTerms) <= leadingTerms * 0.1).IsTrue()
            .Because($"average swaps ({average:F2} over {runs} runs) should be within 10% of n! + 2(n-2)! = {leadingTerms}");
    }

    [Test, SkipCI]
    public async Task SeedTest()
    {
        // The same seed replays the same transpositions; 0 selects the default seed; another seed gives another walk.
        static List<BozoEvent> Run(uint? seed)
        {
            var recorder = new BozoRecorder();
            var array = Enumerable.Range(0, 6).Reverse().ToArray();
            if (seed is { } s)
                BozoSort.Sort(array.AsSpan(), Comparer<int>.Default, recorder, s);
            else
                BozoSort.Sort(array.AsSpan(), Comparer<int>.Default, recorder);
            return recorder.Events;
        }

        var defaultRun = Run(null);
        await Assert.That(Run(null)).IsEquivalentTo(defaultRun, CollectionOrdering.Matching);
        await Assert.That(Run(0u)).IsEquivalentTo(defaultRun, CollectionOrdering.Matching);
        await Assert.That(Run(42u)).IsEquivalentTo(Run(42u), CollectionOrdering.Matching);
        await Assert.That(Run(42u).SequenceEqual(Run(1234u))).IsFalse();
    }

    [Test, SkipCI]
    public async Task CustomComparerTest()
    {
        // The comparer defines the order: a descending comparer yields descending output.
        var stats = new StatisticsContext();
        var array = TestHelpers.ShuffledRange(6, 42);
        var descending = Comparer<int>.Create((x, y) => y.CompareTo(x));
        BozoSort.Sort(array.AsSpan(), descending, stats);

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, 6).Reverse(), CollectionOrdering.Matching);
        await Assert.That(stats.SwapCount).IsNotEqualTo(0UL);
    }

    [Test, SkipCI]
    public async Task NullContextTest()
    {
        var array = TestHelpers.ShuffledRange(6, 1234);
        BozoSort.Sort(array.AsSpan());

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, 6), CollectionOrdering.Matching);
    }

    [Test, SkipCI]
    public async Task ReferenceTypeTest()
    {
        var array = new[] { "delta", "bravo", "echo", "alpha", "charlie" };
        BozoSort.Sort(array.AsSpan(), new StatisticsContext());

        await Assert.That(array).IsEquivalentTo(["alpha", "bravo", "charlie", "delta", "echo"], CollectionOrdering.Matching);
    }

    [Test, SkipCI]
    public async Task UnstableForEqualKeysTest()
    {
        // IsStable is false: a random transposition can move an element past an equal key.
        // Across seeds the two key-1 records must sometimes come out in reversed order, and always sorted by key.
        var byKey = Comparer<(int Key, int Id)>.Create((x, y) => x.Key.CompareTo(y.Key));
        var reversedRuns = 0;
        for (var seed = 1u; seed <= 50u; seed++)
        {
            (int Key, int Id)[] array = [(1, 0), (1, 1), (0, 2)];
            BozoSort.Sort(array.AsSpan(), byKey, NullContext.Default, seed * 2654435761u);

            await Assert.That(array.Select(x => x.Key)).IsEquivalentTo([0, 1, 1], CollectionOrdering.Matching);
            if (array[1].Id == 1 && array[2].Id == 0) reversedRuns++;
        }

        await Assert.That(reversedRuns).IsGreaterThan(0);
    }

    static int CountCycles(int[] permutation)
    {
        var seen = new bool[permutation.Length];
        var cycles = 0;
        for (var start = 0; start < permutation.Length; start++)
        {
            if (seen[start]) continue;
            cycles++;
            for (var i = start; !seen[i]; i = permutation[i]) seen[i] = true;
        }
        return cycles;
    }

    static IEnumerable<int[]> Permutations(int n)
    {
        var current = Enumerable.Range(0, n).ToArray();
        return Permute(current, 0);

        static IEnumerable<int[]> Permute(int[] p, int k)
        {
            if (k == p.Length)
            {
                yield return p.ToArray();
                yield break;
            }
            for (var i = k; i < p.Length; i++)
            {
                (p[k], p[i]) = (p[i], p[k]);
                foreach (var result in Permute(p, k + 1)) yield return result;
                (p[k], p[i]) = (p[i], p[k]);
            }
        }
    }

    enum EventKind { Phase, Swap }

    readonly record struct BozoEvent(EventKind Kind, int Attempt, int I, int J);

    /// <summary>
    /// Records BozoSwap announcements and swaps in the order they happen.
    /// </summary>
    sealed class BozoRecorder : ISortContext
    {
        public List<BozoEvent> Events { get; } = [];

        public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0)
        {
            if (phase == SortPhase.BozoSwap) Events.Add(new BozoEvent(EventKind.Phase, param1, param2, param3));
        }

        public void OnSwap(int i, int j, int bufferId) => Events.Add(new BozoEvent(EventKind.Swap, 0, i, j));

        public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) { }
        public void OnIndexRead(int index, int bufferId) { }
        public void OnIndexWrite(int index, int bufferId) { }
        public void OnIndexWrite<T>(int index, int bufferId, T value) { }
        public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId) { }
        public void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values) { }
        public void OnRole(int index, int bufferId, RoleType role) { }
    }
}
