using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using System.Numerics;
using System.Runtime.InteropServices;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

public class PreparedNaNComparisonTests
{
    [Test]
    public void NaNKeyClassificationCoversEveryHalfBitPatternAndRandomFloatingBits()
    {
        var half = new SortSpan<Half, ComparableComparer<Half>, NullContext>(Span<Half>.Empty, default, default, 0);
        for (var bits = 0; bits <= ushort.MaxValue; bits++)
        {
            var value = BitConverter.UInt16BitsToHalf((ushort)bits);
            if (half.IsNaturalFloatingNaN(value) != Half.IsNaN(value))
                throw new InvalidOperationException($"Half NaN classification mismatch: {bits:X4}");
        }
        var single = new SortSpan<float, ComparableComparer<float>, NullContext>(Span<float>.Empty, default, default, 0);
        var wide = new SortSpan<double, ComparableComparer<double>, NullContext>(Span<double>.Empty, default, default, 0);
        var random = new Random(2026);
        for (var i = 0; i < 20_000; i++)
        {
            var a = BitConverter.Int32BitsToSingle((int)random.NextInt64(int.MinValue, (long)int.MaxValue + 1));
            var b = BitConverter.Int64BitsToDouble(random.NextInt64(long.MinValue, long.MaxValue));
            if (single.IsNaturalFloatingNaN(a) != float.IsNaN(a) || wide.IsNaturalFloatingNaN(b) != double.IsNaN(b))
                throw new InvalidOperationException("Floating NaN bit classification mismatch.");
        }
    }

    [Test]
    public void PreparedComparisonsPreserveOrderAndRetainedBits()
    {
        VerifyComparisons<Half>();
        VerifyComparisons<float>();
        VerifyComparisons<double>();
    }

    static void VerifyComparisons<T>() where T : unmanaged, IFloatingPointIeee754<T>
    {
        VerifyComparisons<T, ComparableComparer<T>, NullContext>(default, default, true);
        VerifyComparisons<T, ComparableComparer<T>, StatisticsContext>(default, new(), true);
        VerifyComparisons<T, DescendingComparer<T>, NullContext>(default, default, false);
        VerifyComparisons<T, DescendingComparer<T>, StatisticsContext>(default, new(), false);
        VerifyComparisons<T, Comparer<T>, NullContext>(Comparer<T>.Default, default, false);
    }

    static void VerifyComparisons<T, TComparer, TContext>(TComparer comparer, TContext context, bool natural)
        where T : unmanaged, IFloatingPointIeee754<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        T[] values = [T.NaN, -T.NaN, T.NegativeInfinity, -T.One, -T.Epsilon, -T.Zero,
            T.Zero, T.Epsilon, T.One, T.PositiveInfinity];
        foreach (var a in values)
        foreach (var b in values)
        {
            T[] pair = [a, b];
            var span = new SortSpan<T, TComparer, TContext>(pair, context, comparer, 7, 11);
            var state = span.IsNaturalFloatingNaN(b);
            var expectedState = natural && typeof(TContext) == typeof(NullContext) && T.IsNaN(b);
            if (state != expectedState) throw new InvalidOperationException("Wrong prepared NaN classification.");
            var actual = span.IsElementGreaterThanWithNaNState(0, b, state, out var retained);
            if (actual != (comparer.Compare(a, b) > 0) || !SameBits(a, retained))
                throw new InvalidOperationException("Prepared comparison changed order or element bits.");
            if (span.IsElementLessOrEqual(0, b, out retained) != (comparer.Compare(a, b) <= 0) || !SameBits(a, retained))
                throw new InvalidOperationException("Read-retaining inclusive comparison changed order or element bits.");
        }
        if (context is StatisticsContext statistics && (statistics.CompareCount != 200 || statistics.IndexReadCount != 200))
            throw new InvalidOperationException("Preparing a cached value must not announce additional element operations.");
    }

    [Test]
    public void InsertionRetainsNaNPayloadsSignedZerosAndComparerOrder()
    {
        VerifySort<Half>();
        VerifySort<float>();
        VerifySort<double>();
    }

    static void VerifySort<T>() where T : unmanaged, IFloatingPointIeee754<T>
    {
        T[] edge = [T.NaN, -T.NaN, T.Zero, -T.Zero, T.One, -T.One, T.Epsilon, -T.Epsilon,
            T.PositiveInfinity, T.NegativeInfinity,
            T.CreateSaturating(BitConverter.Int32BitsToSingle(0x7fc00001)),
            T.CreateSaturating(BitConverter.Int32BitsToSingle(unchecked((int)0xffc00002))),
            T.CreateSaturating(BitConverter.Int32BitsToSingle(0x7f800001))];
        foreach (var length in new[] { 0, 1, 2, 3, 30, 31, 32, 33, 256, 257 })
        {
            var random = new Random(42 + length);
            var input = Enumerable.Range(0, length).Select(_ => edge[random.Next(edge.Length)]).ToArray();
            var expected = input.OrderBy(static x => x).ToArray();
            var guarded = new T[length + 2];
            guarded[0] = T.CreateChecked(123);
            guarded[^1] = T.CreateChecked(-456);
            input.CopyTo(guarded, 1);
            InsertionSort.Sort(guarded.AsSpan(1, length), NullContext.Default);
            if (!SameBits(expected.AsSpan(), guarded.AsSpan(1, length))
                || guarded[0] != T.CreateChecked(123) || guarded[^1] != T.CreateChecked(-456))
                throw new InvalidOperationException("Natural floating insertion violated order, stability, or slice boundaries.");
            var descending = input.OrderBy(static x => x, new DescendingComparer<T>()).ToArray();
            InsertionSort.Sort(input.AsSpan(), new DescendingComparer<T>(), NullContext.Default);
            if (!SameBits(descending.AsSpan(), input.AsSpan()))
                throw new InvalidOperationException("Prepared path bypassed a custom comparer.");
        }
    }

    [Test]
    public async Task ObservedInsertionKeepsTheCompleteEventSequence()
    {
        var expected = Run(true);
        var actual = Run(false);
        await Assert.That(actual.Events).IsEquivalentTo(expected.Events, CollectionOrdering.Matching);
        await Assert.That(actual.Bits).IsEquivalentTo(expected.Bits, CollectionOrdering.Matching);

        static (List<string> Events, int[] Bits) Run(bool reference)
        {
            float[] values = [-123, 456, BitConverter.Int32BitsToSingle(0x7fc00001), 0f, -0f, 3,
                BitConverter.Int32BitsToSingle(unchecked((int)0xffc00002)), float.NegativeInfinity,
                float.PositiveInfinity, 777, 888];
            var events = new List<string>();
            var context = new VisualizationContext(
                onIndexRead: (i, buffer) => events.Add($"read:{i}:{buffer}"),
                onIndexWrite: (i, buffer, value) => events.Add($"write:{i}:{buffer}:{BitConverter.SingleToInt32Bits((float)value!):X8}"),
                onCompare: (i, j, result, bi, bj) => events.Add($"compare:{i}:{j}:{result}:{bi}:{bj}"),
                onPhase: (phase, a, b, c) => events.Add($"phase:{phase}:{a}:{b}:{c}"),
                onRole: (i, buffer, role) => events.Add($"role:{i}:{buffer}:{role}"));
            var span = new SortSpan<float, ComparableComparer<float>, VisualizationContext>(values.AsSpan(2, 7), context, default, 7, 2);
            if (reference) ReferenceCore(span);
            else InsertionSort.SortCore(span, 0, span.Length);
            return (events, values.Select(BitConverter.SingleToInt32Bits).ToArray());
        }
    }

    // Faithful pre-optimization guarded insertion: comparison order and callbacks are the reference contract.
    static void ReferenceCore(SortSpan<float, ComparableComparer<float>, VisualizationContext> span)
    {
        for (var i = 1; i < span.Length; i++)
        {
            span.Context.OnPhase(SortPhase.InsertionPass, span.Offset + i, span.Offset, span.Offset + span.Length - 1);
            span.Context.OnRole(span.Offset + i, span.BufferId, RoleType.RightPointer);
            var value = span.Read(i);
            var j = i - 1;
            while (j >= 0)
            {
                if (!span.IsElementGreaterThan(j, value, out var a)) break;
                span.Write(j + 1, a);
                j--;
            }
            if (j != i - 1) span.Write(j + 1, value);
            span.Context.OnRole(span.Offset + i, span.BufferId, RoleType.None);
        }
    }

    static bool SameBits<T>(T a, T b) where T : unmanaged
        => MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref a, 1))
            .SequenceEqual(MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref b, 1)));

    static bool SameBits<T>(Span<T> a, Span<T> b) where T : unmanaged
        => MemoryMarshal.AsBytes(a).SequenceEqual(MemoryMarshal.AsBytes(b));

    readonly struct DescendingComparer<T> : IComparer<T> where T : unmanaged, IFloatingPointIeee754<T>
    {
        public int Compare(T x, T y) => y.CompareTo(x);
    }
}
