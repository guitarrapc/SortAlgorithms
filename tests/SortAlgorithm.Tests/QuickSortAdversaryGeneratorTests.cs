using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

/// <summary>
/// Verifies that <see cref="QuickSortAdversaryGenerator"/> actually forces quadratic behavior,
/// and forces it on the whole middle-pivot family rather than on one member.
///
/// <para>
/// The arrangement this replaced was quadratic for <see cref="QuickSort"/> only, at about a
/// quarter of the achievable comparison count, and cost <see cref="QuickSortMedian3"/> - the
/// variant it was documented as targeting - barely more than random input. Both halves of that
/// failure are asserted against here: the count has to be quadratic, and it has to be quadratic
/// for every variant that pivots on the middle element.
/// </para>
///
/// <para>
/// The variants that sample differently are asserted to stay near random on purpose. A single
/// array cannot be worst-case for every pivot rule at once, so "this input is bad for all
/// quicksorts" would be a claim the pattern cannot keep; pinning down which variants it does and
/// does not defeat is what keeps the documentation honest.
/// </para>
/// </summary>
public class QuickSortAdversaryGeneratorTests
{
    // Large enough that quadratic and n log n are unmistakably apart (n^2 / 8 = 500,000 against
    // roughly 1.4 n log2 n = 31,000 for random input), small enough to stay fast.
    private const int MeasuredSize = 2000;

    private static ulong Compares(int[] input, Action<Span<int>, StatisticsContext> sort)
    {
        var stats = new StatisticsContext();
        var work = (int[])input.Clone();
        sort(work.AsSpan(), stats);

        // Statistics only describe a real run, so confirm the run actually sorted.
        var expected = (int[])input.Clone();
        Array.Sort(expected);
        if (!work.SequenceEqual(expected))
            throw new InvalidOperationException("the sort did not sort the adversary input.");

        return stats.CompareCount;
    }

    private static int[] Shuffled(int size, int seed)
    {
        var random = new Random(seed);
        var array = Enumerable.Range(0, size).ToArray();
        for (var i = array.Length - 1; i > 0; i--)
        {
            var j = random.Next(i + 1);
            (array[i], array[j]) = (array[j], array[i]);
        }
        return array;
    }

    private static Action<Span<int>, StatisticsContext> Variant(string name) => name switch
    {
        nameof(QuickSort) => (s, c) => QuickSort.Sort(s, c),
        nameof(QuickSortMedian3) => (s, c) => QuickSortMedian3.Sort(s, c),
        nameof(QuickSort3way) => (s, c) => QuickSort3way.Sort(s, c),
        nameof(QuickSortMedian9) => (s, c) => QuickSortMedian9.Sort(s, c),
        nameof(StableQuickSort) => (s, c) => StableQuickSort.Sort(s, c),
        nameof(DualPivotQuickSort) => (s, c) => DualPivotQuickSort.Sort(s, c),
        _ => throw new ArgumentOutOfRangeException(nameof(name), name, null),
    };

    [Test]
    [Arguments(1)]
    [Arguments(2)]
    [Arguments(3)]
    [Arguments(17)]
    [Arguments(64)]
    [Arguments(127)]
    [Arguments(128)]
    [Arguments(1000)]
    public async Task GenerateProducesPermutationOfRange(int size)
    {
        var generated = ArrayPatterns.GenerateQuickSortAdversary(size);

        await Assert.That(generated.Length).IsEqualTo(size);
        await Assert.That(generated.Order()).IsEquivalentTo(Enumerable.Range(0, size));
    }

    /// <summary>
    /// The adversary decides values while QuickSort runs, so determinism is a property of the
    /// generator rather than an obvious consequence of its shape. Consumers that cache the pattern
    /// - and at these generation costs they should - depend on it.
    /// </summary>
    [Test]
    [Arguments(100)]
    [Arguments(1500)]
    public async Task GenerateIsDeterministic(int size)
    {
        var first = ArrayPatterns.GenerateQuickSortAdversary(size);
        var second = ArrayPatterns.GenerateQuickSortAdversary(size);

        await Assert.That(second).IsEquivalentTo(first);
    }

    /// <summary>This overload has always answered a non-positive size with an empty array.</summary>
    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    public async Task GenerateAnswersNonPositiveSizeWithEmptyArray(int size)
    {
        await Assert.That(ArrayPatterns.GenerateQuickSortAdversary(size)).IsEmpty();
    }

    [Test]
    [Arguments(0)]
    [Arguments(-1)]
    public async Task GeneratorRejectsNonPositiveLength(int length)
    {
        await Assert.That(() => QuickSortAdversaryGenerator.Generate(length))
            .Throws<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// The defining property. QuickSort, QuickSortMedian3 and QuickSort3way all end up pivoting on
    /// the middle element, which is exactly the element the adversary keeps undecided, so one array
    /// is quadratic for all three.
    /// </summary>
    [Test]
    [Arguments(nameof(QuickSort))]
    [Arguments(nameof(QuickSortMedian3))]
    [Arguments(nameof(QuickSort3way))]
    public async Task MiddlePivotVariantsGoQuadratic(string variant)
    {
        var sort = Variant(variant);
        var adversarial = Compares(ArrayPatterns.GenerateQuickSortAdversary(MeasuredSize), sort);

        // Measured at ~n^2 / 4; half of that leaves room for implementation drift while staying
        // an order of magnitude above anything O(n log n) can reach.
        await Assert.That(adversarial).IsGreaterThan((ulong)MeasuredSize * MeasuredSize / 8);
    }

    /// <summary>
    /// Random input must stay far below the quadratic floor, or the assertion above would pass on
    /// an ordinary permutation and prove nothing.
    /// </summary>
    [Test]
    [Arguments(nameof(QuickSort))]
    [Arguments(nameof(QuickSortMedian3))]
    [Arguments(nameof(QuickSort3way))]
    public async Task RandomInputStaysBelowTheQuadraticFloor(string variant)
    {
        var random = Compares(Shuffled(MeasuredSize, seed: 20260802), Variant(variant));

        await Assert.That(random).IsLessThan((ulong)MeasuredSize * MeasuredSize / 8);
    }

    /// <summary>
    /// Variants that sample elsewhere are not defeated, and the pattern must not be described as
    /// if they were. The ninther reads nine positions instead of one; the stable variants pivot on
    /// quartiles. Each needs its own killer, which is a different array.
    /// </summary>
    [Test]
    [Arguments(nameof(QuickSortMedian9))]
    [Arguments(nameof(StableQuickSort))]
    [Arguments(nameof(DualPivotQuickSort))]
    public async Task OtherPivotRulesAreNotDefeatedByThisInput(string variant)
    {
        var sort = Variant(variant);
        var adversarial = Compares(ArrayPatterns.GenerateQuickSortAdversary(MeasuredSize), sort);
        var random = Compares(Shuffled(MeasuredSize, seed: 20260802), sort);

        await Assert.That(adversarial).IsLessThan(random * 4);
    }

    [Test]
    public async Task IntKeyOverloadMatchesTheIntPattern()
    {
        var keys = ArrayPatterns.GenerateQuickSortAdversaryIntKey(500);
        var expected = ArrayPatterns.GenerateQuickSortAdversary(500);

        await Assert.That(keys.Select(x => x.Key)).IsEquivalentTo(expected);
    }
}
