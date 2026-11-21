using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class BlitSortTests : StableSortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => BlitSort.Sort(span, context);

    // Up to 132 elements go to quadsort, whose tiny sort writes the n % 8 remainder of sorted input. Above that the analysis
    // recognises sorted input without writing.
    protected override CountExpectation SortedInputWrites => CountExpectation.Any;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    /// <summary>Records of the given keys, each carrying its position, sorted stably by key as the expected result.</summary>
    static (StabilityTestItem[] Input, StabilityTestItem[] Expected) Keyed(int[] keys)
    {
        var input = keys.Select((k, i) => new StabilityTestItem(k, i)).ToArray();
        return (input, input.OrderBy(x => x.Value).ToArray());
    }

    [Test]
    public async Task RangeSortTest()
    {
        var source = TestHelpers.ShuffledRange(3000, 5);
        var expected = source.ToArray();
        Array.Sort(expected, 7, 2990);
        var actual = source.ToArray();

        BlitSort.Sort(actual.AsSpan(), 7, 2997, new StatisticsContext());

        await Assert.That(actual).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task CustomComparerDescendingTest()
    {
        var array = TestHelpers.ShuffledRange(5000, 42);
        var descending = Comparer<int>.Create(static (a, b) => b.CompareTo(a));

        BlitSort.Sort(array.AsSpan(), descending, new StatisticsContext());

        await Assert.That(array).IsEquivalentTo(Enumerable.Range(0, 5000).Reverse().ToArray(), CollectionOrdering.Matching);
    }

    /// <summary>
    /// Elements that hold references are partitioned through pooled swap memory and handed to quadsort's inline stack array.
    /// </summary>
    [Test]
    [Arguments(100)]
    [Arguments(5000)]
    public async Task ReferenceContainingElementsSortStably(int n)
    {
        var random = new Random(n);
        var items = Enumerable.Range(0, n).Select(i => new StabilityTestItemWithId(random.Next(64), $"id{i}")).ToArray();
        var expected = items.OrderBy(x => x.Key).ToArray();

        BlitSort.Sort(items.AsSpan());

        await Assert.That(items).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    /// <summary>
    /// Every length up to 300 and the lengths around each threshold: quadsort up to 132, the analysis from 133, partitions that fit the
    /// 512 elements of swap memory and longer ones that rotate, the pseudomedian of nine up to 2,048 and the cube-root sample above it,
    /// under both contexts.
    /// </summary>
    [Test]
    public async Task EveryShapeAndLengthSortsStably()
    {
        var lengths = Enumerable.Range(0, 301).Concat([511, 512, 513, 1025, 2047, 2048, 2049, 2050, 5000, 20_000, 100_000]);

        foreach (var n in lengths)
        {
            foreach (var pattern in CrumSortTests.Patterns)
            {
                var (items, expected) = Keyed(CrumSortTests.Pattern(pattern, n, n));
                var unobserved = items.ToArray();

                BlitSort.Sort(items.AsSpan(), new StatisticsContext());
                BlitSort.Sort(unobserved.AsSpan());

                await Assert.That(items).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"{pattern} n={n}");
                await Assert.That(unobserved).IsEquivalentTo(expected, CollectionOrdering.Matching).Because($"{pattern} n={n} (NullContext)");
            }
        }
    }

    /// <summary>
    /// Blitsort is in place: whatever the length, its swap memory is the 512 elements of the reference, and every access to it stays there.
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
        var (items, expected) = Keyed(CrumSortTests.Pattern(pattern, 200_000, 3));

        BlitSort.Sort(items.AsSpan(), context);

        await Assert.That(highest).IsBetween(0, 511);
        await Assert.That(items).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }
}
