using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using SortAlgorithm.Utils;

namespace SortAlgorithm.Tests;

public class TimsortAdversaryGeneratorTests
{
    // Regression: sizes 64..127 used to throw InvalidOperationException
    // ("Run normalization changed total"). In that range ComputeMinRun(size) is roughly
    // size/2, so the run seed [minRun, minRun+1] (2*minRun+1 elements) overshot the array
    // and the remainder went negative, which the trimming logic never handled.
    [Test]
    public async Task Generate_DoesNotThrowForAnySizeUpTo1024()
    {
        var failures = new List<string>();

        for (var size = 1; size <= 1024; size++)
        {
            try
            {
                var array = ArrayPatterns.GenerateTimsortDragAdversary(size);
                if (array.Length != size)
                    failures.Add($"size={size}: length {array.Length}");
                else if (array.Distinct().Count() != size)
                    failures.Add($"size={size}: duplicate values");
            }
            catch (Exception ex)
            {
                failures.Add($"size={size}: {ex.GetType().Name} {ex.Message}");
            }
        }

        await Assert.That(failures).IsEmpty()
            .Because($"GenerateTimsortDragAdversary must produce a valid array for every size:\n{string.Join("\n", failures.Take(10))}" +
                     (failures.Count > 10 ? $"\n... and {failures.Count - 10} more" : ""));
    }

    [Test]
    [Arguments(63)]   // GenerateTiny path (size <= minRun)
    [Arguments(64)]   // smallest previously-throwing size (two-run fallback)
    [Arguments(65)]
    [Arguments(96)]
    [Arguments(127)]  // largest previously-throwing size
    [Arguments(128)]  // smallest size where the Fibonacci-ish seed fits again
    [Arguments(1024)]
    public async Task Generate_ProducesSortableInput(int size)
    {
        var array = ArrayPatterns.GenerateTimsortDragAdversary(size);
        await Assert.That(array.Length).IsEqualTo(size);

        var sorted = (int[])array.Clone();
        TimSort.Sort(sorted.AsSpan(), new StatisticsContext());

        var expected = (int[])array.Clone();
        Array.Sort(expected);
        await Assert.That(sorted.SequenceEqual(expected)).IsTrue();
    }

    [Test]
    [Arguments(64)]
    [Arguments(127)]
    public async Task Generate_TwoRunFallback_IsPermutationOfRanks(int size)
    {
        // size in (minRun, 2*minRun] produces exactly the materialized two-run shape,
        // whose values are the ranks 0..size-1.
        var array = ArrayPatterns.GenerateTimsortDragAdversary(size);
        var sorted = (int[])array.Clone();
        Array.Sort(sorted);
        await Assert.That(sorted.SequenceEqual(Enumerable.Range(0, size))).IsTrue();
    }
}
