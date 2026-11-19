using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

/// <summary>
/// The merges of QuadSort and FluxSort advance their cursors on the strength of earlier comparisons, and Release builds do not check
/// element access. Guard cells surround each buffer under test. The comparers report every guard cell they are shown, which reveals a
/// read that only feeds a comparison, and the cells must keep their value, which reveals a stray write.
/// </summary>
public class InconsistentComparerSafetyTests
{
    const int GuardLength = 64;
    const long GuardKey = long.MinValue;

    enum Sorter { Quad, QuadLimitedSwap, Flux }

    sealed class GuardWatch
    {
        public bool SawGuard;
    }

    readonly struct WatchingComparer<T, TInner>(TInner inner, T guard, GuardWatch watch) : IComparer<T>
        where TInner : IComparer<T>
    {
        public int Compare(T? x, T? y)
        {
            if (EqualityComparer<T>.Default.Equals(x, guard) || EqualityComparer<T>.Default.Equals(y, guard)) watch.SawGuard = true;
            return inner.Compare(x, y);
        }
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

    static void Sort<T, TComparer, TContext>(Sorter sorter, Span<T> span, TComparer comparer, TContext context)
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        switch (sorter)
        {
            case Sorter.Quad: QuadSort.Sort(span, comparer, context); break;
            case Sorter.QuadLimitedSwap: QuadSort.SortWithSwapSize(span, 64, comparer, context); break;
            default: FluxSort.Sort(span, comparer, context); break;
        }
    }

    /// <summary>
    /// Sorts between guard cells. The sort may finish or report the broken comparer, and either way the span may hold only values of the
    /// input. Returns a description of the violation, if any.
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
            Sort(sorter, buffer.AsSpan(GuardLength, input.Length), new WatchingComparer<T, TComparer>(comparer, guard, watch), context);
        }
        catch (InvalidOperationException)
        {
            // The documented report of a comparer that is not a consistent total order.
        }

        if (watch.SawGuard) return "the comparer was shown a guard cell";

        var allowed = new HashSet<T>(input);
        for (var i = 0; i < buffer.Length; i++)
        {
            var inside = i >= GuardLength && i < GuardLength + input.Length;
            if (inside && !allowed.Contains(buffer[i])) return $"index {i - GuardLength} holds {buffer[i]}, which is not an input value";
            if (!inside && !EqualityComparer<T>.Default.Equals(buffer[i], guard)) return $"guard cell {i} was overwritten";
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
