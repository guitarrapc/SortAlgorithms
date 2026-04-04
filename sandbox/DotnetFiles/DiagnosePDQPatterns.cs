#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Utils;

var rng = new Random(42);
var n = 8192;

// --- PipeOrgan: trace ninther pivot ---
var pipe = ArrayPatterns.GeneratePipeOrgan(n);
var s2 = n / 2;
int a0 = pipe[0], a_mid = pipe[s2], a_last = pipe[n-1];
Console.WriteLine($"PipeOrgan ninther candidates:");
Console.WriteLine($"  Group1: [{a0}, {a_mid}, {a_last}] -> median={Median(a0, a_mid, a_last)}");
Console.WriteLine($"  Group2: [{pipe[1]}, {pipe[s2-1]}, {pipe[n-2]}] -> median={Median(pipe[1], pipe[s2-1], pipe[n-2])}");
Console.WriteLine($"  Group3: [{pipe[2]}, {pipe[s2+1]}, {pipe[n-3]}] -> median={Median(pipe[2], pipe[s2+1], pipe[n-3])}");
// After three Sort3 calls, the medians end up at positions s2-1, s2, s2+1
// Simulate Sort3 inline:
var g1_med = Median(pipe[0], pipe[s2], pipe[n-1]);
var g2_med = Median(pipe[1], pipe[s2-1], pipe[n-2]);
var g3_med = Median(pipe[2], pipe[s2+1], pipe[n-3]);
var final_pivot = Median(g2_med, g1_med, g3_med); // position s2-1, s2, s2+1 after Sort3s
Console.WriteLine($"  Medians: [{g2_med}, {g1_med}, {g3_med}] -> final pivot~={final_pivot}");
Console.WriteLine($"  n={n}, pivot~={final_pivot} => left_partition~{final_pivot-1} elements ({100.0*(final_pivot-1)/n:F2}%)");

Console.WriteLine();

// --- SingleElementMoved: what does the first partition look like? ---
var moved = ArrayPatterns.GenerateSingleElementMoved(n, new Random(42));
// Count inversions to understand how "unsorted" it is
var inversions = 0;
for (var i = 0; i < moved.Length - 1; i++)
    if (moved[i] > moved[i+1]) inversions++;
Console.WriteLine($"SingleElementMoved: {inversions} adjacent inversions out of {n-1} pairs");

// Find where the out-of-place element is
var outOfPlaceCount = 0;
for (var i = 0; i < moved.Length; i++)
    if (moved[i] != i + 1) outOfPlaceCount++;
Console.WriteLine($"SingleElementMoved: {outOfPlaceCount} elements not in original position");

// Find the displaced region
int firstBad = -1, lastBad = -1;
for (var i = 0; i < moved.Length; i++)
    if (moved[i] != i + 1) { if (firstBad < 0) firstBad = i; lastBad = i; }
Console.WriteLine($"SingleElementMoved: displaced region [{firstBad}..{lastBad}], span={lastBad-firstBad+1}");
Console.WriteLine($"  PartialInsertionSort limit=8 would FAIL (span > 8) => falls back to full QuickSort");

static int Median(int a, int b, int c)
{
    if (a > b) (a, b) = (b, a);
    if (b > c) (b, c) = (c, b);
    if (a > b) (a, b) = (b, a);
    return b;
}
