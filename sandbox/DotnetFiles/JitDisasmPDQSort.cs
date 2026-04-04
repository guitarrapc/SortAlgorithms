#:sdk Microsoft.NET.Sdk
#:property TargetFramework=net10.0
#:project ../../src/SortAlgorithm
using SortAlgorithm.Algorithms;

// Use public API (internal ComparableComparer is instantiated inside Sort<T>)
var data = new int[4096];
var rng = new Random(42);
for (var i = 0; i < data.Length; i++) data[i] = rng.Next();

// Warm up — force JIT to compile the full call chain
PDQSort.Sort<int>(data.AsSpan());

// Sort again so JIT output covers the hot path
rng = new Random(42);
for (var i = 0; i < data.Length; i++) data[i] = rng.Next();
PDQSort.Sort<int>(data.AsSpan());

Console.WriteLine("Done");
