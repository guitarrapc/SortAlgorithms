#:project ../../src/SortAlgorithm

using SortAlgorithm.Algorithms;
using SortAlgorithm.Contexts;

// Reports AVL tree sort operation counts per input pattern, to compare the cost of the
// insertion retrace before and after the early-termination change.
const int N = 1024;

var patterns = new (string Name, int[] Data)[]
{
    ("Random", Shuffled(N, 42)),
    ("Sorted", [.. Enumerable.Range(0, N)]),
    ("Reversed", [.. Enumerable.Range(0, N).Reverse()]),
    ("ManyDuplicates", [.. Enumerable.Range(0, N).Select(i => i % 8)]),
};

Console.WriteLine($"n = {N}");
Console.WriteLine($"{"Pattern",-16}{"Compares",12}{"Reads",12}{"Writes",12}");
foreach (var (name, data) in patterns)
{
    var stats = new StatisticsContext();
    var work = data.ToArray();
    BalancedBinaryTreeSort.Sort(work.AsSpan(), stats);

    for (var i = 1; i < work.Length; i++)
    {
        if (work[i - 1] > work[i]) throw new InvalidOperationException($"{name}: not sorted at {i}");
    }

    Console.WriteLine($"{name,-16}{stats.CompareCount,12}{stats.IndexReadCount,12}{stats.IndexWriteCount,12}");
}

static int[] Shuffled(int n, int seed)
{
    var random = new Random(seed);
    var values = Enumerable.Range(0, n).ToArray();
    for (var i = values.Length - 1; i > 0; i--)
    {
        var j = random.Next(i + 1);
        (values[i], values[j]) = (values[j], values[i]);
    }
    return values;
}
