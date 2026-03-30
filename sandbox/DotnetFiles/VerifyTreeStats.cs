#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

Console.WriteLine("=== BinaryTreeSort ===");
foreach (var n in new[] { 10, 20, 50, 100 })
{
    var stats = new StatisticsContext();
    var sorted = Enumerable.Range(0, n).ToArray();
    BinaryTreeSort.Sort(sorted.AsSpan(), stats);
    Console.WriteLine($"Sorted   n={n}: C={stats.CompareCount}, R={stats.IndexReadCount}, W={stats.IndexWriteCount}");

    stats.Reset();
    var reversed = Enumerable.Range(0, n).Reverse().ToArray();
    BinaryTreeSort.Sort(reversed.AsSpan(), stats);
    Console.WriteLine($"Reversed n={n}: C={stats.CompareCount}, R={stats.IndexReadCount}, W={stats.IndexWriteCount}");
}
foreach (var n in new[] { 7, 15, 31 })
{
    var stats = new StatisticsContext();
    var balanced = CreateBalancedInsertionOrder(n);
    BinaryTreeSort.Sort(balanced, stats);
    Console.WriteLine($"Balanced n={n}: C={stats.CompareCount}, R={stats.IndexReadCount}, W={stats.IndexWriteCount}");
}

Console.WriteLine();
Console.WriteLine("=== BalancedBinaryTreeSort ===");
foreach (var n in new[] { 10, 20, 50, 100 })
{
    var stats = new StatisticsContext();
    var sorted = Enumerable.Range(0, n).ToArray();
    BalancedBinaryTreeSort.Sort(sorted.AsSpan(), stats);
    Console.WriteLine($"Sorted   n={n}: C={stats.CompareCount}, R={stats.IndexReadCount}, W={stats.IndexWriteCount}");

    stats.Reset();
    var reversed = Enumerable.Range(0, n).Reverse().ToArray();
    BalancedBinaryTreeSort.Sort(reversed.AsSpan(), stats);
    Console.WriteLine($"Reversed n={n}: C={stats.CompareCount}, R={stats.IndexReadCount}, W={stats.IndexWriteCount}");
}

Console.WriteLine();
Console.WriteLine("=== TreapSort ===");
foreach (var n in new[] { 10, 20, 50, 100 })
{
    var stats = new StatisticsContext();
    var sorted = Enumerable.Range(0, n).ToArray();
    TreapSort.Sort(sorted.AsSpan(), stats);
    Console.WriteLine($"Sorted   n={n}: C={stats.CompareCount}, R={stats.IndexReadCount}, W={stats.IndexWriteCount}");

    stats.Reset();
    var reversed = Enumerable.Range(0, n).Reverse().ToArray();
    TreapSort.Sort(reversed.AsSpan(), stats);
    Console.WriteLine($"Reversed n={n}: C={stats.CompareCount}, R={stats.IndexReadCount}, W={stats.IndexWriteCount}");
}

Console.WriteLine();
Console.WriteLine("=== SplaySort ===");
foreach (var n in new[] { 10, 20, 50, 100 })
{
    var stats = new StatisticsContext();
    var sorted = Enumerable.Range(0, n).ToArray();
    SplaySort.Sort(sorted.AsSpan(), stats);
    Console.WriteLine($"Sorted   n={n}: C={stats.CompareCount}, R={stats.IndexReadCount}, W={stats.IndexWriteCount}");

    stats.Reset();
    var reversed = Enumerable.Range(0, n).Reverse().ToArray();
    SplaySort.Sort(reversed.AsSpan(), stats);
    Console.WriteLine($"Reversed n={n}: C={stats.CompareCount}, R={stats.IndexReadCount}, W={stats.IndexWriteCount}");
}

static int[] CreateBalancedInsertionOrder(int n)
{
    var result = new int[n];
    var index = 0;
    var queue = new Queue<(int, int)>();
    queue.Enqueue((0, n - 1));
    while (queue.Count > 0)
    {
        var (lo, hi) = queue.Dequeue();
        if (lo > hi) continue;
        var mid = lo + (hi - lo) / 2;
        result[index++] = mid;
        queue.Enqueue((lo, mid - 1));
        queue.Enqueue((mid + 1, hi));
    }
    return result;
}
