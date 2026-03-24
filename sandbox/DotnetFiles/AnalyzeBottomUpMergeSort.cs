#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

Console.WriteLine("Bottom-Up Merge Sort Theoretical Values Analysis");
Console.WriteLine("=================================================");
Console.WriteLine();

// Test sorted data
Console.WriteLine("SORTED DATA:");
foreach (var n in new[] { 10, 20, 50, 100 })
{
    var stats = new StatisticsContext();
    var sorted = Enumerable.Range(0, n).ToArray();
    BottomUpMergeSort.Sort(sorted.AsSpan(), stats);
    
    Console.WriteLine($"n={n,3}: Compares={stats.CompareCount,4}, Writes={stats.IndexWriteCount,4}, Reads={stats.IndexReadCount,4}");
}

Console.WriteLine();
Console.WriteLine("REVERSED DATA:");
foreach (var n in new[] { 10, 20, 50, 100 })
{
    var stats = new StatisticsContext();
    var reversed = Enumerable.Range(0, n).Reverse().ToArray();
    BottomUpMergeSort.Sort(reversed.AsSpan(), stats);
    
    Console.WriteLine($"n={n,3}: Compares={stats.CompareCount,4}, Writes={stats.IndexWriteCount,4}, Reads={stats.IndexReadCount,4}");
}

Console.WriteLine();
Console.WriteLine("RANDOM DATA (average of 5 runs):");
foreach (var n in new[] { 10, 20, 50, 100 })
{
    var comparesSum = 0UL;
    var writesSum = 0UL;
    var readsSum = 0UL;
    
    for (var i = 0; i < 5; i++)
    {
        var stats = new StatisticsContext();
        var random = Enumerable.Range(0, n).OrderBy(_ => Guid.NewGuid()).ToArray();
        BottomUpMergeSort.Sort(random.AsSpan(), stats);
        
        comparesSum += stats.CompareCount;
        writesSum += stats.IndexWriteCount;
        readsSum += stats.IndexReadCount;
    }
    
    Console.WriteLine($"n={n,3}: Compares={comparesSum/5,4}, Writes={writesSum/5,4}, Reads={readsSum/5,4}");
}
