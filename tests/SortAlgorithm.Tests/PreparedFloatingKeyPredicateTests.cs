using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using System.Numerics;

namespace SortAlgorithm.Tests;

public class PreparedFloatingKeyPredicateTests
{
    [Test]
    public void PreparedPredicatesPreserveAllFloatingOrderClasses()
    {
        Verify<Half>();
        Verify<float>();
        Verify<double>();
    }

    static void Verify<T>() where T : unmanaged, IFloatingPointIeee754<T>
    {
        Verify<T, ComparableComparer<T>, NullContext>(default, default);
        Verify<T, ComparableComparer<T>, StatisticsContext>(default, new());
        Verify<T, Descending<T>, NullContext>(default, default);
        Verify<T, Descending<T>, StatisticsContext>(default, new());
        Verify<T, SpecialOrder<T>, NullContext>(new(), default);
        Verify<T, SpecialOrder<T>, StatisticsContext>(new(), new());
        Verify<T, Comparer<T>, NullContext>(Comparer<T>.Default, default);
    }

    static void Verify<T, TComparer, TContext>(TComparer comparer, TContext context)
        where T : unmanaged, IFloatingPointIeee754<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        T[] values = [T.NaN, -T.NaN, T.NegativeInfinity, -T.One, -T.Epsilon,
            -T.Zero, T.Zero, T.Epsilon, T.One, T.PositiveInfinity];
        foreach (var a in values)
        foreach (var b in values)
        {
            T[] pair = [a, b];
            var span = new SortSpan<T, TComparer, TContext>(pair, context, comparer, 7, 11);
            var result = comparer.Compare(a, b);
            var aIsNaN = span.IsNaturalFloatingNaN(a);
            var bIsNaN = span.IsNaturalFloatingNaN(b);
            Check(span.IsElementLessThanWithNaNState(0, b, bIsNaN), result < 0);
            Check(span.IsElementLessOrEqualWithNaNState(0, b, bIsNaN), result <= 0);
            Check(span.IsValueLessThanWithNaNState(a, 1, aIsNaN), result < 0);
            Check(span.IsValueLessOrEqualWithNaNState(a, 1, aIsNaN), result <= 0);
            Check(span.IsValueGreaterThanWithNaNState(a, 1, aIsNaN), result > 0);
            Check(span.IsValueGreaterOrEqualWithNaNState(a, 1, aIsNaN), result >= 0);
        }
        if (context is StatisticsContext statistics
            && (statistics.CompareCount != 600 || statistics.IndexReadCount != 600))
            throw new InvalidOperationException("Prepared predicates changed observable operation counts.");
    }

    [Test]
    public void PreparedPredicatesRetainComparisonSidesAndCoordinates()
    {
        var events = new List<string>();
        var context = new VisualizationContext(
            onIndexRead: (index, buffer) => events.Add($"read:{index}:{buffer}"),
            onCompare: (i, j, result, bi, bj) => events.Add($"compare:{i}:{j}:{result}:{bi}:{bj}"));
        float[] pair = [float.NaN, 0f];
        var span = new SortSpan<float, SpecialOrder<float>, VisualizationContext>(pair, context, new(), 7, 11);
        span.IsElementLessThanWithNaNState(0, 0f, false);
        span.IsElementLessOrEqualWithNaNState(0, 0f, false);
        span.IsValueLessThanWithNaNState(float.NaN, 1, false);
        span.IsValueLessOrEqualWithNaNState(float.NaN, 1, false);
        span.IsValueGreaterThanWithNaNState(float.NaN, 1, false);
        span.IsValueGreaterOrEqualWithNaNState(float.NaN, 1, false);
        string[] expected = [
            "read:11:7", $"compare:11:-1:{int.MaxValue}:7:-1",
            "read:11:7", $"compare:11:-1:{int.MaxValue}:7:-1",
            "read:12:7", $"compare:-1:12:{int.MaxValue}:-1:7",
            "read:12:7", $"compare:-1:12:{int.MaxValue}:-1:7",
            "read:12:7", $"compare:-1:12:{int.MaxValue}:-1:7",
            "read:12:7", $"compare:-1:12:{int.MaxValue}:-1:7"];
        if (!events.SequenceEqual(expected)) throw new InvalidOperationException("Prepared comparison events changed.");
    }

    static void Check(bool actual, bool expected)
    {
        if (actual != expected) throw new InvalidOperationException("Prepared predicate disagrees with comparer order.");
    }

    readonly struct Descending<T> : IComparer<T> where T : unmanaged, IFloatingPointIeee754<T>
    {
        public int Compare(T a, T b) => b.CompareTo(a);
    }

    sealed class SpecialOrder<T> : IComparer<T> where T : unmanaged, IFloatingPointIeee754<T>
    {
        public int Compare(T a, T b)
        {
            var result = T.IsNaN(a) != T.IsNaN(b) ? (T.IsNaN(a) ? 1 : -1)
                : a == T.Zero && b == T.Zero && T.IsNegative(a) != T.IsNegative(b) ? (T.IsNegative(a) ? -1 : 1)
                : a.CompareTo(b);
            return result < 0 ? int.MinValue : result > 0 ? int.MaxValue : 0;
        }
    }
}
