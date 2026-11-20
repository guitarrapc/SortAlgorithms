using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SortAlgorithm.Tests;

public class PreparedFloatingSortCorrectnessTests
{
    [Test]
    public void NativeNaNFixturesRetainTheirSignalingBits()
    {
        if (BitConverter.SingleToInt32Bits(NaN<float>(3)) != 0x7f800001
            || BitConverter.DoubleToInt64Bits(NaN<double>(3)) != 0x7ff0000000000001L
            || BitConverter.HalfToUInt16Bits(NaN<Half>(3)) != 0x7c01)
            throw new InvalidOperationException("A native signaling-NaN fixture was quieted before the sort.");
    }

    [Test]
    [Arguments("QuickSort")]
    [Arguments("SampleSort")]
    [Arguments("BinaryInsertionSort")]
    [Arguments("HeapSort")]
    [Arguments("MinHeapSort")]
    [Arguments("BottomupHeapSort")]
    [Arguments("TernaryHeapSort")]
    [Arguments("SmoothSort")]
    [Arguments("IntroSort")]
    [Arguments("BlockQuickSort")]
    [Arguments("PDQSort")]
    [Arguments("PDQSortBranchless")]
    [Arguments("Ipnsort")]
    [Arguments("StdSort")]
    [Arguments("CycleSort")]
    [Arguments("DoubleSelectionSort")]
    public void FloatingSortsPreserveOrderBitsAndDeclaredStability(string algorithm)
    {
        Verify<Half>(algorithm);
        Verify<float>(algorithm);
        Verify<double>(algorithm);
    }

    static void Verify<T>(string algorithm) where T : unmanaged, IFloatingPointIeee754<T>
    {
        T[] edges = [NaN<T>(0), NaN<T>(1), NaN<T>(2), NaN<T>(3), -T.Zero, T.Zero,
            T.NegativeInfinity, T.PositiveInfinity, -T.Epsilon, T.Epsilon, -T.One, T.One];
        foreach (var n in new[] { 0, 1, 2, 3, 7, 13, 21, 31, 32, 33, 55, 89, 257, 1009 })
        foreach (var kind in new[] { 0, 1, 2 })
        foreach (var observed in new[] { false, true })
        {
            var random = new Random(2026 + n);
            var input = Enumerable.Range(0, n).Select(_ => edges[random.Next(edges.Length)]).ToArray();
            var comparer = new FloatingComparer<T>(kind);
            var expected = input.OrderBy(static value => value, comparer).ToArray();
            var actual = new T[n + 4];
            Array.Fill(actual, T.CreateChecked(987));
            input.CopyTo(actual, 2);
            if (observed) Run(algorithm, actual.AsSpan(), 2, n + 2, comparer, kind, new StatisticsContext());
            else Run(algorithm, actual.AsSpan(), 2, n + 2, comparer, kind, NullContext.Default);
            var output = actual.AsSpan(2, n);
            if (actual[0] != T.CreateChecked(987) || actual[1] != T.CreateChecked(987)
                || actual[^2] != T.CreateChecked(987) || actual[^1] != T.CreateChecked(987))
                throw new InvalidOperationException("A sort wrote outside its range.");
            for (var i = 0; i < n; i++)
                if (comparer.Compare(output[i], expected[i]) != 0) throw new InvalidOperationException("Wrong comparer order.");
            if (algorithm == "BinaryInsertionSort")
            {
                if (!MemoryMarshal.AsBytes(expected.AsSpan()).SequenceEqual(MemoryMarshal.AsBytes(output)))
                    throw new InvalidOperationException("Binary insertion changed the order of equivalent NaNs or signed zeros.");
            }
            else
            {
                var before = input.Select(Bits).Order().ToArray();
                var after = output.ToArray().Select(Bits).Order().ToArray();
                if (!before.SequenceEqual(after)) throw new InvalidOperationException("A sort changed element bits or multiplicity.");
            }
        }
    }

    internal static string Bits<T>(T value) where T : unmanaged
        => Convert.ToHexString(MemoryMarshal.AsBytes(MemoryMarshal.CreateReadOnlySpan(ref value, 1)));

    internal static T NaN<T>(int index) where T : unmanaged, IFloatingPointIeee754<T>
    {
        if (typeof(T) == typeof(float))
        {
            uint[] bits = [0x7fc00001U, 0x7fc00002U, 0xffc00003U, 0x7f800001U];
            var value = BitConverter.UInt32BitsToSingle(bits[index]);
            return Unsafe.As<float, T>(ref value);
        }
        if (typeof(T) == typeof(double))
        {
            ulong[] bits = [0x7ff8000000000001UL, 0x7ff8000000000002UL, 0xfff8000000000003UL, 0x7ff0000000000001UL];
            var value = BitConverter.UInt64BitsToDouble(bits[index]);
            return Unsafe.As<double, T>(ref value);
        }
        ushort[] halfBits = [0x7e01, 0x7e02, 0xfe03, 0x7c01];
        var half = BitConverter.UInt16BitsToHalf(halfBits[index]);
        return Unsafe.As<Half, T>(ref half);
    }

    internal static void Run<T, TComparer, TContext>(string algorithm, Span<T> span, int first, int last, TComparer comparer, int kind, TContext context)
        where T : unmanaged, IFloatingPointIeee754<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        switch (algorithm)
        {
            case "QuickSort":
                if (kind == 0) QuickSort.Sort(span, first, last, context);
                else QuickSort.Sort(span, first, last, comparer, context);
                break;
            case "SampleSort":
                if (kind == 0) SampleSort.Sort(span, first, last, context);
                else SampleSort.Sort(span, first, last, comparer, context);
                break;
            case "BinaryInsertionSort":
                if (kind == 0) BinaryInsertionSort.Sort(span, first, last, context);
                else BinaryInsertionSort.Sort(span, first, last, comparer, context);
                break;
            case "HeapSort":
                if (kind == 0) HeapSort.Sort(span, first, last, context);
                else HeapSort.Sort(span, first, last, comparer, context);
                break;
            case "MinHeapSort":
                if (kind == 0) MinHeapSort.Sort(span, first, last, context);
                else MinHeapSort.Sort(span, first, last, comparer, context);
                break;
            case "BottomupHeapSort":
                if (kind == 0) BottomupHeapSort.Sort(span, first, last, context);
                else BottomupHeapSort.Sort(span, first, last, comparer, context);
                break;
            case "TernaryHeapSort":
                if (kind == 0) TernaryHeapSort.Sort(span, first, last, context);
                else TernaryHeapSort.Sort(span, first, last, comparer, context);
                break;
            case "SmoothSort":
                if (kind == 0) SmoothSort.Sort(span.Slice(first, last - first), context);
                else SmoothSort.Sort(span.Slice(first, last - first), comparer, context);
                break;
            case "IntroSort":
                if (kind == 0) IntroSort.Sort(span, first, last, context);
                else IntroSort.Sort(span, first, last, comparer, context);
                break;
            case "BlockQuickSort":
                if (kind == 0) BlockQuickSort.Sort(span, first, last, context);
                else BlockQuickSort.Sort(span, first, last, comparer, context);
                break;
            case "PDQSort":
                if (kind == 0) PDQSort.Sort(span, first, last, context);
                else PDQSort.Sort(span, first, last, comparer, context);
                break;
            case "PDQSortBranchless":
                if (kind == 0) PDQSortBranchless.Sort(span, first, last, context);
                else PDQSortBranchless.Sort(span, first, last, comparer, context);
                break;
            case "Ipnsort":
                if (kind == 0) Ipnsort.Sort(span, first, last, context);
                else Ipnsort.Sort(span, first, last, comparer, context);
                break;
            case "StdSort":
                if (kind == 0) StdSort.Sort(span, first, last, context);
                else StdSort.Sort(span, first, last, comparer, context);
                break;
            case "CycleSort":
                if (kind == 0) CycleSort.Sort(span.Slice(first, last - first), context);
                else CycleSort.Sort(span.Slice(first, last - first), comparer, context);
                break;
            case "DoubleSelectionSort":
                if (kind == 0) DoubleSelectionSort.Sort(span, first, last, context);
                else DoubleSelectionSort.Sort(span, first, last, comparer, context);
                break;
        }
    }

    sealed class FloatingComparer<T>(int kind) : IComparer<T> where T : unmanaged, IFloatingPointIeee754<T>
    {
        public int Compare(T a, T b)
        {
            var result = kind switch
            {
                1 => b.CompareTo(a),
                2 when T.IsNaN(a) != T.IsNaN(b) => T.IsNaN(a) ? 1 : -1,
                2 when a == T.Zero && b == T.Zero && T.IsNegative(a) != T.IsNegative(b) => T.IsNegative(a) ? -1 : 1,
                _ => a.CompareTo(b)
            };
            return result < 0 ? int.MinValue : result > 0 ? int.MaxValue : 0;
        }
    }
}
