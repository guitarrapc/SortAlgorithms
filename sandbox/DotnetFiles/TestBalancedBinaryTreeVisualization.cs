#:sdk Microsoft.NET.Sdk.Web
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

// Test BalancedBinaryTreeSort with statistics
var stats = new StatisticsContext();

var data = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };
Console.WriteLine($"Original: [{string.Join(", ", data)}]");

BalancedBinaryTreeSort.Sort(data.AsSpan(), stats);

Console.WriteLine($"Sorted:   [{string.Join(", ", data)}]");
Console.WriteLine();
Console.WriteLine("Statistics:");
Console.WriteLine($"  Compares:     {stats.CompareCount}");
Console.WriteLine($"  Swaps:        {stats.SwapCount}");
Console.WriteLine($"  Index Reads:  {stats.IndexReadCount}");
Console.WriteLine($"  Index Writes: {stats.IndexWriteCount}");
Console.WriteLine();

// Test with visualization callbacks
int treeReadCount = 0;
int treeWriteCount = 0;
int mainReadCount = 0;
int mainWriteCount = 0;

var viz = new VisualizationContext(
    onIndexRead: (index, bufferId) =>
    {
        if (bufferId == 0) mainReadCount++;
        else if (bufferId == -1) treeReadCount++;
    },
    onIndexWrite: (index, bufferId, value) =>
    {
        if (bufferId == 0) mainWriteCount++;
        else if (bufferId == -1) treeWriteCount++;
    }
);

var data2 = new[] { 5, 3, 8, 1, 9, 2, 7, 4, 6 };
BalancedBinaryTreeSort.Sort(data2.AsSpan(), viz);

Console.WriteLine("Visualization tracking:");
Console.WriteLine($"  Main array reads:   {mainReadCount}");
Console.WriteLine($"  Main array writes:  {mainWriteCount}");
Console.WriteLine($"  Tree node reads:    {treeReadCount}");
Console.WriteLine($"  Tree node writes:   {treeWriteCount}");
Console.WriteLine();
Console.WriteLine("✅ BalancedBinaryTreeSort visualization test completed!");
