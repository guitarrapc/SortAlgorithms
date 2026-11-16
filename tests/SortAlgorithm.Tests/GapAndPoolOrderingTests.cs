using System.Numerics;
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

namespace SortAlgorithm.Tests;

public class GapAndPoolOrderingTests
{
    [Test]
    [Arguments("ShellSortKnuth1973")]
    [Arguments("ShellSortSedgewick1986")]
    [Arguments("ShellSortTokuda1992")]
    [Arguments("ShellSortCiura2001")]
    [Arguments("ShellSortLee2021")]
    [Arguments("TimSort")]
    [Arguments("PowerSort")]
    public void FloatingOrderingAndBitsRemainCorrect(string algorithm)
    {
        Verify<Half>(algorithm);
        Verify<float>(algorithm);
        Verify<double>(algorithm);
    }

    static void Verify<T>(string algorithm) where T : unmanaged, IFloatingPointIeee754<T>
    {
        foreach (var length in new[] { 0, 1, 2, 3, 16, 33, 65, 257, 1024 })
        foreach (var allNaN in new[] { false, true })
        {
            var input = Enumerable.Range(0, length).Select(i => allNaN || i % 4 == 0
                ? PreparedFloatingSortCorrectnessTests.NaN<T>(allNaN ? i % 4 : i / 4 % 4)
                : i % 4 == 1 ? -T.Zero : i % 4 == 2 ? T.Zero : T.CreateChecked(i % 17 - 8)).ToArray();
            foreach (var comparer in new IComparer<T>?[] { null, Comparer<T>.Default,
                Comparer<T>.Create((x, y) => y.CompareTo(x)),
                Comparer<T>.Create((x, y) => PreparedFloatingSortCorrectnessTests.Bits(x).CompareTo(PreparedFloatingSortCorrectnessTests.Bits(y))) })
            {
                Check(algorithm, input, comparer, NullContext.Default);
                Check(algorithm, input, comparer, new StatisticsContext());
            }
        }
    }

    static void Check<T, TContext>(string algorithm, T[] input, IComparer<T>? comparer, TContext context)
        where T : unmanaged, IFloatingPointIeee754<T> where TContext : ISortContext
    {
        var sentinel = T.CreateChecked(987);
        var actual = Enumerable.Repeat(sentinel, input.Length + 4).ToArray();
        input.CopyTo(actual, 2);
        var span = actual.AsSpan(2, input.Length);
        switch (algorithm)
        {
            case "ShellSortKnuth1973":
                if (comparer is null) ShellSortKnuth1973.Sort(span, context); else ShellSortKnuth1973.Sort(span, comparer, context);
                break;
            case "ShellSortSedgewick1986":
                if (comparer is null) ShellSortSedgewick1986.Sort(span, context); else ShellSortSedgewick1986.Sort(span, comparer, context);
                break;
            case "ShellSortTokuda1992":
                if (comparer is null) ShellSortTokuda1992.Sort(span, context); else ShellSortTokuda1992.Sort(span, comparer, context);
                break;
            case "ShellSortCiura2001":
                if (comparer is null) ShellSortCiura2001.Sort(span, context); else ShellSortCiura2001.Sort(span, comparer, context);
                break;
            case "ShellSortLee2021":
                if (comparer is null) ShellSortLee2021.Sort(span, context); else ShellSortLee2021.Sort(span, comparer, context);
                break;
            case "TimSort":
                if (comparer is null) TimSort.Sort(span, context); else TimSort.Sort(span, comparer, context);
                break;
            case "PowerSort":
                if (comparer is null) PowerSort.Sort(span, context); else PowerSort.Sort(span, comparer, context);
                break;
            default: throw new ArgumentOutOfRangeException(nameof(algorithm));
        }
        var output = span.ToArray();
        var expected = input.OrderBy(x => x, comparer ?? Comparer<T>.Default).ToArray();
        for (var i = 0; i < output.Length; i++)
            if ((comparer ?? Comparer<T>.Default).Compare(output[i], expected[i]) != 0) throw new InvalidOperationException("Wrong floating order.");
        if (algorithm is "TimSort" or "PowerSort" && !output.Select(PreparedFloatingSortCorrectnessTests.Bits).SequenceEqual(expected.Select(PreparedFloatingSortCorrectnessTests.Bits)))
            throw new InvalidOperationException("Stable equal-key order changed.");
        if (!output.Select(PreparedFloatingSortCorrectnessTests.Bits).Order().SequenceEqual(input.Select(PreparedFloatingSortCorrectnessTests.Bits).Order()))
            throw new InvalidOperationException("Element bits changed.");
        if (actual[0] != sentinel || actual[1] != sentinel || actual[^2] != sentinel || actual[^1] != sentinel)
            throw new InvalidOperationException("An element outside the sorted range changed.");
    }
}
