using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

/// <summary>
/// Verifies that <see cref="TimsortAdversaryGenerator"/> actually costs TimSort more than random
/// input, and that the structure it relies on to do so is present.
///
/// <para>
/// This file previously checked only that the generator returned a valid permutation and that
/// TimSort could sort it. Everything it asserted stayed true while the pattern was measuring
/// <b>0.7x</b> of random input - that is, while it was handing TimSort an input that was easier
/// than no pattern at all. Validity is not the property this generator exists to provide, so the
/// cost comparison is asserted here directly.
/// </para>
///
/// <para>
/// No quadratic case is asserted, and none exists: TimSort is a stable merge sort with an
/// O(n log n) bound and a comparison count already close to the information-theoretic minimum.
/// The realistic headroom is in merge cost, and it is a constant factor - measured 1.20x to 1.50x
/// depending on size.
/// </para>
/// </summary>
public class TimsortAdversaryGeneratorTests
{
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

    private static ulong TimSortCost(int[] input)
    {
        var stats = new StatisticsContext();
        var work = (int[])input.Clone();
        TimSort.Sort(work.AsSpan(), stats);

        // A cost figure only describes a real run, so confirm the run actually sorted.
        var expected = (int[])input.Clone();
        Array.Sort(expected);
        if (!work.SequenceEqual(expected))
            throw new InvalidOperationException("TimSort did not sort the adversary input.");

        return stats.CompareCount + stats.SwapCount + stats.IndexReadCount + stats.IndexWriteCount;
    }

    /// <summary>Length of the run TimSort would detect starting at <paramref name="lo"/>.</summary>
    private static int DetectRun(int[] array, int lo)
    {
        if (lo >= array.Length - 1) return array.Length - lo;

        var hi = lo + 1;
        if (array[hi] < array[lo])
        {
            while (hi < array.Length - 1 && array[hi + 1] < array[hi]) hi++;
        }
        else
        {
            while (hi < array.Length - 1 && array[hi + 1] >= array[hi]) hi++;
        }

        return hi - lo + 1;
    }

    // Historical regression: sizes 64..127 used to throw InvalidOperationException
    // ("Run normalization changed total") because the old run-length seed overshot the array.
    // The current construction has no run-length arithmetic to overshoot, but the size sweep is
    // kept - it is the cheapest guard against a whole class of off-by-one in a generator.
    [Test]
    public async Task GenerateProducesAValidPermutationForEverySizeUpTo1024()
    {
        var failures = new List<string>();

        for (var size = 1; size <= 1024; size++)
        {
            try
            {
                var array = ArrayPatterns.GenerateTimsortAdversary(size);
                if (array.Length != size)
                    failures.Add($"size={size}: length {array.Length}");
                else if (array.Distinct().Count() != size)
                    failures.Add($"size={size}: duplicate values");
                else if (array.Min() != 0 || array.Max() != size - 1)
                    failures.Add($"size={size}: not a permutation of [0, {size})");
            }
            catch (Exception ex)
            {
                failures.Add($"size={size}: {ex.GetType().Name} {ex.Message}");
            }
        }

        await Assert.That(failures).IsEmpty()
            .Because($"GenerateTimsortAdversary must produce a valid array for every size:\n{string.Join("\n", failures.Take(10))}" +
                     (failures.Count > 10 ? $"\n... and {failures.Count - 10} more" : ""));
    }

    [Test]
    [Arguments(63)]
    [Arguments(64)]
    [Arguments(65)]
    [Arguments(96)]
    [Arguments(127)]
    [Arguments(128)]
    [Arguments(1024)]
    public async Task GenerateProducesSortableInput(int size)
    {
        var array = ArrayPatterns.GenerateTimsortAdversary(size);
        await Assert.That(array.Length).IsEqualTo(size);

        var sorted = (int[])array.Clone();
        TimSort.Sort(sorted.AsSpan(), new StatisticsContext());

        var expected = (int[])array.Clone();
        Array.Sort(expected);
        await Assert.That(sorted.SequenceEqual(expected)).IsTrue();
    }

    [Test]
    public async Task GenerateAnswersSizeZeroWithAnEmptyArray()
    {
        // Asserted on the generator rather than on ArrayPatterns: the facade derives minRun from
        // TimSort.ComputeMinRun(size), which is 0 at size 0 and trips the minRun guard first.
        await Assert.That(TimsortAdversaryGenerator.Generate(0, 32)).IsEmpty();
    }

    [Test]
    public async Task GenerateRejectsInvalidArguments()
    {
        await Assert.That(() => TimsortAdversaryGenerator.Generate(-1, 32)).Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => TimsortAdversaryGenerator.Generate(64, 0)).Throws<ArgumentOutOfRangeException>();
        await Assert.That(() => TimsortAdversaryGenerator.Generate(64, -1)).Throws<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// The defining property. Sizes cover run counts that are and are not powers of two, because
    /// the merge tree is only balanced in the first case and a construction that assumes otherwise
    /// silently degrades in the second.
    /// </summary>
    [Test]
    [Arguments(1024)]
    [Arguments(5000)]
    [Arguments(8192)]
    [Arguments(32768)]
    public async Task CostsTimSortMoreThanRandomInput(int size)
    {
        var adversarial = TimSortCost(ArrayPatterns.GenerateTimsortAdversary(size));
        var random = TimSortCost(Shuffled(size, seed: 20260802));

        // Measured 1.20x-1.50x; the floor asserted here fails on a regression of the generator
        // rather than on ordinary drift in TimSort.
        await Assert.That((double)adversarial / random).IsGreaterThan(1.05)
            .Because($"n={size}: adversary {adversarial:N0} ops vs random {random:N0} ops. "
                   + "A pattern that costs no more than random input is not an adversary.");
    }

    /// <summary>
    /// The structure the cost rests on: TimSort must never find a run it can use as-is. Any run
    /// at or above minRun is one TimSort gets for free, and skipping the binary-insertion cost of
    /// even a fraction of the runs is what made the previous construction cheaper than random.
    /// </summary>
    [Test]
    [Arguments(1024)]
    [Arguments(5000)]
    [Arguments(32768)]
    public async Task GivesTimSortNoRunItCanUseForFree(int size)
    {
        var array = ArrayPatterns.GenerateTimsortAdversary(size);
        var minRun = TimSort.ComputeMinRun(size);

        // Walk the array the way TimSort does: detect a run, then extend it to minRun.
        var longest = 0;
        for (var pos = 0; pos < size;)
        {
            var detected = DetectRun(array, pos);
            longest = Math.Max(longest, detected);
            pos += Math.Max(detected, Math.Min(minRun, size - pos));
        }

        await Assert.That(longest).IsLessThan(minRun)
            .Because($"n={size}, minRun={minRun}: a detected run of {longest} is used as-is, so TimSort skips "
                   + "the binary insertion sort that this pattern exists to charge it for.");
    }
}
