using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using SortAlgorithm.Utils;

Console.WriteLine("QuickSort Anti-Pattern Performance Test");
Console.WriteLine("========================================\n");

var random = new Random(42);
int size = 1000;

// Generate patterns
var patterns = new Dictionary<string, int[]>
{
    ["Random"] = ArrayPatterns.GenerateRandom(size, random),
    ["Sorted"] = ArrayPatterns.GenerateSorted(size),
    ["Reversed"] = ArrayPatterns.GenerateReversed(size),
    ["Sawtooth"] = ArrayPatterns.GenerateSawtooth(size),
    ["PipeOrgan"] = ArrayPatterns.GeneratePipeOrgan(size),
    ["Interleaved"] = ArrayPatterns.GenerateEvensReversedOddsInOrder(size),
};

Console.WriteLine($"Pattern          | Comparisons | Swaps    | IndexReads | IndexWrites");
Console.WriteLine("-----------------|-------------|----------|------------|------------");

ulong maxComparisons = 0;
string worstPattern = "";

foreach (var (name, pattern) in patterns)
{
    var stats = new StatisticsContext();
    var array = pattern.ToArray();
    
    QuickSort.Sort(array.AsSpan(), stats);
    
    Console.WriteLine($"{name,-16} | {stats.CompareCount,11:N0} | {stats.SwapCount,8:N0} | {stats.IndexReadCount,10:N0} | {stats.IndexWriteCount,11:N0}");
    
    if (stats.CompareCount > maxComparisons)
    {
        maxComparisons = stats.CompareCount;
        worstPattern = name;
    }
}

Console.WriteLine($"\nWorst pattern: {worstPattern} with {maxComparisons:N0} comparisons");
Console.WriteLine($"Theoretical worst case (n²/2): {size * size / 2:N0}");
Console.WriteLine($"Theoretical average case (2n ln n): {2.0 * size * Math.Log(size):N0}");
