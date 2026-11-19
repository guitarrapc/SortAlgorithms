using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

/// <summary>
/// The merges of QuadSort and FluxSort and the partitions and insertion sorts of the quicksort family advance their cursors and fill their
/// regions on the strength of earlier comparisons, and Release builds do not check element access. Guard cells surround each range under test. The comparers report every
/// guard cell they are shown, which reveals a read that only feeds a comparison, and the cells must keep their value, which reveals a stray
/// write.
/// </summary>
public class InconsistentComparerSafetyTests
{
    const int GuardLength = 64;
    const long GuardKey = long.MinValue;

    enum Sorter
    {
        Quad, QuadLimitedSwap, Flux, Pdq, PdqBranchless, Intro, IntroDotnet, Std,
        Quick, QuickMedian3, QuickMedian9, Quick3way, Stable, BidirectionalStable, PairInsertion,
    }

    /// <summary>
    /// QuadSort, FluxSort and BidirectionalStableQuickSort may repeat or lose elements under a broken comparer. The others document that the
    /// range keeps its elements.
    /// </summary>
    static bool KeepsElements(Sorter sorter) => sorter is not (Sorter.Quad or Sorter.QuadLimitedSwap or Sorter.Flux or Sorter.BidirectionalStable);

    /// <summary>Thrown by a watching comparer, so that a cursor that has left the range stops before it can leave the buffer too.</summary>
    sealed class GuardCellReadException : Exception;

    sealed class GuardWatch
    {
        public bool SawGuard;
    }

    readonly struct WatchingComparer<T, TInner>(TInner inner, T guard, GuardWatch watch) : IComparer<T>
        where TInner : IComparer<T>
    {
        public int Compare(T? x, T? y)
        {
            if (EqualityComparer<T>.Default.Equals(x, guard) || EqualityComparer<T>.Default.Equals(y, guard))
            {
                watch.SawGuard = true;
                throw new GuardCellReadException();
            }
            return inner.Compare(x, y);
        }
    }

    readonly struct ConstantComparer(int answer) : IComparer<int>
    {
        public int Compare(int x, int y) => answer;
    }

    /// <summary>Keeps giving the same answer and changes it rarely, which runs one cursor far past the elements that should stop it.</summary>
    sealed class StickyIntComparer(Random random) : IComparer<int>
    {
        bool greater = random.Next(2) == 0;

        public int Compare(int x, int y)
        {
            if (random.Next(64) == 0) greater = !greater;
            return greater ? 1 : -1;
        }
    }

    /// <summary>Answers truthfully, then reverses its order part way through the sort, so earlier answers no longer hold.</summary>
    sealed class ReversingComparer(int honestCalls) : IComparer<int>
    {
        int calls;

        public int Compare(int x, int y) => ++calls > honestCalls ? y.CompareTo(x) : x.CompareTo(y);
    }

    /// <summary>Answers truthfully for a number of calls and then gives one answer for good. <see cref="Calls"/> counts every call.</summary>
    sealed class HonestThenConstantComparer(int honestCalls, int answer) : IComparer<int>
    {
        public int Calls;

        public int Compare(int x, int y) => ++Calls > honestCalls ? answer : x.CompareTo(y);
    }

    /// <summary>Answers "equal" for a number of calls and then gives one answer for good.</summary>
    sealed class EqualThenConstantComparer(int equalCalls, int answer) : IComparer<int>
    {
        int calls;

        public int Compare(int x, int y) => ++calls > equalCalls ? answer : 0;
    }

    /// <summary>
    /// Runs a sort on its own thread and returns whether it finished in time and what it threw. A sort that never finishes keeps spinning on
    /// a background thread, which fails the test instead of hanging the run.
    /// </summary>
    static (bool Finished, Exception? Error) RunWithin(Action sort)
    {
        Exception? error = null;
        var thread = new Thread(() =>
        {
            try { sort(); }
            catch (Exception e) { error = e; }
        })
        { IsBackground = true };
        thread.Start();
        var finished = thread.Join(TimeSpan.FromSeconds(30));
        return (finished, error);
    }

    readonly struct OverflowingComparer : IComparer<int>
    {
        public int Compare(int x, int y) => unchecked(x - y);
    }

    /// <summary>IEEE comparison: NaN compares equal to everything, so "equal" is not transitive.</summary>
    readonly struct IeeeComparer : IComparer<double>
    {
        public int Compare(double x, double y) => x < y ? -1 : x > y ? 1 : 0;
    }

    readonly struct CoinFlipComparer(Random random) : IComparer<int>
    {
        public int Compare(int x, int y) => random.Next(3) - 1;
    }

    /// <summary>Answers from a script of "greater" decisions and records how many it gave. Answers past the script's end are "not greater".</summary>
    sealed class ScriptedComparer(bool[] script) : IComparer<long>
    {
        public int Used;
        public bool SawGuard;

        public int Compare(long x, long y)
        {
            if (x == GuardKey || y == GuardKey) SawGuard = true;
            var index = Used++;
            return index < script.Length && script[index] ? 1 : -1;
        }
    }

    /// <summary>Keeps giving the same answer and changes it rarely, which runs one cursor far ahead of what the keys allow.</summary>
    sealed class StickyComparer(Random random) : IComparer<long>
    {
        bool greater = random.Next(2) == 0;
        public bool SawGuard;

        public int Compare(long x, long y)
        {
            if (x == GuardKey || y == GuardKey) SawGuard = true;
            if (random.Next(64) == 0) greater = !greater;
            return greater ? 1 : -1;
        }
    }

    sealed class ReversedComparer : IComparer<long>
    {
        public bool SawGuard;

        public int Compare(long x, long y)
        {
            if (x == GuardKey || y == GuardKey) SawGuard = true;
            return y.CompareTo(x);
        }
    }

    /// <summary>
    /// Sorts [first, last) of the buffer. The range overloads keep the guard cells inside the span, so a cursor below the range reads a
    /// guard cell instead of an address far outside the buffer.
    /// </summary>
    static void Sort<T, TComparer, TContext>(Sorter sorter, T[] buffer, int first, int last, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        switch (sorter)
        {
            case Sorter.Quad: QuadSort.Sort(buffer.AsSpan(), first, last, comparer, context); break;
            case Sorter.QuadLimitedSwap: QuadSort.SortWithSwapSize(buffer.AsSpan(first, last - first), 64, comparer, context); break;
            case Sorter.Flux: FluxSort.Sort(buffer.AsSpan(), first, last, comparer, context); break;
            case Sorter.Pdq: PDQSort.Sort(buffer.AsSpan(), first, last, comparer, context); break;
            case Sorter.PdqBranchless: PDQSortBranchless.Sort(buffer.AsSpan(), first, last, comparer, context); break;
            case Sorter.Intro: IntroSort.Sort(buffer.AsSpan(), first, last, comparer, context); break;
            case Sorter.IntroDotnet: IntroSortDotnet.Sort(buffer.AsSpan(), first, last, comparer, context); break;
            case Sorter.Std: StdSort.Sort(buffer.AsSpan(), first, last, comparer, context); break;
            case Sorter.Quick: QuickSort.Sort(buffer.AsSpan(), first, last, comparer, context); break;
            case Sorter.QuickMedian3: QuickSortMedian3.Sort(buffer.AsSpan(), first, last, comparer, context); break;
            case Sorter.QuickMedian9: QuickSortMedian9.Sort(buffer.AsSpan(), first, last, comparer, context); break;
            case Sorter.Quick3way: QuickSort3way.Sort(buffer.AsSpan(), first, last, comparer, context); break;
            case Sorter.Stable: StableQuickSort.Sort(buffer.AsSpan(), first, last, comparer, context); break;
            case Sorter.BidirectionalStable: BidirectionalStableQuickSort.Sort(buffer.AsSpan(), first, last, comparer, context); break;
            case Sorter.PairInsertion: PairInsertionSort.Sort(buffer.AsSpan(), first, last, comparer, context); break;
            default: throw new ArgumentOutOfRangeException(nameof(sorter));
        }
    }

    /// <summary>
    /// Sorts between guard cells. The sort may finish or report the broken comparer, and either way the range may hold only values of the
    /// input. A sort that documents that the range keeps its elements must also leave it a permutation of the input.
    /// Returns a description of the violation, if any.
    /// </summary>
    static string? Exercise<T, TComparer, TContext>(Sorter sorter, T[] input, T guard, TComparer comparer, TContext context)
        where T : notnull
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var buffer = new T[GuardLength + input.Length + GuardLength];
        Array.Fill(buffer, guard);
        input.CopyTo(buffer, GuardLength);
        var watch = new GuardWatch();

        try
        {
            Sort(sorter, buffer, GuardLength, GuardLength + input.Length, new WatchingComparer<T, TComparer>(comparer, guard, watch), context);
        }
        catch (InvalidOperationException)
        {
            // The documented report of a comparer that is not a consistent total order.
        }
        catch (GuardCellReadException)
        {
            // The watch has recorded the read, and it is reported below.
        }

        if (watch.SawGuard) return "the comparer was shown a guard cell";

        var allowed = new HashSet<T>(input);
        for (var i = 0; i < buffer.Length; i++)
        {
            var inside = i >= GuardLength && i < GuardLength + input.Length;
            if (inside && !allowed.Contains(buffer[i])) return $"index {i - GuardLength} holds {buffer[i]}, which is not an input value";
            if (!inside && !EqualityComparer<T>.Default.Equals(buffer[i], guard)) return $"guard cell {i} was overwritten";
        }

        if (KeepsElements(sorter))
        {
            var expected = input.Order().ToArray();
            var actual = buffer.AsSpan(GuardLength, input.Length).ToArray().Order().ToArray();
            if (!expected.SequenceEqual(actual)) return "the range is no longer a permutation of the input";
        }
        return null;
    }

    /// <summary>
    /// Runs one cross merge of two adjacent runs between guard cells and returns a description of the violation, if any.
    /// <paramref name="sawGuard"/> reads the comparer's own record after the merge.
    /// </summary>
    static string? MergeBetweenGuards<TComparer>(long[] source, long[] output, int left, int right, TComparer comparer, Func<bool> sawGuard)
        where TComparer : IComparer<long>
    {
        var src = new SortSpan<long, TComparer, NullContext>(source, NullContext.Default, comparer, 0);
        var dst = new SortSpan<long, TComparer, NullContext>(output, NullContext.Default, comparer, 1);

        try
        {
            QuadSort.CrossMerge(dst, GuardLength, src, GuardLength, left, right);
        }
        catch (InvalidOperationException)
        {
            // The documented report of a comparer that is not a consistent total order.
        }

        if (sawGuard()) return $"left {left}, right {right}: the comparer was shown a guard cell";
        for (var i = 0; i < GuardLength; i++)
        {
            if (output[i] != GuardKey || output[^(i + 1)] != GuardKey) return $"left {left}, right {right}: an output guard cell was overwritten";
        }
        return null;
    }

    static (long[] Source, long[] Output) MergeBuffers(int left, int right)
    {
        var n = left + right;
        var source = new long[GuardLength + n + GuardLength];
        var output = new long[source.Length];
        Array.Fill(source, GuardKey);
        Array.Fill(output, GuardKey);
        // The two runs interleave, so a comparer that answers truthfully would alternate between them.
        for (var i = 0; i < left; i++) source[GuardLength + i] = 2L * i;
        for (var i = 0; i < right; i++) source[GuardLength + left + i] = 2L * i + 1;
        return (source, output);
    }

    /// <summary>
    /// Follows every distinct sequence of answers: each run extends the script with "not greater", and the next script flips the last
    /// "not greater" answer that the run used. Merges of 17 and 18 elements are the shortest that reach the branchless batches.
    /// </summary>
    static string? WalkEveryAnswerSequence(int left, int right)
    {
        var (source, output) = MergeBuffers(left, right);
        var script = new bool[128];

        while (true)
        {
            var comparer = new ScriptedComparer(script);
            if (MergeBetweenGuards(source, output, left, right, comparer, () => comparer.SawGuard) is { } failure) return failure;

            var last = Math.Min(comparer.Used, script.Length) - 1;
            while (last >= 0 && script[last]) script[last--] = false;
            if (last < 0) return null;
            script[last] = true;
        }
    }

    static string? MergeWithStickyAnswers(int seed)
    {
        var random = new Random(seed);
        var left = random.Next(1, 300);
        var right = random.Next(1, 300);
        var (source, output) = MergeBuffers(left, right);
        var comparer = new StickyComparer(random);
        return MergeBetweenGuards(source, output, left, right, comparer, () => comparer.SawGuard);
    }

    static string? MergeWithReversedAnswers(int left, int right)
    {
        var (source, output) = MergeBuffers(left, right);
        var comparer = new ReversedComparer();
        return MergeBetweenGuards(source, output, left, right, comparer, () => comparer.SawGuard);
    }

    static string? MergeWithScript(int left, int right, string answers)
    {
        var (source, output) = MergeBuffers(left, right);
        var comparer = new ScriptedComparer(answers.Select(c => c == '1').ToArray());
        return MergeBetweenGuards(source, output, left, right, comparer, () => comparer.SawGuard);
    }

    static int[] FullRangeInts(int n, int seed)
    {
        var random = new Random(seed);
        return Enumerable.Range(0, n).Select(_ => (int)random.NextInt64(int.MinValue + 1L, int.MaxValue + 1L)).ToArray();
    }

    static double[] DoublesWithNaN(int n, int seed)
    {
        var random = new Random(seed);
        return Enumerable.Range(0, n).Select(_ => random.Next(5) switch { 0 => double.NaN, var k => (double)(k - 1) }).ToArray();
    }

    public static IEnumerable<string> Sorters() => Enum.GetNames<Sorter>();

    public static IEnumerable<(string Sorter, int N)> Cases()
    {
        foreach (var sorter in Enum.GetNames<Sorter>())
        {
            foreach (var n in new[] { 7, 8, 31, 64, 133, 1000, 5000 })
            {
                yield return (sorter, n);
            }
        }
    }

    [Test]
    [MethodDataSource(nameof(Cases))]
    public async Task ReadsAndWritesStayInsideTheSpan(string sorterName, int n)
    {
        var sorter = Enum.Parse<Sorter>(sorterName);

        for (var seed = 1; seed <= 40; seed++)
        {
            var label = $"{sorter} n={n} seed={seed}";
            var ints = FullRangeInts(n, seed);
            var doubles = DoublesWithNaN(n, seed);

            await Assert.That(Exercise(sorter, ints, int.MinValue, new OverflowingComparer(), NullContext.Default)).IsNull().Because($"{label} overflow");
            await Assert.That(Exercise(sorter, ints, int.MinValue, new OverflowingComparer(), new StatisticsContext())).IsNull().Because($"{label} overflow observed");
            await Assert.That(Exercise(sorter, ints, int.MinValue, new CoinFlipComparer(new Random(seed)), NullContext.Default)).IsNull().Because($"{label} coin flip");
            await Assert.That(Exercise(sorter, doubles, 12345.5, new IeeeComparer(), NullContext.Default)).IsNull().Because($"{label} NaN");
        }
    }

    /// <summary>
    /// A constant answer drives every unguarded scan to one end of its range, and a sticky one does so part way through a sort. The
    /// reversing comparer is honest until the sort has come to rely on its answers.
    /// </summary>
    [Test]
    [MethodDataSource(nameof(Cases))]
    public async Task CursorsStayInsideTheRangeWhateverTheAnswers(string sorterName, int n)
    {
        var sorter = Enum.Parse<Sorter>(sorterName);
        var ints = FullRangeInts(n, n);

        await Assert.That(Exercise(sorter, ints, int.MinValue, new ConstantComparer(-1), NullContext.Default)).IsNull().Because($"{sorter} n={n} always less");
        await Assert.That(Exercise(sorter, ints, int.MinValue, new ConstantComparer(1), NullContext.Default)).IsNull().Because($"{sorter} n={n} always greater");

        for (var seed = 1; seed <= 40; seed++)
        {
            var label = $"{sorter} n={n} seed={seed}";
            await Assert.That(Exercise(sorter, ints, int.MinValue, new StickyIntComparer(new Random(seed)), NullContext.Default)).IsNull().Because($"{label} sticky");
            await Assert.That(Exercise(sorter, ints, int.MinValue, new ReversingComparer(seed * n / 8), NullContext.Default)).IsNull().Because($"{label} reversing");
        }
    }

    /// <summary>
    /// Turns to a constant answer after every possible number of truthful calls, so each scan is stranded at each point it can reach. A
    /// truthful sort of each of these inputs reaches the PDQSort partition that puts equal elements on the left, and between them they reach
    /// both forms of its first scan from the left. A comparer has to answer truthfully to get there.
    /// </summary>
    [Test]
    [MethodDataSource(nameof(Sorters))]
    public async Task ConstantAnswersFromAnyPointStayInsideTheRange(string sorterName)
    {
        var sorter = Enum.Parse<Sorter>(sorterName);

        foreach (var (n, distinct) in new[] { (133, 4), (100, 8), (200, 3) })
        {
            var random = new Random(n);
            var keys = Enumerable.Range(0, n).Select(_ => random.Next(distinct)).ToArray();
            var honest = new HonestThenConstantComparer(int.MaxValue, 0);
            Sort(sorter, keys.ToArray(), 0, n, honest, NullContext.Default);

            for (var k = 0; k <= honest.Calls; k++)
            {
                foreach (var answer in new[] { -1, 1 })
                {
                    await Assert.That(Exercise(sorter, keys, int.MinValue, new HonestThenConstantComparer(k, answer), NullContext.Default))
                        .IsNull().Because($"{sorter} n={n} after {k} calls always {answer}");
                }
            }
        }
    }

    /// <summary>
    /// A comparer that calls every element less than every other puts all of them, the pivot included, on the less side, so the partition is
    /// no smaller than its range. The sort must report the comparer instead of partitioning the same range again forever.
    /// </summary>
    [Test]
    public async Task BidirectionalPartitionThatSeparatesNothingStops()
    {
        var data = Enumerable.Range(0, 100).ToArray();
        var (finished, error) = RunWithin(() => BidirectionalStableQuickSort.Sort(data.AsSpan(), new ConstantComparer(-1), NullContext.Default));

        await Assert.That(finished).IsTrue();
        await Assert.That(error).IsTypeOf<InvalidOperationException>();
    }

    /// <summary>
    /// The first pass finds every element equal to the pivot and the second finds none, which leaves the gap for the equal elements unfilled.
    /// The scratch starts with a marker that no input holds, so copying that gap back would put the marker into the range. Sweeping the
    /// switch point reaches this case whatever number of comparisons pivot selection makes.
    /// </summary>
    [Test]
    public async Task BidirectionalPartitionCopiesNoUnfilledGap()
    {
        const int marker = -12345;
        for (var k = 0; k <= 64; k++)
        {
            var data = Enumerable.Range(1, 8).ToArray();
            var scratch = Enumerable.Repeat(marker, data.Length).ToArray();
            var comparer = new EqualThenConstantComparer(k, -1);
            var (finished, error) = RunWithin(() => BidirectionalSortCore(data, scratch, comparer));

            await Assert.That(finished).IsTrue().Because($"after {k} calls");
            await Assert.That(error is null or InvalidOperationException).IsTrue().Because($"after {k} calls: {error}");
            await Assert.That(data).DoesNotContain(marker).Because($"after {k} calls");
        }
    }

    static void BidirectionalSortCore(int[] data, int[] scratch, EqualThenConstantComparer comparer)
    {
        var s = new SortSpan<int, EqualThenConstantComparer, NullContext>(data, NullContext.Default, comparer, 0);
        BidirectionalStableQuickSort.SortCore(s, 0, data.Length - 1, scratch);
    }

    /// <summary>
    /// The stable partitions write into a scratch buffer that the public overloads rent. Passing the scratch directly puts guard cells right
    /// after it, so a region that overflows writes a guard cell instead of memory past the buffer. The guard is as long as the range, which
    /// covers the largest possible overflow.
    /// </summary>
    [Test]
    [Arguments(nameof(Sorter.Stable))]
    [Arguments(nameof(Sorter.BidirectionalStable))]
    public async Task StablePartitionWritesStayInsideTheScratch(string sorterName)
    {
        var sorter = Enum.Parse<Sorter>(sorterName);
        foreach (var n in new[] { 7, 31, 133, 1000 })
        {
            for (var seed = 1; seed <= 40; seed++)
            {
                foreach (var sticky in new[] { false, true })
                {
                    var label = $"{sorter} n={n} seed={seed} {(sticky ? "sticky" : "coin flip")}";
                    var data = FullRangeInts(n, seed);
                    var scratch = new int[n + n];
                    Array.Fill(scratch, int.MinValue);
                    IComparer<int> comparer = sticky ? new StickyIntComparer(new Random(seed)) : new CoinFlipComparer(new Random(seed));
                    var (finished, error) = RunWithin(() => StableSortCore(sorter, data, scratch, comparer));

                    await Assert.That(finished).IsTrue().Because(label);
                    await Assert.That(error is null or InvalidOperationException).IsTrue().Because($"{label}: {error}");
                    await Assert.That(scratch.Skip(n).All(v => v == int.MinValue)).IsTrue().Because($"{label}: a guard cell after the scratch was written");
                }
            }
        }
    }

    static void StableSortCore(Sorter sorter, int[] data, int[] scratch, IComparer<int> comparer)
    {
        var s = new SortSpan<int, IComparer<int>, NullContext>(data, NullContext.Default, comparer, 0);
        var span = scratch.AsSpan(0, data.Length);
        if (sorter == Sorter.Stable) StableQuickSort.SortCore(s, 0, data.Length - 1, span);
        else BidirectionalStableQuickSort.SortCore(s, 0, data.Length - 1, span);
    }

    /// <summary>
    /// The unguarded core of PairInsertionSort relies on the element before the range as a sentinel. Guard cells lie below that sentinel,
    /// so a shift that passes it reads a guard cell. Pairs start after the first element, so only even lengths leave a last element alone.
    /// </summary>
    [Test]
    public async Task PairInsertionUnguardedCoreStopsAtItsSentinel()
    {
        foreach (var n in new[] { 7, 8, 31, 32, 133 })
        {
            for (var seed = 1; seed <= 40; seed++)
            {
                var data = FullRangeInts(n, seed);
                IComparer<int>[] comparers = [new ConstantComparer(-1), new ConstantComparer(1), new StickyIntComparer(new Random(seed)), new CoinFlipComparer(new Random(seed)), new ReversingComparer(seed * n / 8)];
                foreach (var comparer in comparers)
                {
                    await Assert.That(PairInsertionUnguarded(data, comparer)).IsNull().Because($"n={n} seed={seed} {comparer.GetType().Name}");
                }
            }
        }
    }

    static string? PairInsertionUnguarded(int[] data, IComparer<int> comparer)
    {
        const int sentinel = int.MinValue + 1;
        var buffer = new int[GuardLength + 1 + data.Length + GuardLength];
        Array.Fill(buffer, int.MinValue);
        buffer[GuardLength] = sentinel;
        data.CopyTo(buffer, GuardLength + 1);
        var watch = new GuardWatch();
        var s = new SortSpan<int, WatchingComparer<int, IComparer<int>>, NullContext>(buffer, NullContext.Default, new WatchingComparer<int, IComparer<int>>(comparer, int.MinValue, watch), 0);
        try
        {
            PairInsertionSort.UnguardedSortCore(s, GuardLength + 1, GuardLength + 1 + data.Length);
        }
        catch (InvalidOperationException)
        {
        }
        catch (GuardCellReadException)
        {
        }

        if (watch.SawGuard) return "the comparer was shown a guard cell";
        if (buffer.Take(GuardLength).Concat(buffer.Skip(GuardLength + 1 + data.Length)).Any(v => v != int.MinValue)) return "a guard cell was overwritten";
        var expected = data.Append(sentinel).Order();
        var actual = buffer.Skip(GuardLength).Take(data.Length + 1).Order();
        return expected.SequenceEqual(actual) ? null : "the range and its sentinel are no longer a permutation of the input";
    }

    /// <summary>Answers from a table indexed by the two values, so its answers repeat but need not be consistent.</summary>
    sealed class TableComparer(int[][] table) : IComparer<int>
    {
        public int Compare(int x, int y) => table[x - 1][y - 1];
    }

    public static IEnumerable<Func<(string Name, int[] Input, int[][] Table)>> CycleSkipCases()
    {
        // x > y answers "equal" and anything else "less", which finds the 3 a place past both other values and then skips to the end.
        yield return () => ("first skip", [3, 1, 2], [[-1, -1, -1], [0, -1, -1], [0, 0, -1]]);
        // Found by a search over random tables. The skip reaches the end inside the rotation, after the first item has been placed.
        yield return () => ("rotation skip", [3, 1, 2], [[1, -1, -1], [1, 0, 0], [0, -1, -1]]);
    }

    /// <summary>
    /// The duplicate skip of CycleSort runs to the end of the range with these comparers. Guard cells follow the range and hold a value the
    /// table does not cover, so reading one throws from the comparer. The sort must stop with the range a permutation of its input.
    /// </summary>
    [Test]
    [MethodDataSource(nameof(CycleSkipCases))]
    public async Task CycleSortSkipStopsAtTheEndOfTheRange(string name, int[] input, int[][] table)
    {
        const int guard = -1;
        var buffer = input.Concat(Enumerable.Repeat(guard, GuardLength)).ToArray();
        var (finished, error) = RunWithin(() => CycleSort.Sort(buffer.AsSpan(0, input.Length), new TableComparer(table), NullContext.Default));

        await Assert.That(finished).IsTrue().Because(name);
        await Assert.That(error).IsTypeOf<InvalidOperationException>().Because($"{name}: {error}");
        await Assert.That(buffer.Skip(input.Length).All(v => v == guard)).IsTrue().Because($"{name}: a guard cell was overwritten");
        await Assert.That(buffer.Take(input.Length).Order().SequenceEqual(input.Order())).IsTrue().Because($"{name}: the range is no longer a permutation");
    }

    /// <summary>Sorts that stop only once their comparisons report order, and so count their progress against a bound.</summary>
    enum Bounded { OddEven, Circle, Gnome, GnomeNonOptimized, Fun, Bingo, Cycle }

    public static IEnumerable<string> BoundedSorters() => Enum.GetNames<Bounded>();

    static void SortBounded(Bounded sorter, int[] data, IComparer<int> comparer)
    {
        var span = data.AsSpan();
        switch (sorter)
        {
            case Bounded.OddEven: OddEvenSort.Sort(span, comparer, NullContext.Default); break;
            case Bounded.Circle: CircleSort.Sort(span, comparer, NullContext.Default); break;
            case Bounded.Gnome: GnomeSort.Sort(span, comparer, NullContext.Default); break;
            case Bounded.GnomeNonOptimized: GnomeSortNonOptimized.Sort(span, comparer, NullContext.Default); break;
            case Bounded.Fun: FunSort.Sort(span, comparer, NullContext.Default); break;
            case Bounded.Bingo: BingoSort.Sort(span, comparer, NullContext.Default); break;
            default: CycleSort.Sort(span, comparer, NullContext.Default); break;
        }
    }

    /// <summary>
    /// Each of these sorts repeats work until its comparisons report order, and counts that work against a bound that a consistent comparer
    /// never reaches. These comparers never report a consistent order, and each made one of the sorts run forever before it had a bound.
    /// The sorts must stop with the range a permutation of the input. Each runs on its own thread, so a missing bound fails the test instead
    /// of hanging the run.
    /// </summary>
    [Test]
    [MethodDataSource(nameof(BoundedSorters))]
    public async Task SortsThatWaitForOrderStillFinish(string sorterName)
    {
        var sorter = Enum.Parse<Bounded>(sorterName);
        foreach (var n in new[] { 3, 17, 40, 100 })
        {
            for (var seed = 1; seed <= 8; seed++)
            {
                var input = FullRangeInts(n, seed);
                IComparer<int>[] comparers =
                [
                    new ConstantComparer(-1), new ConstantComparer(1), new CoinFlipComparer(new Random(seed)), new StickyIntComparer(new Random(seed)),
                    new OverflowingComparer(),
                ];
                foreach (var comparer in comparers)
                {
                    var label = $"{sorter} n={n} seed={seed} {comparer.GetType().Name}";
                    var data = input.ToArray();
                    var (finished, error) = RunWithin(() => SortBounded(sorter, data, comparer));

                    await Assert.That(finished).IsTrue().Because(label);
                    await Assert.That(error is null or InvalidOperationException).IsTrue().Because($"{label}: {error}");
                    await Assert.That(data.Order().SequenceEqual(input.Order())).IsTrue().Because($"{label}: the range is no longer a permutation");
                }
            }
        }
    }

    /// <summary>
    /// BucketSortInteger takes its minimum and maximum from the comparer but its buckets from the numeric values. Ordering by absolute value
    /// is consistent, yet it leaves -1 numerically below the minimum 1. That disagreement, and the broken comparers that once indexed outside
    /// the buckets, must be reported, and the range is not written before the report.
    /// </summary>
    [Test]
    public async Task BucketSortIntegerReportsAComparerThatDisagreesWithTheValues()
    {
        int[] absolute = [1, 2, -1];
        var (_, error) = RunWithin(() => BucketSortInteger.Sort(absolute.AsSpan(), Comparer<int>.Create((a, b) => Math.Abs(a).CompareTo(Math.Abs(b))), NullContext.Default));
        await Assert.That(error).IsTypeOf<InvalidOperationException>();
        await Assert.That(absolute.SequenceEqual([1, 2, -1])).IsTrue();

        foreach (var n in new[] { 3, 17, 100, 1000 })
        {
            for (var seed = 1; seed <= 8; seed++)
            {
                var input = FullRangeInts(n, seed);
                IComparer<int>[] comparers =
                [
                    new ConstantComparer(-1), new ConstantComparer(1), new CoinFlipComparer(new Random(seed)), new StickyIntComparer(new Random(seed)),
                    new OverflowingComparer(),
                ];
                foreach (var comparer in comparers)
                {
                    var label = $"n={n} seed={seed} {comparer.GetType().Name}";
                    var data = input.ToArray();
                    (_, error) = RunWithin(() => BucketSortInteger.Sort(data.AsSpan(), comparer, NullContext.Default));

                    await Assert.That(error is null or InvalidOperationException).IsTrue().Because($"{label}: {error}");
                    await Assert.That(data.Order().SequenceEqual(input.Order())).IsTrue().Because($"{label}: the range is no longer a permutation");
                }
            }
        }
    }

    [Test]
    public async Task CrossMergeSurvivesEveryAnswerSequence()
    {
        foreach (var n in new[] { 17, 18 })
        {
            for (var left = 1; left < n; left++)
            {
                await Assert.That(WalkEveryAnswerSequence(left, n - left)).IsNull();
            }
        }
    }

    /// <summary>Longer merges cannot be walked exhaustively. Long stretches of one answer drive a single cursor to the end of its run.</summary>
    [Test]
    public async Task CrossMergeSurvivesStickyAnswers()
    {
        for (var seed = 1; seed <= 40_000; seed++)
        {
            await Assert.That(MergeWithStickyAnswers(seed)).IsNull().Because($"seed {seed}");
        }
    }

    /// <summary>With these lengths, a comparer reversed against interleaved runs overdraws the left run while the right run still looks long.</summary>
    [Test]
    public async Task CrossMergeSurvivesAReversedComparer()
    {
        await Assert.That(MergeWithReversedAnswers(83, 155)).IsNull();
    }

    /// <summary>
    /// Answers that make one batch exhaust a run. The first overdraws the right run, which would send a later block copy below the output.
    /// The others leave a cursor on the far end of its run, where the next comparison would read a guard cell.
    /// </summary>
    [Test]
    [Arguments(18, 11, "1000101010101010101010111")]
    [Arguments(18, 8, "101111111111111111")]
    [Arguments(8, 18, "0111111111111111110")]
    public async Task CrossMergeStopsWhenABatchExhaustsARun(int left, int right, string answers)
    {
        await Assert.That(MergeWithScript(left, right, answers)).IsNull();
    }

    /// <summary>NaN keys found to overdraw the right run during a whole sort, with an ordinary IEEE comparison.</summary>
    [Test]
    public async Task NaNKeysThatOverdrawARunStayInsideTheSpan()
    {
        var random = new Random(1760837542);
        var keys = new double[1024];
        for (var i = 0; i < keys.Length; i++) keys[i] = random.NextDouble() * 100 < 30 ? double.NaN : random.NextDouble();

        await Assert.That(Exercise(Sorter.Quad, keys, 12345.5, new IeeeComparer(), NullContext.Default)).IsNull();
    }

    /// <summary>
    /// The smallest known input: seven doubles whose NaNs use up the three left elements of the seven-element parity swap. Swap memory starts
    /// after guard cells, so the check that stops the merge is what keeps the next comparison from reading one.
    /// </summary>
    [Test]
    public async Task SevenElementParitySwapStopsBeforeItsSwapMemory()
    {
        var (threw, sawGuard) = SortSevenWithGuardedSwap();

        await Assert.That(threw).IsTrue();
        await Assert.That(sawGuard).IsFalse();
    }

    static (bool Threw, bool SawGuard) SortSevenWithGuardedSwap()
    {
        const double guard = 12345.5;
        const int swapStart = 8;
        double[] input = [2, 1, 1, double.NaN, double.NaN, double.NaN, 0];
        var swap = new double[swapStart + input.Length + swapStart];
        Array.Fill(swap, guard);
        var watch = new GuardWatch();
        var comparer = new WatchingComparer<double, IeeeComparer>(new IeeeComparer(), guard, watch);
        var a = new SortSpan<double, WatchingComparer<double, IeeeComparer>, NullContext>(input, NullContext.Default, comparer, 0);
        var w = new SortSpan<double, WatchingComparer<double, IeeeComparer>, NullContext>(swap, NullContext.Default, comparer, 1);

        try
        {
            QuadSort.SortSwap(a, 0, w, swapStart, input.Length, input.Length);
            return (false, watch.SawGuard);
        }
        catch (InvalidOperationException)
        {
            return (true, watch.SawGuard);
        }
    }
}
