#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:property Configuration=Release
#:project ../../src/SortAlgorithm

// Verify whether StrandSort's huge operation counts on reversed input are an implementation
// mistake or inherent to the algorithm.
//
// Method: implement the canonical linked-list strand sort (Wikipedia definition: repeatedly
// pull a greedy non-decreasing strand off the unsorted list, merge it into the result) with
// its own comparison counter, and compare against the library implementation's counts.
// If the canonical algorithm needs the same Θ(n²) comparisons on reversed input, the library
// implementation is faithful and the cost is algorithmic, not a bug.

using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

Console.WriteLine($"{"pattern",10} | {"n",6} | {"lib compares",12} | {"ref compares",12} | {"n(n-1)/2",10} | lib writes | lib reads");
foreach (var n in new[] { 100, 1000 })
{
    foreach (var pattern in new[] { "sorted", "reversed", "random" })
    {
        var rng = new Random(42);
        int[] data = pattern switch
        {
            "sorted" => Enumerable.Range(0, n).ToArray(),
            "reversed" => Enumerable.Range(0, n).Reverse().ToArray(),
            _ => Enumerable.Range(0, n).OrderBy(_ => rng.Next()).ToArray(),
        };

        var stats = new StatisticsContext();
        var libArray = data.ToArray();
        StrandSort.Sort(libArray.AsSpan(), stats);
        var libSorted = libArray.SequenceEqual(data.OrderBy(x => x));

        var (refResult, refCompares) = CanonicalStrandSort(data);
        var refSorted = refResult.SequenceEqual(data.OrderBy(x => x));
        if (!libSorted || !refSorted) Console.WriteLine("  !! NOT SORTED !!");

        Console.WriteLine($"{pattern,10} | {n,6} | {stats.CompareCount,12} | {refCompares,12} | {(ulong)n * ((ulong)n - 1) / 2,10} | {stats.IndexWriteCount,10} | {stats.IndexReadCount,9}");
    }
}

// Canonical strand sort per Wikipedia: linked lists, greedy strand extraction, merge per pass.
static (List<int> result, ulong compares) CanonicalStrandSort(int[] input)
{
    ulong compares = 0;
    var unsorted = new LinkedList<int>(input);
    var result = new LinkedList<int>();

    while (unsorted.Count > 0)
    {
        // Extract one greedy non-decreasing strand
        var strand = new List<int> { unsorted.First!.Value };
        unsorted.RemoveFirst();
        var node = unsorted.First;
        while (node is not null)
        {
            var next = node.Next;
            compares++;
            if (node.Value >= strand[^1])
            {
                strand.Add(node.Value);
                unsorted.Remove(node);
            }
            node = next;
        }

        // Merge strand into result
        var merged = new LinkedList<int>();
        var rNode = result.First;
        var sIdx = 0;
        while (rNode is not null && sIdx < strand.Count)
        {
            compares++;
            if (strand[sIdx] < rNode.Value)
            {
                merged.AddLast(strand[sIdx++]);
            }
            else
            {
                merged.AddLast(rNode.Value);
                rNode = rNode.Next;
            }
        }
        while (rNode is not null) { merged.AddLast(rNode.Value); rNode = rNode.Next; }
        while (sIdx < strand.Count) merged.AddLast(strand[sIdx++]);
        result = merged;
    }

    return (result.ToList(), compares);
}
