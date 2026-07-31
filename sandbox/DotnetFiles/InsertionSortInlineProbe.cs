#:project ../../src/SortAlgorithm

// Probe: does the JIT actually inline InsertionSort.SortCore into its callers?
//
// The [MethodImpl(AggressiveInlining)] on SortCore is what is under question. The attribute
// bypasses the JIT's size heuristic, but the JIT still refuses some candidates outright, so
// the attribute may be inert. Run with JitDisasmSummary to see which methods were jitted
// standalone; a SortCore instantiation appearing there was NOT inlined at that call site.
//
//   $env:DOTNET_JitDisasmSummary=1
//   $env:DOTNET_TieredCompilation=0
//   dotnet run -c Release sandbox/DotnetFiles/InsertionSortInlineProbe.cs

using SortAlgorithm.Algorithms;

var rng = new Random(42);

// Small ranges so the insertion-sort fallback of each hybrid is what actually runs.
for (var round = 0; round < 200; round++)
{
    var a = new int[64];
    for (var i = 0; i < a.Length; i++) a[i] = rng.Next();

    var b = a.ToArray();
    var c = a.ToArray();
    var d = a.ToArray();

    InsertionSort.Sort(a.AsSpan());
    PDQSort.Sort(b.AsSpan());
    IntroSort.Sort(c.AsSpan());
    StdSort.Sort(d.AsSpan());

    if (!IsSorted(a) || !IsSorted(b) || !IsSorted(c) || !IsSorted(d)) throw new Exception("not sorted");
}

Console.WriteLine("done");

static bool IsSorted(int[] x)
{
    for (var i = 1; i < x.Length; i++) if (x[i - 1] > x[i]) return false;
    return true;
}
