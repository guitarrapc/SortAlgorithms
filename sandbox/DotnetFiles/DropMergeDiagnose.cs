#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:property Configuration=Release
#:project ../../src/SortAlgorithm

// Diagnose why DropMergeSort underperforms on its target workload (nearly-sorted data).
// Hypothesis: the extra early-out checks (undoCount > 16, totalBackTracked > n) that do NOT
// exist in the reference implementation (emilk/drop-merge-sort) trigger on realistic
// nearly-sorted inputs and silently degrade the algorithm to QuickSort fallback.
//
// This harness:
//   1. Runs an instrumented faithful Rust port (no extra checks) to count undo events etc.
//   2. Reports which fallback check the current library implementation would hit.
//   3. Times library DropMergeSort vs QuickSortMedian3 vs the faithful port.

using System.Diagnostics;
using SortAlgorithm.Algorithms;

// Disorder generator matching dmsort's benchmark: each element is either in place (value i)
// or replaced with a random value, with probability = disorderFactor.
static int[] GenDisorder(int n, double disorderFactor, int seed)
{
    var rng = new Random(seed);
    var a = new int[n];
    for (var i = 0; i < n; i++)
    {
        a[i] = rng.NextDouble() < disorderFactor ? rng.Next(n) : i;
    }
    return a;
}

static double TimeMs(Action action)
{
    // median of 5
    var times = new List<double>();
    for (var i = 0; i < 5; i++)
    {
        var sw = Stopwatch.StartNew();
        action();
        sw.Stop();
        times.Add(sw.Elapsed.TotalMilliseconds);
    }
    times.Sort();
    return times[2];
}

const int N = 1_000_000;
double[] disorders = [0.001, 0.01, 0.05, 0.10, 0.20, 0.40];

Console.WriteLine($"n = {N}");
Console.WriteLine();
Console.WriteLine("=== Instrumented faithful reference port: how often do undo/backtrack events occur? ===");
Console.WriteLine($"{"disorder",9} | {"dropped",9} | {"undoEvents",10} | {"totalBackTracked",16} | current-impl fallback?");
foreach (var d in disorders)
{
    var data = GenDisorder(N, d, seed: 42);
    var copy = (int[])data.Clone();
    var (sorted, droppedCount, undoEvents, totalBackTracked) = FaithfulDmsort.SortInstrumented(copy);
    if (!sorted) Console.WriteLine("  !! NOT SORTED !!");
    var fallback =
        undoEvents > 16 ? "YES: undoCount > 16" :
        totalBackTracked > N ? "YES: totalBackTracked > n" :
        "no";
    Console.WriteLine($"{d,9:P1} | {droppedCount,9} | {undoEvents,10} | {totalBackTracked,16} | {fallback}");
}

Console.WriteLine();
Console.WriteLine("=== Timings (median of 5, ms) ===");
Console.WriteLine($"{"disorder",9} | {"DropMergeSort(lib)",18} | {"QuickSortMedian3",16} | {"faithful port",13} | lib/qs | faithful/qs");
foreach (var d in disorders)
{
    var data = GenDisorder(N, d, seed: 42);

    var libMs = TimeMs(() => { var c = (int[])data.Clone(); DropMergeSort.Sort(c.AsSpan()); });
    var qsMs = TimeMs(() => { var c = (int[])data.Clone(); QuickSortMedian3.Sort(c.AsSpan()); });
    var faithMs = TimeMs(() => { var c = (int[])data.Clone(); FaithfulDmsort.SortInstrumented(c); });

    Console.WriteLine($"{d,9:P1} | {libMs,18:F1} | {qsMs,16:F1} | {faithMs,13:F1} | {libMs / qsMs,6:F2} | {faithMs / qsMs,6:F2}");
}

static class FaithfulDmsort
{
    const bool DoubleComparisons = true;
    const int Recency = 8;
    const bool FastBackTracking = true;
    const bool EarlyOut = true;
    const int EarlyOutTestAt = 4;
    const double EarlyOutDisorderFraction = 0.60;

    // Faithful port of the Rust reference (sort_copy_by), instrumented with counters.
    // Returns (isSorted, droppedCount, undoEvents, totalBackTracked).
    public static (bool sorted, int droppedCount, int undoEvents, long totalBackTracked) SortInstrumented(int[] slice)
    {
        var undoEvents = 0;
        long totalBackTracked = 0;

        var dropped = new List<int>();
        var numDroppedInRow = 0;
        var write = 0;
        var read = 0;
        long iteration = 0;
        var earlyOutStop = slice.Length / EarlyOutTestAt;

        while (read < slice.Length)
        {
            iteration++;
            if (EarlyOut
                && iteration == earlyOutStop
                && dropped.Count > read * EarlyOutDisorderFraction)
            {
                for (var i = 0; i < dropped.Count; i++) slice[write + i] = dropped[i];
                Array.Sort(slice);
                return (IsSorted(slice), dropped.Count, undoEvents, totalBackTracked);
            }

            if (write == 0 || slice[read] >= slice[write - 1])
            {
                slice[write] = slice[read];
                read++;
                write++;
                numDroppedInRow = 0;
            }
            else
            {
                if (DoubleComparisons
                    && numDroppedInRow == 0
                    && 2 <= write
                    && slice[read] >= slice[write - 2])
                {
                    dropped.Add(slice[write - 1]);
                    slice[write - 1] = slice[read];
                    read++;
                    continue;
                }

                if (numDroppedInRow < Recency)
                {
                    dropped.Add(slice[read]);
                    read++;
                    numDroppedInRow++;
                }
                else
                {
                    undoEvents++;

                    dropped.RemoveRange(dropped.Count - numDroppedInRow, numDroppedInRow);
                    read -= numDroppedInRow;

                    var numBackTracked = 1;
                    write--;

                    if (FastBackTracking)
                    {
                        var maxOfDropped = slice[read];
                        for (var i = 1; i < numDroppedInRow + 1; i++)
                        {
                            if (slice[read + i] > maxOfDropped) maxOfDropped = slice[read + i];
                        }
                        while (1 <= write && maxOfDropped < slice[write - 1])
                        {
                            numBackTracked++;
                            write--;
                        }
                    }

                    totalBackTracked += numBackTracked;

                    for (var i = 0; i < numBackTracked; i++)
                    {
                        dropped.Add(slice[write + i]);
                    }
                    numDroppedInRow = 0;
                }
            }
        }

        var droppedCount = dropped.Count;
        dropped.Sort();

        var back = slice.Length;
        var di = droppedCount - 1;
        while (di >= 0)
        {
            var lastDropped = dropped[di];
            while (0 < write && lastDropped < slice[write - 1])
            {
                slice[back - 1] = slice[write - 1];
                back--;
                write--;
            }
            slice[back - 1] = lastDropped;
            back--;
            di--;
        }

        return (IsSorted(slice), droppedCount, undoEvents, totalBackTracked);
    }

    static bool IsSorted(int[] a)
    {
        for (var i = 1; i < a.Length; i++)
        {
            if (a[i - 1] > a[i]) return false;
        }
        return true;
    }
}
