using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using System.Numerics;

namespace SortAlgorithm.Tests;

public class SortSpanFloatingOrderTests
{
    [Test]
    public void FloatingPredicatesPreserveComparerOrder()
    {
        Verify<Half>();
        Verify<float>();
        Verify<double>();
    }

    static void Verify<T>() where T : struct, IFloatingPointIeee754<T>
    {
        Verify<T, ComparableComparer<T>, NullContext>(default, default);
        Verify<T, ComparableComparer<T>, StatisticsContext>(default, new());
        Verify<T, DescendingComparer<T>, NullContext>(default, default);
        Verify<T, DescendingComparer<T>, StatisticsContext>(default, new());
    }

    static void Verify<T, TComparer, TContext>(TComparer comparer, TContext context)
        where T : struct, IFloatingPointIeee754<T>
        where TComparer : IComparer<T>
        where TContext : ISortContext
    {
        // Cartesian product covers either/both operands NaN, infinities, signed zero, and finite order/equality.
        T[] values = [T.NaN, -T.NaN, T.NegativeInfinity, -T.One, -T.Epsilon, -T.Zero,
            T.Zero, T.Epsilon, T.One, T.PositiveInfinity];
        foreach (var a in values)
        foreach (var b in values)
        {
            T[] pair = [a, b];
            var span = new SortSpan<T, TComparer, TContext>(pair, context, comparer, 0);
            var expected = comparer.Compare(a, b);
            Check(span.IsLessThan(a, b), expected < 0);
            Check(span.IsLessOrEqual(a, b), expected <= 0);
            Check(span.IsGreaterThan(a, b), expected > 0);
            Check(span.IsGreaterOrEqual(a, b), expected >= 0);
            Check(span.IsLessAt(0, 1), expected < 0);
            Check(span.IsGreaterAt(0, 1), expected > 0);
            Check(span.IsElementLessThan(0, b, out _), expected < 0);
            Check(span.IsElementGreaterThan(0, b, out _), expected > 0);
            Check(span.IsElementLessOrEqual(0, b), expected <= 0);
            Check(span.IsElementGreaterOrEqual(0, b), expected >= 0);
        }
    }

    static void Check(bool actual, bool expected)
    {
        if (actual != expected) throw new InvalidOperationException("SortSpan predicate disagrees with comparer order.");
    }

    readonly struct DescendingComparer<T> : IComparer<T> where T : struct, IFloatingPointIeee754<T>
    {
        public int Compare(T x, T y) => y.CompareTo(x);
    }
}
