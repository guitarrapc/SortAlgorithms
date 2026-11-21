using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class CrumSortTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => CrumSort.Sort(span, context);

    // Up to 256 elements go to quadsort, whose tiny sort writes the n % 8 remainder of sorted input. Above that the analysis
    // recognises sorted input without writing.
    protected override CountExpectation SortedInputWrites => CountExpectation.Any;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    internal static int[] Pattern(string pattern, int n, int seed)
    {
        var random = new Random(seed);
        return pattern switch
        {
            "random" => Enumerable.Range(0, n).Select(_ => random.Next()).ToArray(),
            "sorted" => Enumerable.Range(0, n).ToArray(),
            "reversed" => Enumerable.Range(0, n).Reverse().ToArray(),
            "reversedDuplicates" => Enumerable.Range(0, n).Select(i => (n - i) / 3).ToArray(),
            "fewDistinct" => Enumerable.Range(0, n).Select(_ => random.Next(4)).ToArray(),
            "someDistinct" => Enumerable.Range(0, n).Select(_ => random.Next(100)).ToArray(),
            "saw" => Enumerable.Range(0, n).Select(i => i % 37).ToArray(),
            "pipeOrgan" => Enumerable.Range(0, n).Select(i => i < n / 2 ? i : n - i).ToArray(),
            "randomTail" => Enumerable.Range(0, n).Select(i => i < n * 3 / 4 ? i : random.Next(n)).ToArray(),
            "randomHalf" => Enumerable.Range(0, n).Select(i => i < n / 2 ? i : random.Next(n)).ToArray(),
            _ => throw new ArgumentException(pattern),
        };
    }

    internal static readonly string[] Patterns = ["random", "sorted", "reversed", "reversedDuplicates", "fewDistinct", "someDistinct", "saw", "pipeOrgan", "randomTail", "randomHalf"];

    [Test]
    public async Task RangeSortTest()
    {
        var source = TestHelpers.ShuffledRange(3000, 5);
        var expected = source.ToArray();
        Array.Sort(expected, 7, 2990);
        var actual = source.ToArray();

        CrumSort.Sort(actual.AsSpan(), 7, 2997, new StatisticsContext());

        await Assert.That(actual).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task CustomComparerDescendingTest()
    {
        var array = TestHelpers.ShuffledRange(5000, 42);
        var descending = Comparer<int>.Create(static (a, b) => b.CompareTo(a));

        CrumSort.Sort(array.AsSpan(), descending, new StatisticsContext());

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, 5000).Reverse().ToArray(), CollectionOrdering.Matching);
    }

    /// <summary>
    /// Every length up to 300 and the lengths around each threshold: quadsort up to 256, the analysis from 257, the median of nine up to
    /// 2,048 and the cube-root sample above it, under both contexts.
    /// </summary>
    [Test]
    public async Task EveryShapeAndLengthSorts()
    {
        var lengths = Enumerable.Range(0, 301).Concat([511, 512, 513, 2047, 2048, 2049, 2050, 5000, 20_000, 100_000]);

        foreach (var n in lengths)
        {
            foreach (var pattern in Patterns)
            {
                var observed = Pattern(pattern, n, n);
                var expected = observed.Order().ToArray();
                var unobserved = observed.ToArray();

                CrumSort.Sort(observed.AsSpan(), new StatisticsContext());
                CrumSort.Sort(unobserved.AsSpan());

                await Assert.That(observed).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"{pattern} n={n}");
                await Assert.That(unobserved).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"{pattern} n={n} (NullContext)");
            }
        }
    }

    /// <summary>
    /// Crumsort is in place: whatever the length, its swap memory is the 512 elements of the reference, and every access to it stays there.
    /// </summary>
    [Test]
    [Arguments("random")]
    [Arguments("fewDistinct")]
    [Arguments("randomHalf")]
    public async Task SwapMemoryStaysWithinItsFixedSize(string pattern)
    {
        var highest = -1;
        var context = new VisualizationContext(
            onIndexRead: (i, b) => { if (b == 1) highest = Math.Max(highest, i); },
            onIndexWrite: (i, b, _) => { if (b == 1) highest = Math.Max(highest, i); },
            onRangeCopy: (source, destination, length, sourceBuffer, destinationBuffer, _) =>
            {
                if (sourceBuffer == 1) highest = Math.Max(highest, source + length - 1);
                if (destinationBuffer == 1) highest = Math.Max(highest, destination + length - 1);
            });
        var array = Pattern(pattern, 200_000, 3);

        CrumSort.Sort(array.AsSpan(), context);

        await Assert.That(highest).IsBetween(0, 511);
        await Assert.That(array).IsEquivalentTo(array.Order().ToArray(), CollectionOrdering.Matching);
    }
}
