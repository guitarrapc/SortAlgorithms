using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SortAlgorithm.Tests;

public class IntroSortMedianTests
{
    delegate int Median<T, TComparer, TContext>(SortSpan<T, TComparer, TContext> span, int a, int b, int c)
        where TComparer : IComparer<T> where TContext : ISortContext;

    static Median<T, TComparer, TContext> Bind<T, TComparer, TContext>()
        where TComparer : IComparer<T> where TContext : ISortContext
        => typeof(IntroSort).GetMethod("MedianOf3Index", BindingFlags.NonPublic | BindingFlags.Static)!
            .MakeGenericMethod(typeof(T), typeof(TComparer), typeof(TContext)).CreateDelegate<Median<T, TComparer, TContext>>();

    [Test]
    public void EveryThreeValueTupleSelectsTheMedianWithoutChangingElements()
    {
        int[] alphabet = [-2, -1, 0, 1, 2];
        VerifyTuples(alphabet, default(ComparableComparer<int>), NullContext.Default);
        VerifyTuples(alphabet, default(ComparableComparer<int>), new StatisticsContext());
        VerifyTuples(alphabet, new Reverse<int>(), NullContext.Default);
        VerifyTuples(alphabet, new Reverse<int>(), new StatisticsContext());
        IComparer<int> boxed = new Reverse<int>();
        VerifyTuples(alphabet, boxed, NullContext.Default);
        VerifyTuples(alphabet, boxed, new StatisticsContext());
    }

    [Test]
    public void FloatingMediansRespectNaNsSignedZerosAndCustomOrder()
    {
        VerifyFloating<Half>();
        VerifyFloating<float>();
        VerifyFloating<double>();
    }

    static void VerifyFloating<T>() where T : unmanaged, IFloatingPointIeee754<T>
    {
        T[] alphabet = [PreparedFloatingSortCorrectnessTests.NaN<T>(0), PreparedFloatingSortCorrectnessTests.NaN<T>(3), -T.Zero, T.Zero, T.One];
        VerifyTuples(alphabet, default(ComparableComparer<T>), NullContext.Default);
        VerifyTuples(alphabet, default(ComparableComparer<T>), new StatisticsContext());
        IComparer<T> payload = Comparer<T>.Create((x, y) => PreparedFloatingSortCorrectnessTests.Bits(x).CompareTo(PreparedFloatingSortCorrectnessTests.Bits(y)));
        VerifyTuples(alphabet, payload, NullContext.Default);
        VerifyTuples(alphabet, payload, new StatisticsContext());
        VerifyTuples(alphabet, new Reverse<T>(), NullContext.Default);
        VerifyTuples(alphabet, new Reverse<T>(), new StatisticsContext());
    }

    static void VerifyTuples<T, TComparer, TContext>(T[] alphabet, TComparer comparer, TContext context)
        where T : unmanaged where TComparer : IComparer<T> where TContext : ISortContext
    {
        var median = Bind<T, TComparer, TContext>();
        var count = 0;
        for (var tuple = 0; tuple < 125; tuple++)
        {
            var values = new T[7];
            var code = tuple;
            for (var i = 0; i < 3; i++) { values[1 + 2 * i] = alphabet[code % 5]; code /= 5; }
            var before = MemoryMarshal.AsBytes(values.AsSpan()).ToArray();
            var expected = new[] { values[1], values[3], values[5] }.OrderBy(x => x, comparer).ToArray()[1];
            var span = new SortSpan<T, TComparer, TContext>(values, context, comparer, 7, 11);
            var index = median(span, 1, 3, 5);
            if (index is not (1 or 3 or 5) || comparer.Compare(values[index], expected) != 0)
                throw new InvalidOperationException($"Wrong median: {typeof(T).Name}/{tuple}.");
            if (!MemoryMarshal.AsBytes(values.AsSpan()).SequenceEqual(before))
                throw new InvalidOperationException("Median selection changed an element or its bits.");
            count++;
        }
        if (context is StatisticsContext stats && (stats.CompareCount < (ulong)(2 * count) || stats.CompareCount > (ulong)(3 * count)
            || stats.IndexReadCount != 2 * stats.CompareCount || stats.IndexWriteCount != 0 || stats.SwapCount != 0))
            throw new InvalidOperationException("Median operations were not accurately counted.");
    }

    [Test]
    public void MedianObservationUsesTheOriginalBufferAndCoordinates()
    {
        int[] input = [999, 4, 999, 0, 999, 3, 999];
        var reads = 0;
        var comparisons = 0;
        static bool IsSample(int i) => i is 12 or 14 or 16;
        var context = new VisualizationContext(
            onIndexRead: (i, b) => { if (b != 7 || !IsSample(i)) throw new InvalidOperationException("Wrong read coordinates."); reads++; },
            onCompare: (i, j, r, bi, bj) => { if (bi != 7 || bj != 7 || !IsSample(i) || !IsSample(j)) throw new InvalidOperationException("Wrong comparison coordinates."); comparisons++; },
            onSwap: (_, _, _) => throw new InvalidOperationException("Unexpected swap."),
            onIndexWrite: (_, _, _) => throw new InvalidOperationException("Unexpected write."),
            onRangeCopy: (_, _, _, _, _, _) => throw new InvalidOperationException("Unexpected copy."),
            onPhase: (_, _, _, _) => throw new InvalidOperationException("Unexpected phase."),
            onRole: (_, _, _) => throw new InvalidOperationException("Unexpected role."),
            onLink: (_, _, _, _) => throw new InvalidOperationException("Unexpected link."));
        var span = new SortSpan<int, ComparableComparer<int>, VisualizationContext>(input, context, default, 7, 11);
        var index = Bind<int, ComparableComparer<int>, VisualizationContext>()(span, 1, 3, 5);
        if (input[index] != 3 || reads != 6 || comparisons != 3)
            throw new InvalidOperationException("Wrong median or observation counts.");
    }

    /// <summary>The first pivot is the median of the first, middle and last elements, which on sorted input is the middle one.</summary>
    [Test]
    [Arguments(17, 0)]
    [Arguments(100, 0)]
    [Arguments(1000, 0)]
    [Arguments(1001, 5)]
    public void PublicSortChoosesTheSampleMedian(int length, int first)
    {
        var input = Enumerable.Repeat(-123456, first + length + 3).ToArray();
        Enumerable.Range(0, length).ToArray().CopyTo(input, first);
        int? pivot = null;
        IntroSort.Sort(input.AsSpan(), first, first + length, new VisualizationContext(
            onRole: (i, b, r) => { if (r == RoleType.Pivot && pivot is null) { if (b != 0) throw new InvalidOperationException("Wrong pivot buffer."); pivot = i; } }));
        if (pivot != first + length / 2 || !input.AsSpan(first, length).SequenceEqual(Enumerable.Range(0, length).ToArray())
            || input.Take(first).Concat(input.Skip(first + length)).Any(x => x != -123456))
            throw new InvalidOperationException("Wrong public pivot, sort result or range.");
    }

    readonly struct Reverse<T> : IComparer<T> where T : IComparable<T>
    {
        public int Compare(T? x, T? y) => y!.CompareTo(x!);
    }
}
