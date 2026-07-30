#:project ../../src/SortAlgorithm

using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

// Audit of two Boost fast paths that the C# port does not implement:
//   1. util::merge / util::merge_half MIN_CHECK (=1024) "already ordered" short-circuit
//   2. insert_partial_sort's std::upper_bound based insertion
static void Report(string name, int[] source)
{
    var array = source.ToArray();
    var stats = new StatisticsContext();
    SpinSort.Sort(array.AsSpan(), stats);

    var expected = source.ToArray();
    Array.Sort(expected);
    var ok = array.AsSpan().SequenceEqual(expected);

    Console.WriteLine($"{name,-42} n={source.Length,8}  compares={stats.CompareCount,10}  reads={stats.IndexReadCount,10}  writes={stats.IndexWriteCount,10}  sorted={ok}");
}

const int N = 1 << 17; // 131072

// (1) Block-partitioned data: values are already segregated by block, shuffled inside each
//     block. Once a block is sorted, every merge above the block level satisfies
//     left.last <= right.first, so Boost's MIN_CHECK collapses it into two bulk moves.
const int Block = 2048;
var rngA = new Random(1);
var ascendingBlocks = new int[N];
for (var i = 0; i < N; i++) ascendingBlocks[i] = (i / Block) * Block + rngA.Next(Block);

// same, but blocks in descending order: every merge hits the "right entirely below left" branch
var rngD = new Random(1);
var descendingBlocks = new int[N];
for (var i = 0; i < N; i++) descendingBlocks[i] = (N / Block - 1 - i / Block) * Block + rngD.Next(Block);

// (2) Sorted array with a small unsorted tail whose values belong at the front.
//     Hits check_stable_sort -> insert_partial_sort.
var tailLow = new int[N];
for (var i = 0; i < N; i++) tailLow[i] = i;
for (var i = 0; i < 32; i++) tailLow[N - 32 + i] = i * 2;

// tail values that belong at the END (best case for the linear merge)
var tailHigh = new int[N];
for (var i = 0; i < N; i++) tailHigh[i] = i;
for (var i = 0; i < 32; i++) tailHigh[N - 32 + i] = N - 32 + (31 - i);

var rng = new Random(42);
var random = new int[N];
for (var i = 0; i < N; i++) random[i] = rng.Next();

Report("ascending blocks (merge fast path)", ascendingBlocks);
Report("descending blocks (merge fast path)", descendingBlocks);
Report("sorted + 32 low tail (upper_bound)", tailLow);
Report("sorted + 32 high tail (upper_bound)", tailHigh);
Report("random (baseline)", random);

Console.WriteLine();
Console.WriteLine($"n log2 n = {(long)(N * Math.Log2(N)):N0}");
Console.WriteLine($"32 * log2(n/2) = {32 * (int)Math.Log2(N / 2)}");
