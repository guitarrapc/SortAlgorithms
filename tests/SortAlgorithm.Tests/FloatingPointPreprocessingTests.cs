using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace SortAlgorithm.Tests;

public class FloatingPointPreprocessingTests
{
    [Test]
    [Arguments("IntroSort")]
    [Arguments("BlockQuickSort")]
    [Arguments("PDQSort")]
    [Arguments("PDQSortBranchless")]
    [Arguments("Ipnsort")]
    [Arguments("StdSort")]
    public void CustomNaNPayloadOrderIsPreserved(string algorithm)
    {
        VerifyCustomOrder<Half>(algorithm);
        VerifyCustomOrder<float>(algorithm);
        VerifyCustomOrder<double>(algorithm);
    }

    static void VerifyCustomOrder<T>(string algorithm) where T : unmanaged, IFloatingPointIeee754<T>
    {
        foreach (var n in new[] { 0, 1, 2, 3, 7, 31, 33, 1009 })
        foreach (var allNaN in new[] { false, true })
        {
            var input = Enumerable.Range(0, n).Select(i => allNaN || i % 3 == 0
                ? PreparedFloatingSortCorrectnessTests.NaN<T>((n - i) % 4)
                : i % 3 == 1 ? -T.Zero : T.Zero).ToArray();
            Verify(algorithm, input, default(PayloadOrder<T>), NullContext.Default);
            Verify(algorithm, input, default(PayloadOrder<T>), new StatisticsContext());
            IComparer<T> boxed = default(PayloadOrder<T>);
            Verify(algorithm, input, boxed, NullContext.Default);
            Verify(algorithm, input, boxed, new StatisticsContext());
            Verify(algorithm, input, Comparer<T>.Default, NullContext.Default);
            Verify(algorithm, input, Comparer<T>.Default, new StatisticsContext());
        }
    }

    static void Verify<T, TComparer, TContext>(string algorithm, T[] input, TComparer comparer, TContext context)
        where T : unmanaged, IFloatingPointIeee754<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        var sentinel = T.CreateChecked(987);
        var guarded = Enumerable.Repeat(sentinel, input.Length + 4).ToArray();
        input.CopyTo(guarded, 2);
        PreparedFloatingSortCorrectnessTests.Run(algorithm, guarded.AsSpan(), 2, input.Length + 2, comparer, 1, context);
        var output = guarded.AsSpan(2, input.Length).ToArray();
        var expected = input.OrderBy(static x => x, comparer).ToArray();
        for (var i = 0; i < output.Length; i++)
            if (comparer.Compare(output[i], expected[i]) != 0) throw new InvalidOperationException("Wrong custom payload order.");
        if (!input.Select(PreparedFloatingSortCorrectnessTests.Bits).Order().SequenceEqual(output.Select(PreparedFloatingSortCorrectnessTests.Bits).Order()))
            throw new InvalidOperationException("Element bits or multiplicity changed.");
        if (guarded[0] != sentinel || guarded[1] != sentinel || guarded[^2] != sentinel || guarded[^1] != sentinel)
            throw new InvalidOperationException("A sort wrote outside its range.");
    }

    [Test]
    public void PreprocessingUsesOnlyTheNaturalFloatingOrder()
    {
        VerifyPreprocessing<Half>();
        VerifyPreprocessing<float>();
        VerifyPreprocessing<double>();
        var events = new List<string>();
        var context = EventContext(events);
        int[] integers = [3, 2, 1];
        var span = new SortSpan<int, ComparableComparer<int>, VisualizationContext>(integers, context, default, 7, 11);
        if (FloatingPointUtils.MoveNaNsToFront(span, 1, 3) != 1 || events.Count != 0 || !integers.SequenceEqual(new[] { 3, 2, 1 }))
            throw new InvalidOperationException("Integer preprocessing was not a no-op.");
        var customSpan = new SortSpan<int, Comparer<int>, VisualizationContext>(integers, context, Comparer<int>.Default, 7, 11);
        if (FloatingPointUtils.MoveNaNsToFront(customSpan, 1, 3) != 1 || events.Count != 0 || !integers.SequenceEqual(new[] { 3, 2, 1 }))
            throw new InvalidOperationException("Custom integer preprocessing was not a no-op.");
    }

    static void VerifyPreprocessing<T>() where T : unmanaged, IFloatingPointIeee754<T>
    {
        CheckPreprocessing<T, ComparableComparer<T>>(default, true);
        IComparer<T> natural = default(ComparableComparer<T>);
        CheckPreprocessing<T, IComparer<T>>(natural, true);
        CheckPreprocessing<T, PayloadOrder<T>>(default, false);
        IComparer<T> custom = default(PayloadOrder<T>);
        CheckPreprocessing<T, IComparer<T>>(custom, false);
        CheckPreprocessing<T, Comparer<T>>(Comparer<T>.Default, false);
    }

    static void CheckPreprocessing<T, TComparer>(TComparer comparer, bool enabled)
        where T : unmanaged, IFloatingPointIeee754<T>
        where TComparer : IComparer<T>
    {
        T[] input = [T.CreateChecked(987), T.CreateChecked(3), PreparedFloatingSortCorrectnessTests.NaN<T>(0),
            T.CreateChecked(2), PreparedFloatingSortCorrectnessTests.NaN<T>(1), T.CreateChecked(987)];
        var before = input.Select(PreparedFloatingSortCorrectnessTests.Bits).ToArray();
        var events = new List<string>();
        var context = EventContext(events);
        var span = new SortSpan<T, TComparer, VisualizationContext>(input, context, comparer, 7, 11);
        var end = FloatingPointUtils.MoveNaNsToFront(span, 1, 5);
        if (enabled)
        {
            string[] expected = ["r:12:7", "r:13:7", "s:12:13:7", "r:14:7", "r:15:7", "s:13:15:7"];
            if (end != 3 || !events.SequenceEqual(expected) || !T.IsNaN(input[1]) || !T.IsNaN(input[2]))
                throw new InvalidOperationException("Natural preprocessing changed its events or range.");
            if (!before.Order().SequenceEqual(input.Select(PreparedFloatingSortCorrectnessTests.Bits).Order()))
                throw new InvalidOperationException("Natural preprocessing changed element bits.");
        }
        else if (end != 1 || events.Count != 0 || !before.SequenceEqual(input.Select(PreparedFloatingSortCorrectnessTests.Bits)))
            throw new InvalidOperationException("Custom-order preprocessing was not a no-op.");
    }

    static VisualizationContext EventContext(List<string> events) => new(
        onCompare: (i, j, r, bi, bj) => events.Add($"c:{i}:{j}:{r}:{bi}:{bj}"),
        onIndexRead: (i, b) => events.Add($"r:{i}:{b}"),
        onSwap: (i, j, b) => events.Add($"s:{i}:{j}:{b}"),
        onIndexWrite: (i, b, v) => events.Add($"w:{i}:{b}"),
        onRangeCopy: (i, j, n, bi, bj, v) => events.Add($"x:{i}:{j}:{n}:{bi}:{bj}"),
        onPhase: (p, a, b, c) => events.Add($"p:{p}:{a}:{b}:{c}"),
        onRole: (i, b, r) => events.Add($"role:{i}:{b}:{r}"),
        onLink: (p, c, b, s) => events.Add($"link:{p}:{c}:{b}:{s}"));

    [Test]
    [Arguments(0)]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(999)]
    [Arguments(1000)]
    [Arguments(1001)]
    [Arguments(2048)]
    public void SpreadSortRespectsCustomFloatingKeys(int length)
    {
        VerifySpread<Half>(length);
        VerifySpread<float>(length);
        VerifySpread<double>(length);
    }

    static void VerifySpread<T>(int length) where T : unmanaged, IFloatingPointIeee754<T>
    {
        foreach (var nanKey in new[] { 0, 1, 2 })
        foreach (var pattern in new[] { 0, 1, 2, 3 })
        {
            var selector = new NaNKey<T>(nanKey);
            var input = Enumerable.Range(0, length).Select(i => pattern == 3 || i % 7 == 0
                ? PreparedFloatingSortCorrectnessTests.NaN<T>(i % 4) : i % 2 == 0 ? -T.Zero : T.One).ToArray();
            if (pattern == 1) input = input.OrderBy(selector.GetKey).ToArray();
            if (pattern == 2) input = input.OrderByDescending(selector.GetKey).ToArray();
            foreach (var observed in new[] { false, true })
            {
                var actual = (T[])input.Clone();
                if (observed) SpreadSort.SortBy(actual.AsSpan(), selector, new StatisticsContext());
                else SpreadSort.SortBy(actual.AsSpan(), selector);
                ValidateKeys(input, actual, selector.GetKey);
                actual = (T[])input.Clone();
                static int Project(T x) => T.IsNaN(x) ? int.MaxValue : x == T.Zero ? int.MinValue : 0;
                if (observed) SpreadSort.SortBy(actual.AsSpan(), Project, new StatisticsContext());
                else SpreadSort.SortBy(actual.AsSpan(), Project);
                ValidateKeys(input, actual, static x => (uint)Project(x) ^ 0x8000_0000U);
            }
        }
    }

    static void ValidateKeys<T>(T[] before, T[] after, Func<T, ulong> key) where T : unmanaged
    {
        for (var i = 1; i < after.Length; i++)
            if (key(after[i - 1]) > key(after[i])) throw new InvalidOperationException("Wrong custom radix-key order.");
        if (!before.Select(PreparedFloatingSortCorrectnessTests.Bits).Order().SequenceEqual(after.Select(PreparedFloatingSortCorrectnessTests.Bits).Order()))
            throw new InvalidOperationException("SpreadSort changed element bits.");
    }

    readonly struct NaNKey<T>(int nanKey) : IRadixKeySelector<T> where T : unmanaged, IFloatingPointIeee754<T>
    {
        public static int KeyBits => 2;
        public ulong GetKey(T value) => T.IsNaN(value) ? (ulong)nanKey : value == T.Zero ? (nanKey == 0 ? 1UL : 0UL) : (nanKey == 2 ? 1UL : 2UL);
    }

    readonly struct PayloadOrder<T> : IComparer<T> where T : unmanaged, IFloatingPointIeee754<T>
    {
        public int Compare(T a, T b)
        {
            if (T.IsNaN(a)) return T.IsNaN(b) ? RawBits(a).CompareTo(RawBits(b)) : 1;
            if (T.IsNaN(b)) return -1;
            return a == T.Zero && b == T.Zero ? RawBits(a).CompareTo(RawBits(b)) : a.CompareTo(b);
        }

        static ulong RawBits(T value)
        {
            if (typeof(T) == typeof(float)) return BitConverter.SingleToUInt32Bits(Unsafe.As<T, float>(ref value));
            if (typeof(T) == typeof(double)) return BitConverter.DoubleToUInt64Bits(Unsafe.As<T, double>(ref value));
            return BitConverter.HalfToUInt16Bits(Unsafe.As<T, Half>(ref value));
        }
    }
}
