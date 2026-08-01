#:project ../../src/SortAlgorithm

using SortAlgorithm.Algorithms;
using SortAlgorithm.Utils;

// Verifies the shape of the ManyDuplicates benchmark pattern and that every
// DistributionBenchmark algorithm sorts it correctly.
foreach (var size in new[] { 256, 1024, 4096, 8192 })
{
    var source = ArrayPatterns.GenerateManyDuplicates(size, new Random(42));
    var distinct = source.Distinct().OrderBy(x => x).ToArray();
    var expected = source.OrderBy(x => x).ToArray();
    var range = distinct[^1] - distinct[0] + 1;
    var maxRun = source.GroupBy(x => x).Max(g => g.Count());

    Console.WriteLine($"size={size} distinct={distinct.Length} range={range} n/k={size / (double)distinct.Length:F1} maxRun={maxRun}");

    Check("CountingSort", a => CountingSort.SortBy(a.AsSpan(), x => x));
    Check("CountingSortInteger", a => CountingSortInteger.Sort(a.AsSpan()));
    Check("PigeonholeSort", a => PigeonholeSort.SortBy(a.AsSpan(), x => x));
    Check("PigeonholeSortInteger", a => PigeonholeSortInteger.Sort(a.AsSpan()));
    Check("BucketSort", a => BucketSort.SortBy(a.AsSpan(), x => x));
    Check("BucketSortInteger", a => BucketSortInteger.Sort(a.AsSpan()));
    Check("FlashSort", a => FlashSort.Sort(a.AsSpan()));
    Check("RadixLSD4Sort", a => RadixLSD4Sort.Sort(a.AsSpan()));
    Check("RadixLSD256Sort", a => RadixLSD256Sort.Sort(a.AsSpan()));
    Check("RadixLSD10Sort", a => RadixLSD10Sort.Sort(a.AsSpan()));
    Check("RadixMSD4Sort", a => RadixMSD4Sort.Sort(a.AsSpan()));
    Check("RadixMSD10Sort", a => RadixMSD10Sort.Sort(a.AsSpan()));
    Check("AmericanFlagSort", a => AmericanFlagSort.Sort(a.AsSpan()));
    Check("SpreadSort", a => SpreadSort.Sort(a.AsSpan()));

    void Check(string name, Action<int[]> sort)
    {
        var actual = source.ToArray();
        try
        {
            sort(actual);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  {name,-22} THREW {ex.GetType().Name}: {ex.Message}");
            return;
        }
        if (!actual.SequenceEqual(expected)) Console.WriteLine($"  {name,-22} WRONG");
    }
}

Console.WriteLine("done (only failures are printed above)");
