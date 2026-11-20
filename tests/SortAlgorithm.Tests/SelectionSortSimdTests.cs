using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using TUnit.Assertions.Enums;

namespace SortAlgorithm.Tests;

[InheritsTests]
public class SelectionSortSimdTests : SortTestsBase
{
    protected override void Sort<T, TContext>(Span<T> span, TContext context)
        => SelectionSortSimd.Sort(span, context);

    // O(n^2) comparisons: skip large inputs in data-driven tests.
    protected override int MaxOrderTestSize => 512;

    // Sorted input keeps min at the boundary each pass: no writes, no swaps.
    protected override CountExpectation SortedInputWrites => CountExpectation.Zero;
    protected override CountExpectation SortedInputSwaps => CountExpectation.Zero;

    [Test]
    public async Task SameSwapsAsSelectionSortOnEveryVectorizedType()
    {
        List<string> problems =
        [
            .. Check(x => (int)x), .. Check(x => (uint)x), .. Check(x => x), .. Check(x => (ulong)x),
            .. Check(x => (short)x), .. Check(x => (ushort)x), .. Check(x => (byte)x), .. Check(x => (sbyte)x),
            .. Check(x => (nint)x), .. Check(x => (nuint)x),
        ];

        // The vector scan takes the same first minimum, so every exchange must match selection sort's, observed or not.
        await Assert.That(problems).IsEmpty();

        static List<string> Check<T>(Func<long, T> convert) where T : IComparable<T>
            => SimdSelectionEquivalence.Mismatches(42, convert,
                static (span, context) => SelectionSort.Sort(span, context),
                static (span, context) => SelectionSortSimd.Sort(span, context),
                static span => SelectionSortSimd.Sort(span));
    }

    [Test]
    [Arguments(2)]
    [Arguments(15)]
    [Arguments(16)]
    [Arguments(17)]
    [Arguments(64)]
    [Arguments(100)]
    [Arguments(257)]
    public async Task SortedInputCountsFollowTheLaneCount(int n)
    {
        var w = SelectionVectorScan.LaneCount<int>();
        var stats = new StatisticsContext();
        SelectionSortSimd.Sort(Enumerable.Range(0, n).ToArray().AsSpan(), stats);

        // A pass over a suffix of k >= w elements reads all k once and compares k - w of them in lanes or one by one, reduces the
        // w lanes with w - 1 comparisons, and finds the minimum in the first vector of the second scan (w reads, w comparisons).
        // The suffix left narrower than one vector is selection sort's.
        ulong compares = 0, reads = 0;
        var k = n;
        for (; w > 0 && k >= w; k--)
        {
            compares += (ulong)(k + w - 1);
            reads += (ulong)(k + w);
        }
        var rest = new StatisticsContext();
        SelectionSort.Sort(Enumerable.Range(0, k).ToArray().AsSpan(), rest);

        await Assert.That(stats.CompareCount).IsEqualTo(compares + rest.CompareCount);
        await Assert.That(stats.IndexReadCount).IsEqualTo(reads + rest.IndexReadCount);
        await Assert.That(stats.SwapCount).IsEqualTo(0UL);
    }

    [Test]
    public async Task FirstScanComparesEachElementWithItsLanesRunningMinimum()
    {
        var w = SelectionVectorScan.LaneCount<int>();
        Skip.When(w == 0, "No vector width is accelerated on this machine.");

        var n = 5 * w + 3;
        var source = TestHelpers.ShuffledRange(n, 7);
        var compares = new List<(int I, int J, int Result)>();
        var phases = 0;
        var context = new VisualizationContext(
            onCompare: (i, j, result, _, _) => { if (phases == 1) compares.Add((i, j, Math.Sign(result))); },
            onPhase: (_, _, _, _) => phases++);
        SelectionSortSimd.Sort(source.ToArray().AsSpan(), context);

        // The first scan is every comparison of the first pass before the lane reduction, which compares held values only.
        // Lane l of vector b compares the element at b*w + l with the minimum of lane l over the vectors before it.
        var scan = compares.TakeWhile(c => c.I >= 0).ToList();
        var expected = new List<(int I, int J, int Result)>();
        var lanes = source[..w];
        for (var b = 1; b < n / w; b++)
        {
            for (var l = 0; l < w; l++)
            {
                var x = source[b * w + l];
                expected.Add((b * w + l, -1, Math.Sign(x.CompareTo(lanes[l]))));
                lanes[l] = Math.Min(lanes[l], x);
            }
        }

        await Assert.That(scan).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    [Test]
    public async Task CustomComparerRunsSelectionSort()
    {
        var source = TestHelpers.ShuffledRange(200, 3);
        var expected = source.ToArray();
        var expectedSwaps = new List<(int, int)>();
        SelectionSort.Sort(expected.AsSpan(), new DescendingInt(), new VisualizationContext(onSwap: (i, j, _) => expectedSwaps.Add((i, j))));
        var actual = source.ToArray();
        var actualSwaps = new List<(int, int)>();
        SelectionSortSimd.Sort(actual.AsSpan(), new DescendingInt(), new VisualizationContext(onSwap: (i, j, _) => actualSwaps.Add((i, j))));

        await Assert.That(actual).IsEquivalentTo(Enumerable.Range(0, 200).Reverse().ToArray(), CollectionOrdering.Matching);
        await Assert.That(actualSwaps).IsEquivalentTo(expectedSwaps, CollectionOrdering.Matching);
    }

    [Test]
    public async Task RangeSortLeavesOutsideUntouched()
    {
        var source = TestHelpers.ShuffledRange(160, 11);
        var expected = source.ToArray();
        SelectionSort.Sort(expected.AsSpan(), 3, 153, NullContext.Default);
        var actual = source.ToArray();
        SelectionSortSimd.Sort(actual.AsSpan(), 3, 153, NullContext.Default);

        await Assert.That(actual).IsEquivalentTo(expected, CollectionOrdering.Matching);
    }

    readonly struct DescendingInt : IComparer<int>
    {
        public int Compare(int x, int y) => y.CompareTo(x);
    }
}

/// <summary>
/// Runs a scalar selection sort and its SIMD version on the same inputs and reports every input where their exchanges or results differ.
/// </summary>
internal static class SimdSelectionEquivalence
{
    public delegate void ObservedSort<T>(Span<T> span, VisualizationContext context);
    public delegate void UnobservedSort<T>(Span<T> span);

    // Every size up to well past two 512-bit vectors of int, plus sizes around the vector multiples of the narrow types.
    static readonly int[] Sizes = [.. Enumerable.Range(0, 140), 191, 192, 193, 255, 256, 257, 300];

    public static List<string> Mismatches<T>(int seed, Func<long, T> convert, ObservedSort<T> scalar, ObservedSort<T> simd, UnobservedSort<T> simdUnobserved)
    {
        var random = new Random(seed);
        var problems = new List<string>();
        foreach (var n in Sizes)
        {
            for (var pattern = 0; pattern < 6; pattern++)
            {
                var source = new T[n];
                for (var i = 0; i < n; i++)
                {
                    source[i] = convert(pattern switch
                    {
                        0 => i,
                        1 => n - i,
                        2 => random.Next(-1_000_000, 1_000_000),
                        3 => random.Next(4),
                        4 => (i & 1) == 0 ? long.MinValue : long.MaxValue,
                        _ => random.NextInt64(long.MinValue, long.MaxValue),
                    });
                }

                var expected = (T[])source.Clone();
                var expectedSwaps = new List<(int, int)>();
                scalar(expected, new VisualizationContext(onSwap: (i, j, _) => expectedSwaps.Add((i, j))));
                var actual = (T[])source.Clone();
                var actualSwaps = new List<(int, int)>();
                simd(actual, new VisualizationContext(onSwap: (i, j, _) => actualSwaps.Add((i, j))));
                var unobserved = (T[])source.Clone();
                simdUnobserved(unobserved);

                if (!actualSwaps.SequenceEqual(expectedSwaps) || !actual.SequenceEqual(expected) || !unobserved.SequenceEqual(expected))
                {
                    problems.Add($"{typeof(T).Name}: n = {n}, pattern {pattern}");
                }
            }
        }
        return problems;
    }
}
