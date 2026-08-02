#:project ../../src/SortAlgorithm

// Verifies that GeneratePdqSortAdversary is adversarial for PDQSort rather than merely
// scrambled, and sizes the cost for a visualization consumer.
//
// Reads as three questions:
//   1. How much does the pattern cost PDQSort compared with random and with other patterns?
//   2. Does PDQSort actually give up and fall back to heapsort?
//   3. How long does generating the pattern take, and what does it cost other algorithms?

using System.Diagnostics;
using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;
using SortAlgorithm.Utils;

Console.WriteLine("--- comparisons per n log2 n (PDQSort) ---");
foreach (var n in new[] { 1000, 10000, 100000 })
{
    Console.WriteLine($"n = {n}");
    Cost("random          ", Shuffled(n));
    Cost("sorted          ", [.. Enumerable.Range(0, n)]);
    Cost("reverse         ", [.. Enumerable.Range(0, n).Reverse()]);
    Cost("quickSortAdv    ", ArrayPatterns.GenerateQuickSortAdversary(n));
    Cost("pdqSortAdv      ", ArrayPatterns.GeneratePdqSortAdversary(n));
    Console.WriteLine();
}

Console.WriteLine("--- adaptive machinery PDQSort engages (n = 100000) ---");
Phases("random          ", Shuffled(100000));
Phases("pdqSortAdv      ", ArrayPatterns.GeneratePdqSortAdversary(100000));

Console.WriteLine();
Console.WriteLine("--- generation cost ---");
_ = ArrayPatterns.GeneratePdqSortAdversary(64); // warm up the JIT
foreach (var n in new[] { 1024, 4096, 16384, 32768 })
{
    var sw = Stopwatch.StartNew();
    for (var i = 0; i < 10; i++) _ = ArrayPatterns.GeneratePdqSortAdversary(n);
    sw.Stop();
    Console.WriteLine($"n={n,6}  {sw.Elapsed.TotalMilliseconds / 10,7:F3} ms/call");
}

Console.WriteLine();
Console.WriteLine("--- what the pattern costs other algorithms ---");
// The pattern is derived against PDQSort, so for anything else it is just an
// inversion-heavy permutation. Comparing it against reverse order bounds how much
// a consumer's recorded-operation budget can move.
Console.WriteLine($"{"algorithm",-20} {"size",6} {"random",12} {"pdqSortAdv",12} {"reverse",12}");
Others("pdqsort", 32768, (s, c) => PDQSort.Sort(s, c));
Others("pdqsortbranchless", 32768, (s, c) => PDQSortBranchless.Sort(s, c));
Others("introsort", 32768, (s, c) => IntroSort.Sort(s, c));
Others("stdsort", 32768, (s, c) => StdSort.Sort(s, c));
Others("timsort", 32768, (s, c) => TimSort.Sort(s, c));
Others("bubblesort", 2048, (s, c) => BubbleSort.Sort(s, c));
Others("insertionsort", 2048, (s, c) => InsertionSort.Sort(s, c));

static int[] Shuffled(int size)
{
    var random = new Random(20260802);
    var array = Enumerable.Range(0, size).ToArray();
    for (var i = array.Length - 1; i > 0; i--)
    {
        var j = random.Next(i + 1);
        (array[i], array[j]) = (array[j], array[i]);
    }
    return array;
}

static void Cost(string label, int[] data)
{
    var work = (int[])data.Clone();
    var stats = new StatisticsContext();
    PDQSort.Sort(work.AsSpan(), stats);

    var n = data.Length;
    Console.WriteLine($"  {label} compares={stats.CompareCount,12:N0}  c/(n log2 n)={stats.CompareCount / (n * Math.Log2(n)),5:F2}  sorted={IsSorted(work)}");
}

static void Phases(string label, int[] data)
{
    var work = (int[])data.Clone();
    var observer = new PhaseObservingContext();
    PDQSort.Sort(work.AsSpan(), observer);

    Console.WriteLine($"  {label} patternShuffles={observer.Shuffles,5}  heapSortFallbacks={observer.HeapFallbacks,4}  partialInsertionSorts={observer.PartialInsertions,5}  sorted={IsSorted(work)}");
}

static void Others(string label, int size, Action<Span<int>, StatisticsContext> sort)
{
    Console.WriteLine($"{label,-20} {size,6} {Ops(Shuffled(size), sort),12:N0} {Ops(ArrayPatterns.GeneratePdqSortAdversary(size), sort),12:N0} {Ops([.. Enumerable.Range(0, size).Reverse()], sort),12:N0}");
}

static ulong Ops(int[] data, Action<Span<int>, StatisticsContext> sort)
{
    var work = (int[])data.Clone();
    var stats = new StatisticsContext();
    sort(work.AsSpan(), stats);
    return stats.CompareCount + stats.SwapCount + stats.IndexReadCount + stats.IndexWriteCount;
}

static bool IsSorted(int[] array)
{
    for (var i = 1; i < array.Length; i++)
    {
        if (array[i - 1] > array[i]) return false;
    }
    return true;
}

sealed class PhaseObservingContext : ISortContext
{
    public int Shuffles;
    public int HeapFallbacks;
    public int PartialInsertions;

    public void OnPhase(SortPhase phase, int param1 = 0, int param2 = 0, int param3 = 0)
    {
        switch (phase)
        {
            case SortPhase.PDQPatternShuffle: Shuffles++; break;
            case SortPhase.HybridToHeapSort: HeapFallbacks++; break;
            case SortPhase.PDQPartialInsertionSort: PartialInsertions++; break;
        }
    }

    public void OnCompare(int i, int j, int result, int bufferIdI, int bufferIdJ) { }
    public void OnSwap(int i, int j, int bufferId) { }
    public void OnIndexRead(int index, int bufferId) { }
    public void OnIndexWrite(int index, int bufferId) { }
    public void OnIndexWrite<T>(int index, int bufferId, T value) { }
    public void OnRangeCopy(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId) { }
    public void OnRangeCopy<T>(int sourceIndex, int destinationIndex, int length, int sourceBufferId, int destinationBufferId, ReadOnlySpan<T> values) { }
    public void OnRole(int index, int bufferId, RoleType role) { }
}
